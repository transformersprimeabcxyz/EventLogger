namespace HashTag.Logging.Connector.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1121 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.dbEvent", "Message", c => c.String(nullable: false, maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.dbEvent", "Message", c => c.String(nullable: false));
        }
    }
}
