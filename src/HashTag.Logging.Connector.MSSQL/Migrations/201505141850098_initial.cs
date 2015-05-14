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
                        Application = c.String(maxLength: 200, unicode: false),
                        Severity = c.String(maxLength: 50, unicode: false),
                        Title = c.String(maxLength: 70, unicode: false),
                        Module = c.String(maxLength: 8000, unicode: false),
                        Environment = c.String(maxLength: 200, unicode: false),
                        HostName = c.String(maxLength: 50, unicode: false),
                        Identity = c.String(maxLength: 50, unicode: false),
                        Channel = c.String(nullable: false, maxLength: 200, unicode: false),
                        EventId = c.Int(),
                        EventCode = c.String(maxLength: 20, unicode: false),
                        Message = c.String(maxLength: 8000, unicode: false),
                        SeverityValue = c.Int(),
                        Priority = c.String(maxLength: 50, unicode: false),
                        PriorityValue = c.Int(),
                        AllJson = c.String(unicode: false),
                        CorrelationUUID = c.String(maxLength: 36, unicode: false),
                        Exceptions = c.String(unicode: false),
                        Properties = c.String(unicode: false),
                        Categories = c.String(maxLength: 4000, unicode: false),
                        Reference = c.String(maxLength: 4000, unicode: false),
                        UserContext = c.String(unicode: false),
                        MachineContext = c.String(unicode: false),
                        HttpContext = c.String(unicode: false),
                        UUID = c.String(nullable: false, maxLength: 36, unicode: false),
                    })
                .PrimaryKey(t => t.UUID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.dbEvent");
        }
    }
}
