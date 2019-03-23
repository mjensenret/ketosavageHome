using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KetoSavageWeb.Repositories
{
    public interface IDateRepository
    {
        int GetWeekNum(DateTime currentDate);
        int getDateKey(DateTime date);
    }
}
