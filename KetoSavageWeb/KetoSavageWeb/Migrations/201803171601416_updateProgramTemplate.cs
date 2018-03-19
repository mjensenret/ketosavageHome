namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProgramTemplate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramTemplate", new[] { "goals_Id" });
            CreateIndex("dbo.ProgramTemplate", "Goals_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramTemplate", new[] { "Goals_Id" });
            CreateIndex("dbo.ProgramTemplate", "goals_Id");
        }
    }
}
