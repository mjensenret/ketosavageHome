using KetoSavageWeb.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class DailyProgressModel : UserManaged
    {
        public int DateId { get; set; }
        public decimal? PlannedWeight { get; set; }
        public decimal? ActualWeight { get; set; }
        public decimal? PlannedFat { get; set; }
        public decimal? ActualFat { get; set; }
        public decimal? PlannedProtein { get; set; }
        public decimal? ActualProtein { get; set; }
        public decimal? PlannedCarbohydrate { get; set; }
        public decimal? ActualCarbohydrate { get; set; }
        public int UserProgramId { get; set; }
        public virtual UserPrograms UserProgram { get; set; }
    }
}