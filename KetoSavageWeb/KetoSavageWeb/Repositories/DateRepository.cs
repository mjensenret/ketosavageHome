using KetoSavageWeb.Domain.Infrastructure;
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
            var model = _context.DateModels.Where(x => x.Date == currentDate).FirstOrDefault();
            
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
    }
}