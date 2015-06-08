using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Connector.MSSQL
{
    public class EventContext : DbContext
    {
        public EventContext()
            : base("dbLog")
        {

        }

        public virtual DbSet<dbEvent> Events { get; set; }
        public virtual DbSet<dbEventProperty> Properties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
