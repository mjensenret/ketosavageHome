using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class KSDataContext : IdentityDbContext<ApplicationUser>
    {
        public KSDataContext() : base("KSDataContext")
        {

        }

        DbSet<UserProfile> UserProfiles { get; set; }
    }
}