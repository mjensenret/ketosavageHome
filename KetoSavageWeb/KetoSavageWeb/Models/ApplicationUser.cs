using KetoSavageWeb.Domain.Models;
using KetoSavageWeb.Properties;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class ApplicationUser : IdentityUser<int, UserLogin, UserRole, UserClaim>, IUserManaged, IUser<int>, IKeyedEntity<int>
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        [Editable(false)]
        public DateTime Created { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; }

        [Editable(false)]
        [Display(Name = "Created By"), MaxLength(50)]
        public string CreatedBy { get; set; }

        [Display(Name = "Last Modified By"), MaxLength(50)]
        public string LastModifiedBy { get; set; }

        //Navigation Properties
        public virtual ICollection<ProgramModels> UserPrograms { get; set; }
        public virtual ICollection<CoachedPrograms> CoachPrograms { get; set; }

        public ApplicationUser()
            : base()
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.Created = DateTime.UtcNow;
            this.LastModified = this.Created;
        }

        //Methods

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            //Note: the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public bool IsNew
        {
            get { return this.Id == default(int); }
        }
    }

    public class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }
    }

    public class UserLogin : IdentityUserLogin<int> { }

    public class UserClaim : IdentityUserClaim<int> { }

    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(KSDataContext context)
        {
            return Create(new UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>(context));

        }

        public static ApplicationUserManager Create(IUserStore<ApplicationUser, int> userStore)
        {
            var manager = new ApplicationUserManager(userStore);

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
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser, int>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            return manager;
        }

        /// <summary>
        /// Check if the specified user belongs to a role that has the requested permission.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Permission"></param>
        /// <returns></returns>
        //public bool HasPermission(int UserId, SFVPermission Permission)
        //{
        //    if (UserId == 0)
        //        return false;

        //    // Build role query
        //    var q = this.Users.Where(u => u.Id == UserId).SelectMany(x => x.Roles);

        //    // Check if user has all permissions
        //    if (q.Any(ur => ur.Role.HasAllPermissions))
        //        return true;

        //    // Check if user has permission
        //    return q.Any(ur => ur.Role.Permissions.Any(y => y.Permission == Permission));
        //}

        public void CreateAdminAccount(Role adminRole)
        {
            var task1 = this.Store.FindByNameAsync("SuperUser");
            task1.Wait();
            var admin = task1.Result;
            if (admin == null)
            {
                // Create admin account
                var user = new ApplicationUser { UserName = Settings.Default.DefaultAdminAccount, Email = Settings.Default.DefaultAdminEmail, EmailConfirmed = true };
                user.Roles.Add(new UserRole { RoleId = adminRole.Id, Role = adminRole });
                var result = this.Create(user, Settings.Default.DefaultPassword);

                if (!result.Succeeded)
                    throw new ApplicationException(string.Format("Error creating admin user: {0}", string.Join(", ", result.Errors)));
            }
        }

    }
}