namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateHungerLevelTable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.HungerLevel", new[] { "Program_Id" });
            RenameColumn(table: "dbo.HungerLevel", name: "Program_Id", newName: "programId");
            AlterColumn("dbo.HungerLevel", "programId", c => c.Int(nullable: false));
            CreateIndex("dbo.HungerLevel", "programId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.HungerLevel", new[] { "programId" });
            AlterColumn("dbo.HungerLevel", "programId", c => c.Int());
            RenameColumn(table: "dbo.HungerLevel", name: "programId", newName: "Program_Id");
            CreateIndex("dbo.HungerLevel", "Program_Id");
        }
    }
}
