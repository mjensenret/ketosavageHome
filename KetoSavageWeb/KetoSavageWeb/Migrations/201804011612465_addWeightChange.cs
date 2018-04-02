namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWeightChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramTemplate", "WeightWeek", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramTemplate", "WeightWeek");
        }
    }
}
