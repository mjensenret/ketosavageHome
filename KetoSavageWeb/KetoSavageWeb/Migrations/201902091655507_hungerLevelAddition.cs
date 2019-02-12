namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hungerLevelAddition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HungerLevel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Value = c.Int(nullable: false),
                        Program_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramTemplate", t => t.Program_Id)
                .Index(t => t.Program_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HungerLevel", "Program_Id", "dbo.ProgramTemplate");
            DropIndex("dbo.HungerLevel", new[] { "Program_Id" });
            DropTable("dbo.HungerLevel");
        }
    }
}
