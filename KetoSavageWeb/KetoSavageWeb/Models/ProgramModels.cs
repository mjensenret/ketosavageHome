using KetoSavageWeb.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public abstract class ProgramModels : UserManaged
    {   
        public int Id { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string programGoal { get; set; }
        public string programNotes { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }

    [Table("CoachedPrograms")]
    public class CoachedPrograms : ProgramModels
    {
        public DateTime renewalDate { get; set; }

        public string CoachId { get; set; }
        public ApplicationUser Coach { get; set; }

        public CoachedPrograms() : base()
        {

        }
    }

    [Table("SelfGuidedPrograms")]
    public class SelfGuidedPrograms : ProgramModels
    {
        public SelfGuidedPrograms() : base()
        {

        }
    }

}