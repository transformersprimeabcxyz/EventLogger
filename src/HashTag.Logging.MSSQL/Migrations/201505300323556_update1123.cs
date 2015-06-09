namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1123 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.dbEvent", name: "EventType", newName: "EventTypeId");
            AddColumn("dbo.dbEvent", "EventTypeName", c => c.String(nullable: false, maxLength: 20, unicode: false));
            DropColumn("dbo.dbEvent", "EventName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.dbEvent", "EventName", c => c.String(nullable: false, maxLength: 20, unicode: false));
            DropColumn("dbo.dbEvent", "EventTypeName");
            RenameColumn(table: "dbo.dbEvent", name: "EventTypeId", newName: "EventType");
        }
    }
}
