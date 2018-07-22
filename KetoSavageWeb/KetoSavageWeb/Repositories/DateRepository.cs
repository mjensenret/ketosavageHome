using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class DateRepository
    {
        private KSDataContext _context = new KSDataContext();
        public DateRepository(IEntityContext<DateModels> entityContext)
        {
        }

        public int GetWeekNum(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault();
            
            return model.WeekOfYear;
        }

        public IQueryable<DateModels> GetPastWeeks(DateTime currentDate)
        {
            var startWeekNum = this.GetWeekNum(currentDate) - 4;
            var currentWeekNum = this.GetWeekNum(currentDate);
            var model = _context.DateModels.Where(x => x.WeekOfMonth >= startWeekNum && x.WeekOfMonth < currentWeekNum);

            return model;
        }

        public int getDateKey(DateTime date)
        {
            return _context.DateModels.Where(x => x.Date == date).Select(y => y.DateKey).First();
        }

        public IQueryable<DateModels> GetDatesByRange(DateTime startDate, DateTime endDate)
        {
            return _context.DateModels.Where(x => x.Date >= startDate.Date && x.Date <= endDate);
        }

        public int getLastWeekNumber(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault().WeekOfYear - 1;
            return model;
        }
        public int getCurrentWeekNumber(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault().WeekOfYear;
            return model;
        }
    }
}