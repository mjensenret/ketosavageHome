namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProgramTemplates : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProgramModels", newName: "ProgramTemplateModels");
            CreateTable(
                "dbo.ProgramGoals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ProgramTemplateModels", "goals_Id", c => c.Int());
            CreateIndex("dbo.ProgramTemplateModels", "goals_Id");
            AddForeignKey("dbo.ProgramTemplateModels", "goals_Id", "dbo.ProgramGoals", "Id");
            DropColumn("dbo.ProgramTemplateModels", "programGoal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProgramTemplateModels", "programGoal", c => c.String());
            DropForeignKey("dbo.ProgramTemplateModels", "goals_Id", "dbo.ProgramGoals");
            DropIndex("dbo.ProgramTemplateModels", new[] { "goals_Id" });
            DropColumn("dbo.ProgramTemplateModels", "goals_Id");
            DropTable("dbo.ProgramGoals");
            RenameTable(name: "dbo.ProgramTemplateModels", newName: "ProgramModels");
        }
    }
}
