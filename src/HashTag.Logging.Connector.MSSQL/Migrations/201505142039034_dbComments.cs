namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.dbEvent", "SeverityCode", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.dbEvent", "PriorityCode", c => c.String(maxLength: 50, unicode: false));
            DropColumn("dbo.dbEvent", "Severity");
            DropColumn("dbo.dbEvent", "Priority");
        }
        
        public override void Down()
        {
            AddColumn("dbo.dbEvent", "Priority", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.dbEvent", "Severity", c => c.String(maxLength: 50, unicode: false));
            DropColumn("dbo.dbEvent", "PriorityCode");
            DropColumn("dbo.dbEvent", "SeverityCode");
        }
    }
}
