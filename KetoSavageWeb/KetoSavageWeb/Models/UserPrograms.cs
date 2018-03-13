using KetoSavageWeb.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class UserPrograms : UserManaged
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string Notes { get; set; }
        public int ProgramUserId { get; set; }
        public virtual ApplicationUser ProgramUser { get; set; }
        public int MasterProgramId { get; set; }
        public virtual ProgramTemplate MasterProgram { get; set; }
        public int? CoachUserId { get; set; }
        public virtual ApplicationUser CoachUser { get; set; }

    }
}