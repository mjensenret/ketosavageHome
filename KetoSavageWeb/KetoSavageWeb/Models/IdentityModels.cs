//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System.Data.Entity;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;
//using System.Collections;
//using System.Collections.Generic;

//namespace KetoSavageWeb.Models
//{
//    public class ApplicationUser : IdentityUser
//    {
//        public async Task<ClaimsIdentity>
//            GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
//        {
//            var userIdentity = await manager
//                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
//            return userIdentity;
//        }

//        public string Address { get; set; }
//        public string City { get; set; }
//        public string State { get; set; }

//        [Display(Name = "First Name")]
//        [Required]
//        public string FirstName { get; set; }

//        [Display(Name = "Last Name")]
//        [Required]
//        public string LastName { get; set; }

//        // Use a sensible display name for views:
//        [Display(Name = "Postal Code")]
//        public string PostalCode { get; set; }
//        [Display(Name = "Notes")]
//        [StringLength(255)]
//        public string Notes { get; set; }

        
//        // Concatenate the address info for display in tables and such:
//        public string DisplayAddress
//        {
//            get
//            {
//                string dspAddress = string.IsNullOrWhiteSpace(this.Address) ? "" : this.Address;
//                string dspCity = string.IsNullOrWhiteSpace(this.City) ? "" : this.City;
//                string dspState = string.IsNullOrWhiteSpace(this.State) ? "" : this.State;
//                string dspPostalCode = string.IsNullOrWhiteSpace(this.PostalCode) ? "" : this.PostalCode;
//                return string.Format("{0} {1} {2} {3}", dspAddress, dspCity, dspState, dspPostalCode);
//            }
//        }

//        public virtual ICollection<ProgramModels> UserPrograms { get; set; }

//    }


//    public class ApplicationRole : IdentityRole
//    {
//        public ApplicationRole() : base() { }
//        public ApplicationRole(string name) : base(name) { }
//        public string Description { get; set; }

//    }


//    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public ApplicationDbContext()
//            : base("KSDataConnection", throwIfV1Schema: false)
//        {
//        }

//        static ApplicationDbContext()
//        {
//            Database.SetInitializer(new ApplicationDbInitializer());
//        }

//        public static ApplicationDbContext Create()
//        {
//            return new ApplicationDbContext();
//        }
//    }
//}