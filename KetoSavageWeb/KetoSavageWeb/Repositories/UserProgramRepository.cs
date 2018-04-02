using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
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
                .Where(x => x.Dates.WeekOfYear >= startWeek && x.Dates.WeekOfYear <= currentWeek)
                .OrderByDescending(x => x.DateId)
                );

            return pastProgress;
        }

    }
}