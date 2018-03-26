using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class DateModels
    {
        [Key]
        [Required]
        public int DateKey { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int Day { get; set; }
        [Required]
        public string DaySuffix { get; set; }
        [Required]
        public int Weekday { get; set; }
        [Required]
        public string WeekDayName { get; set; }
        [Required]
        public bool IsWeekend { get; set; }
        [Required]
        public bool IsHoliday { get; set; }
        public string HolidayText { get; set; }
        [Required]
        public int DOWInMonth { get; set; }
        [Required]
        public int DayOfYear { get; set; }
        [Required]
        public int WeekOfMonth { get; set; }
        [Required]
        public int WeekOfYear { get; set; }
        [Required]
        public int ISOWeekOfYear { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public string MonthName { get; set; }
        [Required]
        public int Quarter { get; set; }
        [Required]
        public string QuarterName { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string MMYYYY { get; set; }
        [Required]
        public string MonthYear { get; set; }
        [Required]
        public DateTime FirstDayOfMonth { get; set; }
        [Required]
        public DateTime LastDayOfMonth { get; set; }
        [Required]
        public DateTime FirstDayOfQuarter { get; set; }
        [Required]
        public DateTime LastDayOfQuarter { get; set; }
        [Required]
        public DateTime FirstDayOfYear { get; set; }
        [Required]
        public DateTime LastDayOfYear { get; set; }
        [Required]
        public DateTime FirstDayOfNextMonth { get; set; }
        [Required]
        public DateTime FirstDayOfNextYear { get; set; }
    }
}