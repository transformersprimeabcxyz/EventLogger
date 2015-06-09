namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupField : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Event_Name");
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Name");
            AddColumn("dbo.dbEventProperty", "Group", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("dbo.dbEventProperty", "Name", c => c.String(maxLength: 30, unicode: false));
            CreateIndex("dbo.dbEventProperty", new[] { "EventUUID", "Name" }, name: "IDX_dbProperty__Event_Name");
            CreateIndex("dbo.dbEventProperty", new[] { "Group", "Name" }, name: "IDX_dbProperty__Group_Name");
            CreateIndex("dbo.dbEventProperty", "Name", name: "IDX_dbProperty__Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Name");
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Group_Name");
            DropIndex("dbo.dbEventProperty", "IDX_dbProperty__Event_Name");
            AlterColumn("dbo.dbEventProperty", "Name", c => c.String(maxLength: 200, unicode: false));
            DropColumn("dbo.dbEventProperty", "Group");
            CreateIndex("dbo.dbEventProperty", "Name", name: "IDX_dbProperty__Name");
            CreateIndex("dbo.dbEventProperty", new[] { "EventUUID", "Name" }, name: "IDX_dbProperty__Event_Name");
        }
    }
}
