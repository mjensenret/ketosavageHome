using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class KSDataContext : IdentityDbContext
    {
        public KSDataContext() : base("KSDataConnection")
        {

        }

        static KSDataContext()
        {
            //Database.SetInitializer<KSDataContext>(new ApplicationDbInitializer());
        }

        public static KSDataContext Create()
        {
            return new KSDataContext();
        }

        //DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<ApplicationUser> ApplicationUser { get; set; }
        DbSet<ProgramModels> Programs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProgramModels>()
                .HasRequired(p => p.ApplicationUser)
                .WithMany(user => user.UserPrograms);
        }
    }
}