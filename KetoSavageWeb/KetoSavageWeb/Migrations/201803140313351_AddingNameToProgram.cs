namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingNameToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramTemplate", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramTemplate", "Name");
        }
    }
}
