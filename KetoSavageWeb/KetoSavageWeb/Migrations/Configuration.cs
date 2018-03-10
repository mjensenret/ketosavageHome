namespace KetoSavageWeb.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KetoSavageWeb.Models.KSDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(KetoSavageWeb.Models.KSDataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.CreateDefaultRoles();

            var adminRole = context.Roles.Where(x => x.Name == "Administrator").FirstOrDefault();
            var coachRole = context.Roles.Where(x => x.Name == "Coach").FirstOrDefault();
            //if (adminRole == null)
            //{
            //    adminRole = new Role { Name = "Administrator" };
            //    context.Roles.Add(adminRole);
            //}
            var manager = ApplicationUserManager.Create(context);
            manager.CreateAdminAccount(adminRole, coachRole);
        }
    }
}
