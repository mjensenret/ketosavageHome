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
        public virtual ICollection<UserPrograms> UserPrograms { get; set; }

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

    public class Role : IdentityRole<int, UserRole>, IRole<int>
    {
        public Role() : base () { }
        public Role(string name)
            : this()
        {
            this.Name = name;
        }

        public Role(string name, string description)
            : this(name)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }
}