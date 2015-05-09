using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HashTag.Web.Http
{
    public class RestResponseBodyNotAvailableException:Exception
    {
        public RestResponseBodyNotAvailableException()
            : base()
        {
        }
        public RestResponseBodyNotAvailableException(string session, string message, params object[] args):base(string.Format(message,args))
        {
            Session = session;
        }
        public string Session
        {
            get
            {
                return (string)base.Data["session"];
            }
            set
            {
                base.Data["session"] =value;
            }
        }
        public RestResponseBodyNotAvailableException(string message, params object[] args):base(string.Format(message,args))
        {
        }
        public RestResponseBodyNotAvailableException(Exception innerException, string message, params object[] args):base(string.Format(message,args),innerException)
        {
        }
        public RestResponseBodyNotAvailableException(SerializationInfo info, StreamingContext context):base(info,context)
        {
        }

    }
}
