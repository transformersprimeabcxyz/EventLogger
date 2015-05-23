namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dbEvent",
                c => new
                    {
                        EventDate = c.DateTime(nullable: false),
                        SeverityValue = c.Int(),
                        SeverityCode = c.String(maxLength: 50, unicode: false),
                        Environment = c.String(maxLength: 100, unicode: false),
                        Application = c.String(nullable: false, maxLength: 100, unicode: false),
                        Message = c.String(nullable: false, maxLength: 8000, unicode: false),
                        HostName = c.String(maxLength: 100, unicode: false),
                        UserIdentity = c.String(maxLength: 100, unicode: false),
                        ExceptionMessage = c.String(maxLength: 8000, unicode: false),
                        ExceptionType = c.String(maxLength: 200, unicode: false),
                        ExceptionSource = c.String(maxLength: 200, unicode: false),
                        ExceptionBaseMessage = c.String(maxLength: 8000, unicode: false),
                        ExceptionBaseType = c.String(maxLength: 200, unicode: false),
                        ExceptionBaseSource = c.String(maxLength: 200, unicode: false),
                        Reference = c.String(maxLength: 100, unicode: false),
                        UUID = c.String(nullable: false, maxLength: 36, unicode: false),
                        Channel = c.String(maxLength: 200, unicode: false),
                        Module = c.String(maxLength: 100, unicode: false),
                        PriorityCode = c.String(maxLength: 50, unicode: false),
                        PriorityValue = c.Int(),
                        HttpContext_Server = c.String(maxLength: 8000, unicode: false),
                        HttpContext_Query = c.String(maxLength: 8000, unicode: false),
                        HttpContext_Cookies = c.String(maxLength: 8000, unicode: false),
                        HttpContext_Form = c.String(maxLength: 8000, unicode: false),
                        HttpContext_Header = c.String(maxLength: 8000, unicode: false),
                        HttpContext_WebEventValue = c.Int(),
                        HttpContext_StatusValue = c.Int(),
                        HttpContext_StatusCode = c.String(maxLength: 50, unicode: false),
                        HttpContext_HtmlMessage = c.String(maxLength: 8000, unicode: false),
                        MachineContext = c.String(maxLength: 8000, unicode: false),
                        EventId = c.Int(),
                        EventCode = c.String(maxLength: 20, unicode: false),
                        CorrelationUUID = c.String(maxLength: 36, unicode: false),
                        Exceptions = c.String(maxLength: 8000, unicode: false),
                        Properties = c.String(maxLength: 8000, unicode: false),
                        Categories = c.String(maxLength: 8000, unicode: false),
                        UserContext_ThreadPrincipal = c.String(maxLength: 100, unicode: false),
                        UserContext_HttpUser = c.String(maxLength: 100, unicode: false),
                        UserContext_EnvUserName = c.String(maxLength: 100, unicode: false),
                        UserContext_UserDomain = c.String(maxLength: 100, unicode: false),
                        UserContext_AppDomainIdentity = c.String(maxLength: 100, unicode: false),
                        UserContext_IsInteractive = c.Boolean(),
                        UserContext_DefaultUser = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.UUID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.dbEvent");
        }
    }
}
