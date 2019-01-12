using KetoSavageWeb.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class DailyProgress : UserManaged
    {
        [ForeignKey("Dates")]
        public int DateId { get; set; }
        public double? PlannedWeight { get; set; }
        public double? ActualWeight { get; set; }
        public double? PlannedFat { get; set; }
        public double? ActualFat { get; set; }
        public double? PlannedProtein { get; set; }
        public double? ActualProtein { get; set; }
        public double? PlannedCarbohydrate { get; set; }
        public double? ActualCarbohydrate { get; set; }
        public string HungerLevel { get; set; }
        public bool IsRefeed { get; set; }
        public string Notes { get; set; }
        [ForeignKey("UserProgram")]
        public int UserProgramId { get; set; }
        public virtual UserPrograms UserProgram { get; set; }
        public virtual DateModels Dates { get; set; }
    }
}