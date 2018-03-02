using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models.Contexts
{
    public class KSDataContext : IdentityDbContext
    {
        public KSDataContext() : base("KSDataConnection")
        {

        }

        //DbSet<UserProfile> UserProfiles { get; set; }
        //DbSet<ApplicationUser> Users { get; set; }
        DbSet<ProgramModels> Programs { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<ProgramModels>()
        //        .HasRequired(p => p.user)
        //        .WithMany(user => user.)
        //}
    }
}