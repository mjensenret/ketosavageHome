namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameDescriptionColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramTemplate", "Description", c => c.String());
            DropColumn("dbo.ProgramTemplate", "programDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProgramTemplate", "programDescription", c => c.String());
            DropColumn("dbo.ProgramTemplate", "Description");
        }
    }
}
