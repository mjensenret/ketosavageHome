namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingDailyProgress_2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DailyProgressModel", newName: "DailyProgress");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DailyProgress", newName: "DailyProgressModel");
        }
    }
}
