namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
           
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(maxLength: 256),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(nullable: false, maxLength: 256),
            //            Address = c.String(),
            //            City = c.String(),
            //            State = c.String(),
            //            FirstName = c.String(),
            //            LastName = c.String(),
            //            PostalCode = c.String(),
            //            Notes = c.String(maxLength: 255),
            //            Discriminator = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId })
            //    .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId)
            //    .Index(t => t.RoleId);
            
            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 256),
            //            Description = c.String(),
            //            Discriminator = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.Name, unique: true, name: "RoleNameIndex");

           //CreateTable(
           //     "dbo.ProgramModels",
           //     c => new
           //         {
           //             Id = c.Int(nullable: false, identity: true),
           //             startDate = c.DateTime(nullable: false),
           //             endDate = c.DateTime(nullable: false),
           //             programGoal = c.String(),
           //             programNotes = c.String(),
           //             IsActive = c.Boolean(nullable: false),
           //             IsDeleted = c.Boolean(nullable: false),
           //             Created = c.DateTime(nullable: false),
           //             LastModified = c.DateTime(nullable: false),
           //             CreatedBy = c.String(maxLength: 50),
           //             LastModifiedBy = c.String(maxLength: 50),
           //             ApplicationUser_Id = c.String(maxLength: 128),
           //         })
           //     .PrimaryKey(t => t.Id)
           //     .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
           //     .Index(t => t.ApplicationUser_Id);
            
           // CreateTable(
           //     "dbo.CoachedPrograms",
           //     c => new
           //         {
           //             Id = c.Int(nullable: false),
           //             Coach_Id = c.String(maxLength: 128),
           //             renewalDate = c.DateTime(nullable: false),
           //             CoachId = c.Int(nullable: false),
           //         })
           //     .PrimaryKey(t => t.Id)
           //     .ForeignKey("dbo.ProgramModels", t => t.Id)
           //     .ForeignKey("dbo.AspNetUsers", t => t.Coach_Id)
           //     .Index(t => t.Id)
           //     .Index(t => t.Coach_Id);
            
           // CreateTable(
           //     "dbo.SelfGuidedPrograms",
           //     c => new
           //         {
           //             Id = c.Int(nullable: false),
           //         })
           //     .PrimaryKey(t => t.Id)
           //     .ForeignKey("dbo.ProgramModels", t => t.Id)
           //     .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.SelfGuidedPrograms", "Id", "dbo.ProgramModels");
            //DropForeignKey("dbo.CoachedPrograms", "Coach_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.CoachedPrograms", "Id", "dbo.ProgramModels");
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            //DropForeignKey("dbo.ProgramModels", "ApplicationUser_Id", "dbo.AspNetUsers");
            //DropIndex("dbo.SelfGuidedPrograms", new[] { "Id" });
            //DropIndex("dbo.CoachedPrograms", new[] { "Coach_Id" });
            //DropIndex("dbo.CoachedPrograms", new[] { "Id" });
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.ProgramModels", new[] { "ApplicationUser_Id" });
            //DropTable("dbo.SelfGuidedPrograms");
            //DropTable("dbo.CoachedPrograms");
            //DropTable("dbo.AspNetRoles");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetUserLogins");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.ProgramModels");
        }
    }
}
