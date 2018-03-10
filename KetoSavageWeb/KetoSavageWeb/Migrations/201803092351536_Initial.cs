namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ApplicationUsers", newName: "ApplicationUser");
            RenameTable(name: "dbo.UserRoles", newName: "UserRole");
            RenameTable(name: "dbo.Roles", newName: "Role");
            RenameTable(name: "dbo.IdentityUserLogins", newName: "IdentityUserLogin");
            RenameTable(name: "dbo.IdentityRoles", newName: "IdentityRole");
            RenameTable(name: "dbo.IdentityUserRoles", newName: "IdentityUserRole");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.IdentityUserRole", newName: "IdentityUserRoles");
            RenameTable(name: "dbo.IdentityRole", newName: "IdentityRoles");
            RenameTable(name: "dbo.IdentityUserLogin", newName: "IdentityUserLogins");
            RenameTable(name: "dbo.Role", newName: "Roles");
            RenameTable(name: "dbo.UserRole", newName: "UserRoles");
            RenameTable(name: "dbo.ApplicationUser", newName: "ApplicationUsers");
        }
    }
}
