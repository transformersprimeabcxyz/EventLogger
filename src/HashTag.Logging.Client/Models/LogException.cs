﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using HashTag.Collections;
using HashTag.Reflection;
using Newtonsoft.Json;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Serializable version of a .Net exception including all inner exceptions and public properties
    /// </summary>
    
    [Serializable]
    [JsonObject]
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; set; }

        

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }


        // https://msdn.microsoft.com/en-us/library/system.web.management.webeventcodes%28v=vs.110%29.aspx
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpWebEventCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int?  HttpStatusValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HttpStatusCode { get; set; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string HttpHtmlMessage { get; set; }

        public List<Property> Properties { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogException InnerException { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HelpLink { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

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

        public object Clone()
        {
            return (object)JsonConvert.DeserializeObject<LogException>(JsonConvert.SerializeObject(this));     
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        

    }
}