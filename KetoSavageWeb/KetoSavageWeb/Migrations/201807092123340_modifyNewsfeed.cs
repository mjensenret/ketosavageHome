namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyNewsfeed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewsModel", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsModel", "Type");
        }
    }
}
