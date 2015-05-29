using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Connector.MSSQL
{
    public class dbProperty
    {
        [Key]
        public Guid UUID { get; set; }

        [Index("IDX_dbProperty_Event_Name",1)]
        public Guid EventUUID { get; set; }

        [Index("IDX_dbProperty_Event_Name", 2)]
        [MaxLength(200), Column(TypeName = "varchar", Order = 60), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar", Order = 60), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Value { get; set; }

          [ForeignKey("EventUUID")] 
        public virtual dbEvent Event { get; set; } 
    }

    public class dbEvent
    {
        public dbEvent()
        {
            UUID = Guid.NewGuid();
            Properties = new List<dbProperty>();
        }

        [Key, Required, Column(Order = 150, TypeName = "varchar"), Display(Name = "Unique identifier of this instance of an event in the event store")]
        public Guid UUID { get; set; }

        public string Message { get; set; }

        [Required, Column(Order = 0), Display(Name = "Local date/time when this event was generated")]
        public DateTime EventDate { get; set; }

        [Required,MaxLength(100), Column(TypeName = "varchar", Order = 40), Display(Name = "Name of application that generated event.")]
        public string Application { get; set; }

        [MaxLength(100), Column(TypeName = "varchar", Order = 60), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string Host { get; set; }

        [MaxLength(50), Column(TypeName = "varchar", Order = 20), Display(Name = "One of the TraceEventType values: e.g. Error, Warning, Information, Verbose")]
        public string Severity { get; set; }

        [MaxLength(100), Column(TypeName = "varchar", Order = 70), Display(Name = "User name in effect when event was generated (e.g. login name, service host, etc.)")]
        public string User { get; set; }

        public virtual List<dbProperty> Properties { get; set; }
        //public DateTime EventDate { get; set; }

        //[Column(Order = 10), Display(Name = "Numeric value of severity event generator considers this event. Most severe is lowest value.  See System.Diagnostics.TraceEventType")]
        //public Nullable<int> SeverityValue { get; set; }

        
        //public string SeverityCode { get; set; }

        //[MaxLength(100), Column(TypeName = "varchar", Order = 30), Display(Name = "Location of application (e.g. DEV, local, STAGE, HOTFIX, PROD1)")]
        //public string Environment { get; set; }

        
        //public string Application { get; set; }

        //[Required,Column(TypeName = "varchar", Order = 50),Display(Name = "Detailed information of the event.  (e.g. ex.ToString()")]
        //public string Message { get; set; }

        //public string HostName { get; set; }

    

        //[Column(TypeName = "varchar", Order = 80),  Display(Name = "Message of root exception")]
        //public string ExceptionMessage { get; set; }

        //[Column(TypeName = "varchar", Order = 90), MaxLength(200), Display(Name = ".Net exception CLR type of root exception")]
        //public string ExceptionType { get; set; }

        //[Column(TypeName = "varchar", Order = 100), MaxLength(200), Display(Name = "Source (e.g. method) of root exception")]
        //public string ExceptionSource { get; set; }

        //[Column(TypeName = "varchar", Order = 110), Display(Name = "Message of innermost exception")]
        //public string ExceptionBaseMessage { get; set; }

        //[Column(TypeName = "varchar", Order = 120), MaxLength(200), Display(Name = ".Net exception CLR type of innermost exception")]
        //public string ExceptionBaseType { get; set; }

        //[Column(TypeName = "varchar", Order = 130), MaxLength(200), Display(Name = "Source (e.g. method) of innermost exception")]
        //public string ExceptionBaseSource { get; set; }
        
        //[Column(TypeName="varchar",Order=140),MaxLength(100), Display(Name = "Source (e.g. method) of innermost exception")]
        //public string Reference { get; set; }

        //public string UUID { get; set; }

        
        //public string Channel { get; set; }

        //[Column(TypeName = "varchar", Order = 170),MaxLength(100), Display(Name = "Sub-part of application (e.g. App:Accounting, Module: PaycheckPrinting)")]
        //public string Module { get; set; }

        //[MaxLength(50), Column(TypeName = "varchar", Order = 180), Display(Name = "Textual value of priority event generator wanted to place on this event.")]
        //public string PriorityCode { get; set; }

        //[Column(Order = 190), Display(Name = "Numeric value of priority event generator wanted to place on this event. Most urgent priority is lowest value to match TraceEventType ordering")]
        //public Nullable<int> PriorityValue { get; set; }

        //public dbEventHttpContext HttpContext {get;set;}

        //[Column(TypeName = "varchar", Order = 290), Display(Name = "If available, provides technical machine information when event was generated")]
        //public string MachineContext { get; set; }

        //[Column(Order = 300), Display(Name = "Numerical identifier of this event type (e.g. 100=unauthorized, 2432-Error printing).  Often used in semantic logging scenarios")]
        //public Nullable<int> EventId { get; set; }

        //[MaxLength(20), Column(TypeName = "varchar", Order = 310), Display(Name = "Textual identifier of this type of event (e.g. AP90033-Invalid account number)")]
        //public string EventCode { get; set; }
        
        //[MaxLength(36), Column(TypeName = "varchar", Order = 320), Display(Name = "Identifier that groups several messages together (e.g. calls through application stack, calls across web farm machines, session)")]
        //public string CorrelationUUID { get; set; }

        //[Column(TypeName = "varchar", Order = 330), Display(Name = "Detailed list of all exceptions, and inner exceptions")]
        //public string Exceptions { get; set; }

        //[Column(TypeName = "varchar", Order = 340), Display(Name = "Extended key-value pairs to associate with this event")]
        //public string Properties { get; set; }

        //[Column(TypeName = "varchar", Order = 350),  Display(Name = "List of categories this message can belong to.  Often used in filtering and routing scenarios. (e.g patterned after EntLib Logging")]
        //public string Categories { get; set; }
        
        //public dbEventUserContext UserContext { get; set; }
        
    }
}
