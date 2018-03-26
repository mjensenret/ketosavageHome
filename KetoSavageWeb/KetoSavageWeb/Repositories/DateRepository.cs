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

        public int GetCurrentWeek(DateTime currentDate)
        {
            var model = _context.DateModels.Where(x => x.Date == currentDate).FirstOrDefault();
            
            return model.WeekOfYear;
        }
    }
}