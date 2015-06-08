using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Connector.MSSQL
{
    [Table("dbEventProperty")]
    
    public class dbEventProperty
    {
        public dbEventProperty()
        {
            UUID = Guid.NewGuid();
        }
        [Key]
        [Column(Order=10)]
        public Guid UUID { get; set; }

        [Column(Order = 20)]
        [Index("IDX_dbProperty__Event_Name",1)]
        public Guid EventUUID { get; set; }

        [Column(Order = 25,TypeName="varchar")]
        [Index("IDX_dbProperty__Group_Name", 1)]
        [MaxLength(20)]
        public string Group { get; set; }

        [Index("IDX_dbProperty__Event_Name",2)]
        [Index("IDX_dbProperty__Name", 1)]
        [Index("IDX_dbProperty__Group_Name", 2)]
        [MaxLength(30), Column(TypeName = "varchar", Order = 30), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar", Order = 40), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Value { get; set; }

        [ForeignKey("EventUUID")]
        [JsonIgnore] 
        public virtual dbEvent Event { get; set; }
    }

    [Table("dbEvent")]
    public class dbEvent
    {
        public dbEvent()
        {
            UUID = Guid.NewGuid();
            Properties = new List<dbEventProperty>();
        }

        [Key, Required, Column(Order = 150), Display(Name = "Unique identifier of this instance of an event in the event store")]
        public Guid UUID { get; set; }

        [Required]
        [Column(Order=100, TypeName="varchar")]
        public string Message { get; set; }

        [Required, Column(Order = 0), Display(Name = "Local date/time when this event was generated")]
        [Index("IDX_dbEvent__EventDate",1)]
        public DateTime EventDate { get; set; }


        [Required, MaxLength(100), Column(TypeName = "varchar", Order = 40), Display(Name = "Name of application that generated event.")]
        [Index("IDX_dbEvent__Application", 1)]
        public string Application { get; set; }

        [MaxLength(100), Column(TypeName = "varchar", Order = 60), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Host { get; set; }


        [Column("EventTypeId", Order = 20), Display(Name = "One of the TraceEventType values: e.g. Error, Warning, Information, Verbose")]
        [Index("IDX_dbEvent__EventType", 1)]
        public TraceEventType? EventType { get; set; }

        
        [MaxLength(20), Required, Column(TypeName = "varchar", Order = 30)]
        public string EventTypeName
        {
            get
            {
                if (EventType == null) return null;

                return EventType.Value.ToString();
            }
            set
            {
                if (value == null)
                {
                    EventType = null;
                }
                EventType = (TraceEventType) Enum.Parse(typeof(TraceEventType), value);
            }
        }
        
        [MaxLength(100), Column(TypeName = "varchar", Order = 70), Display(Name = "User name in effect when event was generated (e.g. login name, service host, etc.)")]
        public string User { get; set; }

        [MaxLength(100), Column(TypeName = "varchar", Order = 80)]
        public string EventSource { get; set; }

        public virtual List<dbEventProperty> Properties { get; set; }

    }
}
