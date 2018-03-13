namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameProgramTemplate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProgramTemplateModels", newName: "ProgramTemplate");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ProgramTemplate", newName: "ProgramTemplateModels");
        }
    }
}
