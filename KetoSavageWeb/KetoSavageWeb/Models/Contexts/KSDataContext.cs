using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models.Contexts
{
    public class KSDataContext : DbContext
    {
        public KSDataContext() : base("KSDataConnection")
        {

        }

        //DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<ProgramModels> Programs { get; set; }
    }
}