namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDailyProgress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyProgressModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateId = c.Int(nullable: false),
                        PlannedWeight = c.Decimal(precision: 18, scale: 2),
                        ActualWeight = c.Decimal(precision: 18, scale: 2),
                        PlannedFat = c.Decimal(precision: 18, scale: 2),
                        ActualFat = c.Decimal(precision: 18, scale: 2),
                        PlannedProtein = c.Decimal(precision: 18, scale: 2),
                        ActualProtein = c.Decimal(precision: 18, scale: 2),
                        PlannedCarbohydrate = c.Decimal(precision: 18, scale: 2),
                        ActualCarbohydrate = c.Decimal(precision: 18, scale: 2),
                        UserProgramId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 50),
                        LastModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserPrograms", t => t.UserProgramId)
                .Index(t => t.UserProgramId);
            
            AddColumn("dbo.UserPrograms", "StartWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.UserPrograms", "GoalWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyProgressModel", "UserProgramId", "dbo.UserPrograms");
            DropIndex("dbo.DailyProgressModel", new[] { "UserProgramId" });
            DropColumn("dbo.UserPrograms", "GoalWeight");
            DropColumn("dbo.UserPrograms", "StartWeight");
            DropTable("dbo.DailyProgressModel");
        }
    }
}
