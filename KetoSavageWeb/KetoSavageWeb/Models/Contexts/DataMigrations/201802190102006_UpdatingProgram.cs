namespace KetoSavageWeb.Models.Contexts.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramModels", "userId", c => c.Int(nullable: false));
            AddColumn("dbo.CoachedPrograms", "coachId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CoachedPrograms", "coachId");
            DropColumn("dbo.ProgramModels", "userId");
        }
    }
}
