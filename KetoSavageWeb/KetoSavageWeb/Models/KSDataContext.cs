using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class KSDataContext : DbContext
    {
        public KSDataContext() : base("KSDataConnection")
        {
            //Configuration.ProxyCreationEnabled = false;
        }

        //static KSDataContext()
        //{
        //    Database.SetInitializer<KSDataContext>(new CreateDatabaseIfNotExists<KSDataContext>());
            
        //}

        public static KSDataContext Create()
        {
            return new KSDataContext();
        }

        public DbSet<ApplicationUser> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ProgramModels> Programs { get; set; }
        public DbSet<UserPrograms> UserPrograms { get; set; }

        public bool RequireUniqueEmail { get; set; }

        public static Role[] DefaultRoles =
        {
            new Role
            {
                Name = "Administrator"
            },
            new Role
            {
                Name = "Coach"
            },
            new Role
            {
                Name = "Registered User"
            },
            new Role
            {
                Name = "Client"
            }
        };

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException("modelBuilder");

            base.OnModelCreating(modelBuilder);


            //Setup conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Roles)
                .WithRequired()
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserPrograms>()
                .HasRequired(c => c.ProgramUser)
                .WithMany(u => u.UserPrograms);

            modelBuilder.Entity<UserPrograms>()
                .HasOptional(c => c.CoachUser);

            // AspNet.Identity models
            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            var user = modelBuilder.Entity<ApplicationUser>();     //.ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));
            user.Property(u => u.Email)
                .HasMaxLength(0x100);

            modelBuilder.Entity<UserRole>()
                .HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId });      //.ToTable("AspNetUserRoles");
            modelBuilder.Entity<UserLogin>()
                .HasKey(l => new { LoginProvider = l.LoginProvider, ProviderKey = l.ProviderKey, UserId = l.UserId })
                .ToTable("AspNetUserLogins");
            modelBuilder.Entity<UserClaim>()
                .ToTable("AspNetUserClaims");

            var role = modelBuilder.Entity<Role>();       //.ToTable("AspNetRoles");
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }

        internal void CreateDefaultRoles()
        {
            foreach (var r in DefaultRoles)
            {
                var role = this.Roles.FirstOrDefault(x => x.Name == r.Name);
                if (role == null)
                {
                    // Create role
                    this.Roles.Add(r);
                }
                //else
                //{
                //    // Check for new permissions
                //    foreach (var rp in r.Permissions.Where(x => !role.Permissions.Any(y => y.Permission == x.Permission)))
                //    {
                //        role.Permissions.Add(new RolePermission { Permission = rp.Permission });
                //    }
                //}
            }
        }

        protected override System.Data.Entity.Validation.DbEntityValidationResult ValidateEntity(System.Data.Entity.Infrastructure.DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as ApplicationUser;
                //check for uniqueness of user name and email
                if (user != null)
                {
                    if (Users.Any(u => String.Equals(u.UserName, user.UserName)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, "User name {0} is already taken.", user.UserName)));
                    }
                    if (RequireUniqueEmail && Users.Any(u => String.Equals(u.Email, user.Email)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, "Email {0} is already taken.", user.Email)));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as Role;
                    //check for uniqueness of role name
                    if (role != null && Roles.Any(r => String.Equals(r.Name, role.Name)))
                    {
                        errors.Add(new DbValidationError("Role",
                            String.Format(CultureInfo.CurrentCulture, "Role {0} already exists.", role.Name)));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }
}