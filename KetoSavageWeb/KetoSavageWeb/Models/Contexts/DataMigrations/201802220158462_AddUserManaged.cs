namespace KetoSavageWeb.Models.Contexts.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserManaged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramModels", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProgramModels", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProgramModels", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProgramModels", "LastModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProgramModels", "CreatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.ProgramModels", "LastModifiedBy", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramModels", "LastModifiedBy");
            DropColumn("dbo.ProgramModels", "CreatedBy");
            DropColumn("dbo.ProgramModels", "LastModified");
            DropColumn("dbo.ProgramModels", "Created");
            DropColumn("dbo.ProgramModels", "IsDeleted");
            DropColumn("dbo.ProgramModels", "IsActive");
        }
    }
}
