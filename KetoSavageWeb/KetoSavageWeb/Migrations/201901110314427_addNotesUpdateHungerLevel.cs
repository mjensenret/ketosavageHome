namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNotesUpdateHungerLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyProgress", "Notes", c => c.String());
            AlterColumn("dbo.DailyProgress", "HungerLevel", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DailyProgress", "HungerLevel", c => c.Int());
            DropColumn("dbo.DailyProgress", "Notes");
        }
    }
}
