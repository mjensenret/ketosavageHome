namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class switchToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DailyProgress", "PlannedWeight", c => c.Double());
            AlterColumn("dbo.DailyProgress", "ActualWeight", c => c.Double());
            AlterColumn("dbo.DailyProgress", "PlannedFat", c => c.Double());
            AlterColumn("dbo.DailyProgress", "ActualFat", c => c.Double());
            AlterColumn("dbo.DailyProgress", "PlannedProtein", c => c.Double());
            AlterColumn("dbo.DailyProgress", "ActualProtein", c => c.Double());
            AlterColumn("dbo.DailyProgress", "PlannedCarbohydrate", c => c.Double());
            AlterColumn("dbo.DailyProgress", "ActualCarbohydrate", c => c.Double());
            AlterColumn("dbo.UserPrograms", "StartWeight", c => c.Double(nullable: false));
            AlterColumn("dbo.UserPrograms", "GoalWeight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserPrograms", "GoalWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.UserPrograms", "StartWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "ActualCarbohydrate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "PlannedCarbohydrate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "ActualProtein", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "PlannedProtein", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "ActualFat", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "PlannedFat", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "ActualWeight", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyProgress", "PlannedWeight", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
