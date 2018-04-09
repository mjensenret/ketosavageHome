namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefeedBool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyProgress", "IsRefeed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyProgress", "IsRefeed");
        }
    }
}
