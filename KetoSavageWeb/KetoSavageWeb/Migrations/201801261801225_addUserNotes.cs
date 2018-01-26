namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "notes");
        }
    }
}
