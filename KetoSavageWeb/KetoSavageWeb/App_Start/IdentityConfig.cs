using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using KetoSavageWeb.Models;
using KetoSavageWeb.Properties;

namespace KetoSavageWeb.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>(context.Get<KSDataContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser, int>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser, int>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<Role, int>
    {
        public ApplicationRoleManager(RoleStore<Role, int, UserRole> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<Role, int, UserRole>(context.Get<KSDataContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<KSDataContext>
    {
        protected override void Seed(KSDataContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(KSDataContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            

            //Create Roles
            var role = roleManager.FindByName("Administrator");
            if (role == null)
            {
                role = new Role("Administrator");
                var roleresult = roleManager.Create(role);
            }

            //Create coach role if it does not exist
            var coachRole = roleManager.FindByName("Coach");
            if (coachRole == null)
            {
                coachRole = new Role("Coach");
                var coachRoleResult = roleManager.Create(coachRole);
            }

            var registeredUserRole = roleManager.FindByName("Registered User");
            if (registeredUserRole == null)
            {
                registeredUserRole = new Role("Registered User");
                var rURResult = roleManager.Create(registeredUserRole);
            }

            var clientUserRole = roleManager.FindByName("Client");
            if (clientUserRole == null)
            {
                clientUserRole = new Role("Client");
                var cuResult = roleManager.Create(clientUserRole);
            }

            var admin = userManager.FindByName(Settings.Default.DefaultAdminAccount);
            if (admin == null)
            {
                admin = new ApplicationUser {
                    UserName = Settings.Default.DefaultAdminAccount,
                    Email = Settings.Default.DefaultAdminEmail,
                    FirstName = "Super",
                    LastName = "User",
                    };
                var result = userManager.Create(admin, Settings.Default.DefaultPassword);
                result = userManager.SetLockoutEnabled(admin.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(admin.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(admin.Id, role.Name);
            }

            //Add coach
            var coach = userManager.FindByName("RobertSikes");
            if (coach == null)
            {
                coach = new ApplicationUser
                {
                    UserName = "RobertSikes",
                    Email = "chief@ketosavage.com",
                    FirstName = "Robert",
                    LastName = "Sikes"
                };
            }

            //add coach to role
            var coachUserRole = userManager.GetRoles(coach.Id);
            if (!coachUserRole.Contains("Coach"))
            {
                var result = userManager.AddToRole(coach.Id, coachRole.Name);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}