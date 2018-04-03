using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.ViewModels
{
    public class UserProgramViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public double StartWeight { get; set; }
        public double GoalWeight { get; set; }
        public string ProgramType { get; set; }
        public DateTime? currentProgramStartDate { get; set; }
        public DateTime? currentProgramEndDate { get; set; }
        public DateTime? currentProgramRenewalDate { get; set; }
        public DateTime LastModified { get; set; }
        public string UserType { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
        public int? CoachId { get; set; }
        public string CoachName { get; set; }
        public string ProgramName { get; set; }
        public int ProgramId { get; set; }
        public bool ShowAllActive { get; set; }
        public int ProgramUserId { get; set; }
        public bool IsNew { get; set; }

    }

    public class UserProgramDetails
    {
        public int Id { get; set; }
        public int ProgramUserId { get; set; }
        public string FullName { get; set; }
        public string Notes { get; set; }
        public string ProgramName { get; set; }
        public string ProgramId { get; set; }

    }

    public class UserProgramProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DateId { get; set; }
        public string WeekDay { get; set; }
        public DateTime Date { get; set; }
        public double? PlannedWeight { get; set; }
        public double? ActualWeight { get; set; }
        public double? PlannedProtein { get; set; }
        public double? ActualProtein { get; set; }
        public double? PlannedFat { get; set; }
        public double? ActualFat { get; set; }
        public double? PlannedCarbohydrates { get; set; }
        public double? ActualCarbohydrates { get; set; }
        public int UserProgramId { get; set; }
        public virtual UserPrograms UserProgram { get; set; }
        public virtual DateModels Dates { get; set; }
    }
    public class UserProgramPerformance
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double ActualWeight { get; set; }
        public double ActualProtein { get; set; }
        public double ActualFat { get; set; }
        public int UserProgramId { get; set; }
        public virtual UserPrograms UserProgram { get; set; }
        public virtual DateModels Dates { get; set; }

    }
}