namespace KetoSavageWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class measurementTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MeasurementDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        measurementHeaderId = c.Int(nullable: false),
                        measurementType = c.String(),
                        measurementValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MeasurementHeader", t => t.measurementHeaderId)
                .Index(t => t.measurementHeaderId);
            
            CreateTable(
                "dbo.MeasurementHeader",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateId = c.Int(nullable: false),
                        UserProgramId = c.Int(nullable: false),
                        MeasurementNotes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 50),
                        LastModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DateModels", t => t.DateId)
                .ForeignKey("dbo.UserPrograms", t => t.UserProgramId)
                .Index(t => t.DateId)
                .Index(t => t.UserProgramId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MeasurementDetails", "measurementHeaderId", "dbo.MeasurementHeader");
            DropForeignKey("dbo.MeasurementHeader", "UserProgramId", "dbo.UserPrograms");
            DropForeignKey("dbo.MeasurementHeader", "DateId", "dbo.DateModels");
            DropIndex("dbo.MeasurementHeader", new[] { "UserProgramId" });
            DropIndex("dbo.MeasurementHeader", new[] { "DateId" });
            DropIndex("dbo.MeasurementDetails", new[] { "measurementHeaderId" });
            DropTable("dbo.MeasurementHeader");
            DropTable("dbo.MeasurementDetails");
        }
    }
}
