using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class ClientListViewModel
    {
        public string userName { get; set; }
        public string FullName { get; set; }
        public DateTime? currentProgramStartDate { get; set; }
        public DateTime? currentProgramEndDate { get; set; }
        public DateTime? currentProgramRenewalDate { get; set; }

        public virtual ProgramModels program { get; set; }

    }
}