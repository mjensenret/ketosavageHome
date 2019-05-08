using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class DateRepository : IDateRepository
    {
        private KSDataContext _context = new KSDataContext();
        public DateRepository(IEntityContext<DateModels> entityContext)
        {
        }

        public int GetWeekNum(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault();
            
            return model.ISOWeekOfYear;
        }

        public IQueryable<DateModels> GetPastWeeks(DateTime currentDate)
        {
            var startWeekNum = this.GetWeekNum(currentDate) - 4;
            var currentWeekNum = this.GetWeekNum(currentDate);
            var model = _context.DateModels.Where(x => x.ISOWeekOfYear >= startWeekNum && x.ISOWeekOfYear < currentWeekNum);

            return model;
        }

        public int getDateKey(DateTime date)
        {
            
            return _context.DateModels.Where(x => x.Date == date.Date).Select(y => y.DateKey).First();
        }

        public IQueryable<DateModels> GetDatesByRange(DateTime startDate, DateTime endDate)
        {
            return _context.DateModels.Where(x => x.Date >= startDate.Date && x.Date <= endDate);
        }

        public int getLastWeekNumber(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault().ISOWeekOfYear - 1;
            return model;
        }
        public int getCurrentWeekNumber(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate.Date).FirstOrDefault().ISOWeekOfYear;
            return model;
        }

        public DateTime GetNextWeek(DateTime currentDate)
        {
            var dateModel = _context.DateModels.Where(x => x.Date == currentDate.Date);
            var nextIsoWeek = dateModel.Select(x => x.ISOWeekOfYear).First() + 1;
            var newDate = _context.DateModels.Where(x => x.ISOWeekOfYear == nextIsoWeek && x.Year == currentDate.Year).First();
            return newDate.Date;
        }
    }
}