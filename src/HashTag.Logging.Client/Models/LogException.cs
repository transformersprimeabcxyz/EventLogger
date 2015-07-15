using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using HashTag.Collections;
using HashTag.Reflection;
using Newtonsoft.Json;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Serializable version of a .Net exception including all inner exceptions and public properties.
    /// Uses reflection to pull out exception specific properties
    /// </summary>    
    [Serializable]
    [JsonObject]
    public class LogException : ICloneable
    {
        static string[] _filterList = Reflector.GetPublicPropertyNames(typeof(Exception));

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogException()
        {
            Properties = new List<Property>();
            Data = new List<Property>();
            ExceptionId = Guid.NewGuid();
        }

        /// <summary>
        /// Create object based on .Net exception
        /// </summary>
        /// <param name="ex">Hydrated .Net exception</param>
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

            if (ex.TargetSite != null)
            {
                Method = ex.TargetSite.ToString();
                Module = ex.TargetSite.DeclaringType.Module.Name;
                Class = ex.TargetSite.DeclaringType.FullName;
            }

            if (ex is HttpException)
            {
                var webEx = ex as HttpException;
                ErrorCode = webEx.ErrorCode.ToString();
                HttpHtmlMessage = webEx.GetHtmlErrorMessage();
                HttpWebEventCode = webEx.WebEventCode;
                HttpStatusValue = webEx.GetHttpCode();
                HttpStatusCode = ((HttpStatusCode)HttpStatusValue).ToString();
            }
            else
            {
                ErrorCode = Reflector.GetProtectedProperty<int>("HResult", ex, default(int)).ToString();
            }


        }


        public Guid ExceptionId { get; set; }

        /// <summary>
        /// Name of module (often .dll)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; set; }

        /// <summary>
        /// Class where exception occured
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }

        
        
        /// <summary>
        /// Http Specific.  https://msdn.microsoft.com/en-us/library/system.web.management.webeventcodes%28v=vs.110%29.aspx
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpWebEventCode { get; set; }

        /// <summary>
        /// Http Specific. 404, 200, 310, etc.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int?  HttpStatusValue { get; set; }

        /// <summary>
        /// Http Specific. Textual represenation of status (Ok, BadMessage, Accepted)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HttpStatusCode { get; set; }
        
        /// <summary>
        /// Http Specific.  Any html message returned from server
        /// </summary>
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string HttpHtmlMessage { get; set; }

        /// <summary>
        /// Properties of .Net exception extracted using reflection
        /// </summary>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Inner exception (if any)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogException InnerException { get; set; }


        /// <summary>
        /// Exception.Message
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Exception.Source
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        /// <summary>
        /// Exception.StackTract
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }

        /// <summary>
        /// HelpLink
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HelpLink { get; set; }

        /// <summary>
        /// Method that was executing when error occured
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        /// <summary>
        /// Data representation of Exception.Data property.  using Data[x].ToString() on Data values
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Property> Data { get; set; }

        /// <summary>
        ///  A coded value that is assigned to a specific exception. Often the HRESULT from the attached exception 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// .Net data type of exception
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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

        /// <summary>
        /// Creates a copy of this message
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return (object)JsonConvert.DeserializeObject<LogException>(JsonConvert.SerializeObject(this));     
        }

        /// <summary>
        /// Returns a human readable version of this exception
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        

    }
}
