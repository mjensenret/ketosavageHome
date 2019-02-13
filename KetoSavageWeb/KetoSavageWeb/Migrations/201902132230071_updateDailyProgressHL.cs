namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDailyProgressHL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyProgress", "HungerLevelId", c => c.Int());
            DropColumn("dbo.DailyProgress", "HungerLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DailyProgress", "HungerLevel", c => c.String());
            DropColumn("dbo.DailyProgress", "HungerLevelId");
        }
    }
}
