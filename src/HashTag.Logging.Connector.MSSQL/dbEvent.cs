using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Connector.MSSQL
{
    public class dbEvent
    {

        [Required, Column(Order = 0), Display(Name = "Local date/time when this event was generated")]
        public DateTime EventDate { get; set; }

        [MaxLength(200), Column(TypeName = "varchar",Order=10), Display(Name = "Name of application that generated event.")]
        public string Application { get; set; }
        [MaxLength(50), Column(TypeName = "varchar", Order = 15), Display(Name = "One of the TraceEventType values: e.g. Error, Warning, Information, Verbose")]
        public string SeverityCode { get; set; }

        [MaxLength(70), Column(TypeName = "varchar", Order = 17), Display(Name = "Short description of event.  May often be first characters of Message")]
        public string Title { get; set; }

        [Column(TypeName = "varchar", Order = 20), Display(Name = "Sub-part of application (e.g. App:Accounting, Module: PaycheckPrinting)")]
        public string Module { get; set; }

        [MaxLength(200), Column(TypeName = "varchar", Order = 40), Display(Name = "Location of application (e.g. DEV, local, STAGE, HOTFIX, PROD1)")]
        public string Environment { get; set; }

        [MaxLength(50), Column(TypeName = "varchar", Order = 50), Display(Name = "Name of computer generating this event (e.g. WS020GTR1)")]
        public string HostName { get; set; }

        [MaxLength(50), Column(TypeName = "varchar", Order = 60), Display(Name = "User name in effect when event was generated (e.g. login name, service host, etc.)")]
        public string Identity { get; set; }


        [Required, MaxLength(200), Column(Order = 70, TypeName = "varchar"), Display(Name = "Name of logger submitting this event.  Some patterns allow for ganular filtering based on logger name")]
        public string Channel { get; set; }

        [Column(Order = 300), Display(Name = "Numerical identifier of this event type (e.g. 100=unauthorized, 2432-Error printing).  Often used in semantic logging scenarios")]                
        public Nullable<int> EventId { get; set; }

        [MaxLength(20), Column(TypeName = "varchar", Order = 400), Display(Name = "Textual identifier of this type of event (e.g. AP90033-Invalid account number)")]
        public string EventCode { get; set; }


        [Column(TypeName = "varchar", Order = 500), Display(Name = "Detailed information of the event.  (e.g. ex.ToString()")]
        public string Message { get; set; }


        [Column(Order = 600), Display(Name = "Numeric value of severity event generator considers this event. Most severe is lowest value.  See System.Diagnostics.TraceEventType")]
        public Nullable<int> SeverityValue { get; set; }

        [MaxLength(50), Column(TypeName = "varchar", Order = 700), Display(Name = "Textual value of priority event generator wanted to place on this event.")]
        public string PriorityCode { get; set; }

        [Column(Order = 800), Display(Name = "Numeric value of priority event generator wanted to place on this event. Most urgent priority is lowest value to match TraceEventType ordering")]
        public Nullable<int> PriorityValue { get; set; }

        [MaxLength(36), Column(TypeName = "varchar", Order = 920), Display(Name = "Identifier that groups several messages together (e.g. calls through application stack, calls across web farm machines, session)")]
        public string CorrelationUUID { get; set; }

        [Column(TypeName = "varchar(max)", Order = 930), Display(Name = "Detailed list of all exceptions, and inner exceptions")]
        public string Exceptions { get; set; }

        [Column(TypeName = "varchar(max)", Order = 940), Display(Name = "Extended key-value pairs to associate with this event")]
        public string Properties { get; set; }

        [Column(TypeName = "varchar", Order = 950), MaxLength(4000), Display(Name = "List of categories this message can belong to.  Often used in filtering and routing scenarios. (e.g patterned after EntLib Logging")]
        public string Categories { get; set; }

        [Column(TypeName = "varchar", Order = 960), MaxLength(4000), Display(Name = "Any text the event generator wants to associate with this event.  (e.g. record id, processor state)")]
        public string Reference { get; set; }

        [Column(TypeName = "varchar(max)", Order = 970), Display(Name = "If available, provides identitie(s) of user's when event was generated")]
        public string UserContext { get; set; }

        [Column(TypeName = "varchar(max)", Order = 980), Display(Name = "If available, provides technical machine information when event was generated")]
        public string MachineContext { get; set; }

        [Column(TypeName = "varchar(max)", Order = 990), Display(Name = "If available, provides information about the HTTP request when event was generated")]
        public string HttpContext { get; set; }

        [Column(TypeName = "varchar(max)", Order = 900), Display(Name = "LogMessage converted to JSON")]
        public string AllJson { get; set; }

        [Key, Required, StringLength(36), MinLength(36), MaxLength(36), Column(Order = 999, TypeName = "varchar"), Display(Name = "Unique identifier of this instance of an event in the event store")]
        public string UUID { get; set; }

    }
}
