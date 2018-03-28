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
        public string ProgramType { get; set; }
        public DateTime? currentProgramStartDate { get; set; }
        public DateTime? currentProgramEndDate { get; set; }
        public DateTime? currentProgramRenewalDate { get; set; }
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
}