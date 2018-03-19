namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyingGoals : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramTemplate", new[] { "Goals_Id" });
            RenameColumn(table: "dbo.ProgramTemplate", name: "Goals_Id", newName: "GoalId");
            AlterColumn("dbo.ProgramTemplate", "GoalId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProgramTemplate", "GoalId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramTemplate", new[] { "GoalId" });
            AlterColumn("dbo.ProgramTemplate", "GoalId", c => c.Int());
            RenameColumn(table: "dbo.ProgramTemplate", name: "GoalId", newName: "Goals_Id");
            CreateIndex("dbo.ProgramTemplate", "Goals_Id");
        }
    }
}
