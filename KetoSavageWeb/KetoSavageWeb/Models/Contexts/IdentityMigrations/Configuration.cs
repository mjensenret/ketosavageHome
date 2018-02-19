namespace KetoSavageWeb.Models.Contexts.IdentityMigrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;

    internal sealed class Configuration : DbMigrationsConfiguration<KetoSavageWeb.Models.Contexts.KSIdentityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Models\Contexts\IdentityMigrations";
            ContextKey = "KetoSavageWeb.Models.Contexts.KSIdentityContext";
        }

        protected override void Seed(KetoSavageWeb.Models.Contexts.KSIdentityContext context)
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
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));

            const string adminName = "superUser";
            const string adminEmail = "mjensen@razoredgetech.com";
            const string adminPassword = "S4v4g3!";
            const string adminRoleName = "Admin";
            const string adminFirstName = "Super";
            const string adminLastName = "User";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(adminRoleName);
            if (role == null)
            {
                role = new ApplicationRole(adminRoleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(adminName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = adminName, Email = adminEmail, FirstName = adminFirstName, LastName = adminLastName };
                var result = userManager.Create(user, adminPassword);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }

            var registeredUserRole = roleManager.FindByName("Registered User");
            if (registeredUserRole == null)
            {
                role = new ApplicationRole("Registered User");
                var roleresult = roleManager.Create(role);
            }

            var clientUserRole = roleManager.FindByName("Client");
            if (clientUserRole == null)
            {
                role = new ApplicationRole("Client");
                var roleresult = roleManager.Create(role);
            }

            var coachRole = roleManager.FindByName("Coach");
            if (coachRole == null)
            {
                role = new ApplicationRole("Coach");
                var roleresult = roleManager.Create(role);
            }
        }
    }
}
