using KetoSavageWeb.Models.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public abstract class ProgramModels : UserManaged
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract ProgramType Type { get; }

        public static ProgramModels CreateByType(ProgramType type)
        {
            if (type == ProgramType.Coached)
                return new CoachedPrograms();
            else if (type == ProgramType.SelfGuided)
                return new SelfGuidedPrograms();
            else
                throw new ApplicationException(string.Format("Unsupported program type: {0}", type));
        }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string programGoal { get; set; }
        public string programNotes { get; set; }
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ProgramUser { get; set; }
    }

    [Table("CoachedPrograms")]
    public class CoachedPrograms : ProgramModels
    {
        public override ProgramType Type { get { return ProgramType.Coached; } }
        public DateTime renewalDate { get; set; }

        public int CoachUserId { get; set; }
        public virtual ApplicationUser CoachUser { get; set; }

        public CoachedPrograms() : base()
        {

        }
    }

    [Table("SelfGuidedPrograms")]
    public class SelfGuidedPrograms : ProgramModels
    {
        public override ProgramType Type { get { return ProgramType.SelfGuided; } }
        public SelfGuidedPrograms() : base()
        {

        }
    }

    public enum ProgramType
    {
        Coached,
        SelfGuided
    }

}