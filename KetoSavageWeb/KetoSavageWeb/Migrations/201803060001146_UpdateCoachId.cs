namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCoachId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CoachedPrograms", new[] { "Coach_Id" });
            DropColumn("dbo.CoachedPrograms", "CoachId");
            RenameColumn(table: "dbo.CoachedPrograms", name: "Coach_Id", newName: "CoachId");
            AlterColumn("dbo.CoachedPrograms", "CoachId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CoachedPrograms", "CoachId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CoachedPrograms", new[] { "CoachId" });
            AlterColumn("dbo.CoachedPrograms", "CoachId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.CoachedPrograms", name: "CoachId", newName: "Coach_Id");
            AddColumn("dbo.CoachedPrograms", "CoachId", c => c.Int(nullable: false));
            CreateIndex("dbo.CoachedPrograms", "Coach_Id");
        }
    }
}
