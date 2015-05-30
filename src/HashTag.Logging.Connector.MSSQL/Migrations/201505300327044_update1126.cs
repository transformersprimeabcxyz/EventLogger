namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1126 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.dbEvent", "EventSource", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.dbEvent", "EventSource", c => c.String(nullable: false, maxLength: 100, unicode: false));
        }
    }
}
