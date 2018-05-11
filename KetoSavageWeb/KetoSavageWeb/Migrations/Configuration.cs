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
            //-----------------Uncomment to debug seed method----------------------------------//
            if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }
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
                
                var cutGoal = context.ProgramGoals.Where(cg => cg.Name == "Cut").FirstOrDefault();
                if (cutGoal == null)
                {
                    cutGoal = new ProgramGoals
                    {
                        Name = "Cut",
                        Description = "Standard cut"
                    };
                }
                context.ProgramGoals.AddOrUpdate(g => new { g.Name }, cutGoal);

                var buildGoal = context.ProgramGoals.Where(cg => cg.Name == "Build").FirstOrDefault();
                if (buildGoal == null)
                {
                    buildGoal = new ProgramGoals
                    {
                        Name = "Build",
                        Description = "Standard bulking program"
                    };
                }
                context.ProgramGoals.AddOrUpdate(g => new { g.Name }, buildGoal);

                var competitionPrep = context.ProgramGoals.Where(cg => cg.Name == "CompPrep").FirstOrDefault();
                if (competitionPrep == null)
                {
                    competitionPrep = new ProgramGoals
                    {
                        Name = "CompPrep",
                        Description = "Competition Prep"
                    };
                }
                context.ProgramGoals.AddOrUpdate(g => new { g.Name }, competitionPrep);

                var maintGoal = context.ProgramGoals.Where(cg => cg.Name == "Maint").FirstOrDefault();
                if (maintGoal == null)
                {
                    maintGoal = new ProgramGoals
                    {
                        Name = "Maint",
                        Description = "Maintenance Goal"

                    };
                }
                context.ProgramGoals.AddOrUpdate(g => new { g.Name }, maintGoal);

                context.SaveChanges();

                var cutProgram = new ProgramTemplate
                {
                    GoalId = cutGoal.Id,
                    Name = "Cut",
                    programDescription = "Reduce bodyfat",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"

                };
                context.Programs.AddOrUpdate(p => new { p.programDescription }, cutProgram);

                var buildProgram = new ProgramTemplate
                {
                    GoalId = buildGoal.Id,
                    Name = "Build",
                    programDescription = "Build Muscle",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"
                };
                context.Programs.AddOrUpdate(p => new { p.programDescription }, buildProgram);

                var compProgram = new ProgramTemplate
                {
                    GoalId = competitionPrep.Id,
                    Name = "CompetitionPrep",
                    programDescription = "Competition Prep",
                    CreatedBy = "Seed Method",
                    LastModifiedBy = "Seed Method"
                };

                context.Programs.AddOrUpdate(p => new { p.programDescription }, compProgram);

                var maintProgram = new ProgramTemplate
                {
                    GoalId = maintGoal.Id,
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
