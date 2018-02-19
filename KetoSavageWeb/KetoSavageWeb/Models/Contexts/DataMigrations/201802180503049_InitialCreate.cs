namespace KetoSavageWeb.Models.Contexts.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgramModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        programGoal = c.String(),
                        programNotes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CoachedPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        renewalDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramModels", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.SelfGuidedPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramModels", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SelfGuidedPrograms", "Id", "dbo.ProgramModels");
            DropForeignKey("dbo.CoachedPrograms", "Id", "dbo.ProgramModels");
            DropIndex("dbo.SelfGuidedPrograms", new[] { "Id" });
            DropIndex("dbo.CoachedPrograms", new[] { "Id" });
            DropTable("dbo.SelfGuidedPrograms");
            DropTable("dbo.CoachedPrograms");
            DropTable("dbo.ProgramModels");
        }
    }
}
