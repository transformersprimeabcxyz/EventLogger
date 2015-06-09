namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dbEvent",
                c => new
                    {
                        EventDate = c.DateTime(nullable: false),
                        EventType = c.Int(),
                        EventName = c.String(nullable: false, maxLength: 20, unicode: false),
                        Application = c.String(nullable: false, maxLength: 100, unicode: false),
                        Host = c.String(maxLength: 100, unicode: false),
                        User = c.String(maxLength: 100, unicode: false),
                        EventSource = c.String(nullable: false, maxLength: 100, unicode: false),
                        Message = c.String(nullable: false),
                        UUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.UUID)
                .Index(t => t.EventDate, name: "IDX_dbEvent__EventDate")
                .Index(t => t.EventType, name: "IDX_dbEvent__EventType")
                .Index(t => t.Application, name: "IDX_dbEvent__Application");
            
            CreateTable(
                "dbo.dbEventProperty",
                c => new
                    {
                        UUID = c.Guid(nullable: false),
                        EventUUID = c.Guid(nullable: false),
                        Name = c.String(maxLength: 200, unicode: false),
                        Value = c.String(maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.UUID)
                .ForeignKey("dbo.dbEvent", t => t.EventUUID, cascadeDelete: true)
                .Index(t => new { t.EventUUID, t.Name }, name: "IDX_dbProperty__Event_Name")
                .Index(t => t.Name, name: "IDX_dbProperty__Name");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dbEventProperty", "EventUUID", "dbo.dbEvent");
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Name");
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Event_Name");
            DropIndex("dbo.dbEvent", "IDX_dbEvent__Application");
            DropIndex("dbo.dbEvent", "IDX_dbEvent__EventType");
            DropIndex("dbo.dbEvent", "IDX_dbEvent__EventDate");
            DropTable("dbo.dbEventProperty");
            DropTable("dbo.dbEvent");
        }
    }
}
