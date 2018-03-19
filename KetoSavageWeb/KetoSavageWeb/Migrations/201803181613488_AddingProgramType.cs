namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingProgramType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPrograms", "ProgramType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserPrograms", "ProgramType");
        }
    }
}
