namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCalendar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DateModels",
                c => new
                    {
                        DateKey = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Day = c.Int(nullable: false),
                        DaySuffix = c.String(nullable: false),
                        Weekday = c.Int(nullable: false),
                        WeekDayName = c.String(nullable: false),
                        IsWeekend = c.Boolean(nullable: false),
                        IsHoliday = c.Boolean(nullable: false),
                        HolidayText = c.String(),
                        DOWInMonth = c.Int(nullable: false),
                        DayOfYear = c.Int(nullable: false),
                        WeekOfMonth = c.Int(nullable: false),
                        WeekOfYear = c.Int(nullable: false),
                        ISOWeekOfYear = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        MonthName = c.String(nullable: false),
                        Quarter = c.Int(nullable: false),
                        QuarterName = c.String(nullable: false),
                        Year = c.Int(nullable: false),
                        MMYYYY = c.String(nullable: false),
                        MonthYear = c.String(nullable: false),
                        FirstDayOfMonth = c.DateTime(nullable: false),
                        LastDayOfMonth = c.DateTime(nullable: false),
                        FirstDayOfQuarter = c.DateTime(nullable: false),
                        LastDayOfQuarter = c.DateTime(nullable: false),
                        FirstDayOfYear = c.DateTime(nullable: false),
                        LastDayOfYear = c.DateTime(nullable: false),
                        FirstDayOfNextMonth = c.DateTime(nullable: false),
                        FirstDayOfNextYear = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DateKey);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DateModels");
        }
    }
}
