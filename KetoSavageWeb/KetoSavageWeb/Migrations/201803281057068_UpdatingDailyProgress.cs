namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingDailyProgress : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DailyProgressModel", "DateId");
            AddForeignKey("dbo.DailyProgressModel", "DateId", "dbo.DateModels", "DateKey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyProgressModel", "DateId", "dbo.DateModels");
            DropIndex("dbo.DailyProgressModel", new[] { "DateId" });
        }
    }
}
