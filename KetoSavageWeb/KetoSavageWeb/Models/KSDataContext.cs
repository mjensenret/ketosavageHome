using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class KSDataContext : DbContext
    {
        public KSDataContext() : base("KSDataContext")
        {

        }

        DbSet<UserProfile> UserProfiles { get; set; }
    }
}