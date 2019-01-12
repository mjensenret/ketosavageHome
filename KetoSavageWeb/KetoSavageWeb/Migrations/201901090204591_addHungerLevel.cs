namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHungerLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyProgress", "HungerLevel", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyProgress", "HungerLevel");
        }
    }
}
