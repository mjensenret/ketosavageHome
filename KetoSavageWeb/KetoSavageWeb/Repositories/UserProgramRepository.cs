using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class UserProgramRepository : UserManagedRepository<UserPrograms>
    {
        KSDataContext db = new KSDataContext();
        public UserProgramRepository(IEntityContext<UserPrograms> entityContext) : base(entityContext)
        {
        }

        public IQueryable<DailyProgress> GetDailyProgressByUser(int userId)
        {
            var up = this.GetActive.Where(x => x.ProgramUserId == userId);

            var dp = up.SelectMany(x => x.DailyProgress);

            return dp;


        }
        public IQueryable<DailyProgress> GetPastProgressByUser(int userId, DateTime currentDate)
        {
            var currentWeek = db.DateModels.Where(x => x.Date == currentDate.Date).Select(y => y.ISOWeekOfYear).First();
            var startWeek = currentWeek - 4;
            var up = this.GetActive.Where(x => x.ProgramUserId == userId).Include(x => x.DailyProgress).SelectMany(x => x.DailyProgress);
            var pastProgress = (up
                .Where(x => x.Dates.ISOWeekOfYear >= startWeek && x.Dates.ISOWeekOfYear <= currentWeek)
                .OrderByDescending(x => x.DateId)
                );

            return pastProgress;
        }

        public DailyProgress GetDailyProgressByDate(int userId, DateTime date)
        {
            var dailyProgress = this.GetActive.Where(x => x.ProgramUserId == userId).Include(x => x.DailyProgress).Select(x => x.DailyProgress.Where(y => y.Dates.Date == date)).FirstOrDefault();
            return dailyProgress.FirstOrDefault();
        }
        public IQueryable<DailyProgress> GetCurrentProgressByUser(int userId, DateTime currentDate)
        {
            //var currentWeek = db.DateModels.Where(x => x.Date == currentDate.Date).Select(y => y.WeekOfYear).First();
            var currentWeek = db.DateModels.Where(x => x.Date == currentDate.Date).First();
            var startWeek = currentWeek.ISOWeekOfYear;
            if (currentWeek.WeekDayName == "Sunday")
            {
                startWeek = currentWeek.ISOWeekOfYear - 1;
            }

            var up = this.GetActive.Where(x => x.ProgramUserId == userId).Include(x => x.DailyProgress).SelectMany(x => x.DailyProgress);
            var currentProgress = (up
                .Where(x => x.Dates.ISOWeekOfYear == startWeek)
                .OrderByDescending(x => x.DateId)
                );

            return currentProgress;
        }
        public ProgressGaugeViewModel calcWeightChangeByUser(int userId)
        {
            var userProgress = this.GetActive.Where(x => x.ProgramUserId == userId).Include(x => x.DailyProgress).SelectMany(x => x.DailyProgress);
            var actualProgress = userProgress.Where(x => x.ActualWeight != null);

            var startWeight = (from w in userProgress
                                  let minProgress = userProgress.Min(r => r.DateId)
                                  where w.DateId == minProgress
                                  select w.UserProgram.StartWeight).Sum();
            var currentProgress = 0.00;
            var plannedProgress = 0.00;
            if (actualProgress.Count() > 0)
            { 
                currentProgress = (from w in actualProgress
                                        let maxProgress = actualProgress.Max(r => r.DateId)
                                        where w.DateId == maxProgress
                                        select w.ActualWeight.Value).Sum();


                plannedProgress = (from w in actualProgress
                                   let maxProgress = actualProgress.Max(r => r.DateId)
                                   where w.DateId == maxProgress
                                   select w.PlannedWeight.Value).Sum();
            }

            ProgressGaugeViewModel model = new ProgressGaugeViewModel()
            {
                actualWeight = currentProgress > 0 ? currentProgress - startWeight : startWeight,
                plannedWeight = plannedProgress - startWeight
            };
            
            return model;
                                   
        }

        public MeasurementHeader GetMeasurements(DateTime date, int userProgramId)
        {
            var measurement = db.MeasurementHeader.Where(x => x.Dates.Date == date.Date && x.UserProgramId == userProgramId)
                .Include(y => y.MeasurementDetails).FirstOrDefault();
            return measurement;
        }

        public List<DailyProgress> GetDailyProgressList()
        {
            var dp = db.DailyProgress.ToList();
            return dp;
        }

        //public void UpdateItem(UpdateMacrosViewModel postedItem, MVCxGridViewBatchUpdateValues<UpdateMacrosViewModel, int> batchValues)
        //{
        //    try
        //    {
        //        var editedItem = db.DailyProgress.First(i => i.Id == postedItem.Id);
        //        LoadNewValues(editedItem, postedItem);
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        batchValues.SetErrorText(postedItem, e.Message);
        //    }
        //}
        //public static void DeleteItem(int itemKey, MVCxGridViewBatchUpdateValues<GridDataItem, int> batchValues)
        //{
        //    try
        //    {
        //        var item = GridData.First(i => i.ID == itemKey);
        //        GridData.Remove(item);
        //    }
        //    catch (Exception e)
        //    {
        //        batchValues.SetErrorText(itemKey, e.Message);
        //    }

        //}
        protected static void LoadNewValues(DailyProgress newItem, UpdateMacrosViewModel postedItem)
        {
            newItem.PlannedFat = postedItem.PlannedFat;
            newItem.PlannedProtein = postedItem.PlannedProtein;
            newItem.PlannedCarbohydrate = postedItem.PlannedCarbs;
            newItem.IsRefeed = postedItem.IsRefeed;
        }
    }
}