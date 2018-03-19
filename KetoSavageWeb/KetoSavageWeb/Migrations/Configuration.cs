namespace KetoSavageWeb.Migrations
{
    using Microsoft.AspNet.Identity.Owin;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using Properties;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<KetoSavageWeb.Models.KSDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            
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
            try
            {
                ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>(context));
                ApplicationRoleManager roleMgr = new ApplicationRoleManager(new RoleStore<Role, int, UserRole>(context));

                context.CreateDefaultRoles();

                var adminUser = userMgr.FindByName(Settings.Default.DefaultAdminAccount);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = Settings.Default.DefaultAdminAccount,
                        Email = Settings.Default.DefaultAdminEmail,
                        FirstName = "Super",
                        LastName = "User"
                    };
                    userMgr.Create(adminUser, Settings.Default.DefaultPassword);
                }
                if (!userMgr.GetRoles(adminUser.Id).Contains("Administrator"))
                {
                    userMgr.AddToRole(adminUser.Id, "Administrator");
                }

                var defaultCoach = userMgr.FindByName(Settings.Default.DefaultCoach);
                if (defaultCoach == null)
                {
                    defaultCoach = new ApplicationUser
                    {
                        UserName = Settings.Default.DefaultCoach,
                        Email = Settings.Default.DefaultCoachEmail,
                        FirstName = "Robert",
                        LastName = "Sikes"
                    };
                    userMgr.Create(defaultCoach, Settings.Default.DefaultPassword);
                }
                if (!userMgr.GetRoles(defaultCoach.Id).Contains("Coach"))
                {
                    userMgr.AddToRole(defaultCoach.Id, "Coach");
                }
                var cutGoal = new ProgramGoals
                {
                    Name = "Cut",
                    Description = "Standard cut"
                };
                var buildGoal = new ProgramGoals
                {
                    Name = "Build",
                    Description = "Standard bulking program"
                };
                var competitionPrep = new ProgramGoals
                {
                    Name = "CompPrep",
                    Description = "Competition Prep"
                };
                var maintGoal = new ProgramGoals
                {
                    Name = "Maint",
                    Description = "Maintenance Goal"

                };
                

                var cutProgram = new ProgramTemplate
                {
                    Goal = cutGoal,
                    Name = "Cut",
                    programDescription = "Reduce bodyfat",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"

                };
                context.Programs.AddOrUpdate(p => new { p.programDescription }, cutProgram);

                var buildProgram = new ProgramTemplate
                {
                    Goal = buildGoal,
                    Name = "Build",
                    programDescription = "Build Muscle",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"
                };
                context.Programs.AddOrUpdate(p => new { p.programDescription }, buildProgram);

                var compProgram = new ProgramTemplate
                {
                    Goal = competitionPrep,
                    Name = "CompetitionPrep",
                    programDescription = "Competition Prep",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"
                };

                context.Programs.AddOrUpdate(p => new { p.programDescription }, compProgram);

                var maintProgram = new ProgramTemplate
                {
                    Goal = maintGoal,
                    Name = "Maintenance",
                    programDescription = "Maintenance Program",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"
                };

                context.Programs.AddOrUpdate(p => new { p.programDescription }, maintProgram);

                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }

        }
    }
}
