namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewsFeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewsModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Headline = c.String(),
                        Author = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Expires = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NewsModel");
        }
    }
}
