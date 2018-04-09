using DevExpress.Web.Mvc;
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
            var currentWeek = db.DateModels.Where(x => x.Date == currentDate.Date).Select(y => y.WeekOfYear).First();
            var startWeek = currentWeek - 4;
            var up = this.GetActive.Where(x => x.ProgramUserId == userId).Include(x => x.DailyProgress).SelectMany(x => x.DailyProgress);
            var pastProgress = (up
                .Where(x => x.Dates.WeekOfYear >= startWeek && x.Dates.WeekOfYear < currentWeek)
                .OrderByDescending(x => x.DateId)
                );

            return pastProgress;
        }

        //public static void InsertNewItem(DailyMacroUpdate postedItem, MVCxGridViewBatchUpdateValues<DailyMacroUpdate, int> batchValues)
        //{
        //    try
        //    {

        //        var newItem = new DailyProgress();
        //        LoadNewValues(newItem, postedItem);
        //        GridData.Add(newItem);

        //    }
        //    catch (Exception e)
        //    {
        //        batchValues.SetErrorText(postedItem, e.Message);
        //    }
        //}

        public List<DailyProgress> GetDailyProgressList()
        {
            var dp = db.DailyProgress.ToList();
            return dp;
        }
        public void UpdateItem(UpdateMacrosViewModel postedItem, MVCxGridViewBatchUpdateValues<UpdateMacrosViewModel, int> batchValues)
        {
            try
            {
                var editedItem = db.DailyProgress.First(i => i.Id == postedItem.Id);
                LoadNewValues(editedItem, postedItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                batchValues.SetErrorText(postedItem, e.Message);
            }
        }
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