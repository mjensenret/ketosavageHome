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
    public class ProgramModels : UserManaged
    {
        public string programGoal { get; set; }
        public string programDescription { get; set; }
        public ICollection<UserPrograms> UserPrograms { get; set; }
    }

    public enum ProgramGoal
    {
        Cut,
        Build
    }

}