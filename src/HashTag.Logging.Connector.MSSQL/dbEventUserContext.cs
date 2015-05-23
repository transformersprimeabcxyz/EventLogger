using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Connector.MSSQL
{
    [ComplexType]
    public class dbEventUserContext
    {
        [Column(TypeName="varchar", Order = 360), MaxLength(100), Display(Name = "Identity name on the thread")]
        public string ThreadPrincipal {get;set;}

        [Column(TypeName = "varchar", Order = 370), MaxLength(100), Display(Name = "Http User name")]
        public string HttpUser {get;set;}

        [Column(TypeName = "varchar", Order = 380), MaxLength(100), Display(Name = "Environment.UserName")]
        public string EnvUserName {get;set;}

        [Column(TypeName = "varchar", Order = 390), MaxLength(100), Display(Name = "Environment.UserDomain")]
        public string UserDomain {get;set;}

        [Column(TypeName = "varchar", Order = 400), MaxLength(100), Display(Name = "Win32 App Domain Name")]
        public string AppDomainIdentity {get;set;}

        [Column(Order = 410), Display(Name = "Machine.IsUserInteractive")]
        public bool? IsInteractive {get;set;}

        [Column(TypeName = "varchar", Order = 420), MaxLength(100), Display(Name = "Identity to report as 'Event User'.  One of the  values")]
        public string DefaultUser { get; set; }
    }
}
