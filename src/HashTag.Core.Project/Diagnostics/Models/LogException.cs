using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using HashTag.Reflection;
using System.Collections;
using HashTag.Collections;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Net;
using Newtonsoft.Json;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Serializable version of a .Net exception including all inner exceptions and public properties
    /// </summary>
    [DataContract(Namespace = CoreConfig.WcfNamespace)]
    [Serializable]
    public class LogException : ICloneable
    {
        static string[] _filterList = Reflector.GetPublicPropertyNames(typeof(Exception));

        public LogException()
        {
            Properties = new List<Property>();
            Data = new List<Property>();
            ExceptionId = Guid.NewGuid();
        }

        public LogException(Exception ex)
            : this()
        {
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
            HelpLink = ex.HelpLink;
            ExceptionType = ex.GetType().FullName;
            if (ex.InnerException != null)
            {
                InnerException = new LogException(ex.InnerException);
            }

            foreach (object key in ex.Data.Keys)
            {
                string keyString = key.ToString();
                Data.Add(new Property(keyString, ex.Data[key]));
            }

            //-------------------------------------------------------
            // use reflection to get all public properties on the
            //	exception being examined except those defined in 
            //	_filterList (generally just those in base Exception class)
            //-------------------------------------------------------			
            this.Properties = Reflector.GetPublicProperties(ex, _filterList);

            TargetSite = (ex.TargetSite == null) ? "(null)" : ex.TargetSite.ToString();

            if (ex is HttpException)
            {
                var webEx = ex as HttpException;
                ErrorCode = webEx.ErrorCode.ToString();
                HttpHtmlMessage = webEx.GetHtmlErrorMessage();
                HttpWebEventCode = webEx.WebEventCode;
                HttpStatusValue = webEx.GetHttpCode();
                HttpStatusCode = ((System.Net.HttpStatusCode)HttpStatusValue).ToString();
            }
            else
            {
                ErrorCode = Reflector.GetProtectedProperty<int>("HResult", ex, default(int)).ToString();
            }


        }


        [Key, DataMember]
        public Guid ExceptionId { get; set; }

        [DataMember, Citation("https://msdn.microsoft.com/en-us/library/system.web.management.webeventcodes%28v=vs.110%29.aspx")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpWebEventCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int?  HttpStatusValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HttpStatusCode { get; set; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string HttpHtmlMessage { get; set; }

        /// <summary>
        /// All public properties on the exception being examined expect well known ones from Exception
        /// </summary>
        [DataMember]
        public List<Property> Properties { get; set; }

        [DataMember]
        public LogException InnerException { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string HelpLink { get; set; }

        [DataMember]
        public string TargetSite { get; set; }

        [DataMember]
        public List<Property> Data { get; set; }

        /// <summary>
        ///  A coded value that is assigned to a specific exception. Often the HRESULT from the attached exception 
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// .Net data type of exception
        /// </summary>
        [DataMember]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Get's the innermost exception or a reference to this instance if there are no inner exceptions
        /// </summary>
        /// <returns></returns>
        public LogException BaseException
        {
            get
            {
                var retEx = this;
                while (retEx.InnerException != null)
                {
                    retEx = retEx.InnerException;
                }
                return retEx;
            }
        }

        public object Clone()
        {
            return (object)JsonConvert.DeserializeObject<LogMessage>(JsonConvert.SerializeObject(this));     
        }

        public override string ToString()
        {
            return this.Expand();
        }
        public string ToString(int initialOffset)
        {
            return this.Expand(initialOffset);
        }

    }
}
