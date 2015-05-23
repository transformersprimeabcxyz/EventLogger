#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Serializable version of a .Net exception including all inner exceptions and public properties
    /// </summary>
    [Serializable]
    public partial class ApiException : ICloneable
    {
        static string[] _filterList = getPublicPropertyNames(typeof(Exception));

        public ApiException()
        {
            Properties = new List<ApiProperty>();
            Data = new List<ApiProperty>();
        }

        public ApiException(Exception ex)
            : this()
        {
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
            HelpLink = ex.HelpLink;
            ExceptionType = ex.GetType().FullName;
            if (ex.InnerException != null)
            {
                InnerException = new ApiException(ex.InnerException );
            }

            foreach (object key in ex.Data.Keys)
            {
                string keyString = key.ToString();
                Data.Add(new ApiProperty(keyString, ex.Data[key]));
            }

            //-------------------------------------------------------
            // use reflection to get all public properties on the
            //	exception being examined except those defined in 
            //	_filterList (generally just those in base Exception class)
            //-------------------------------------------------------			
            this.Properties = getPublicProperties(ex, _filterList);
            
            TargetSite = (ex.TargetSite == null)?"(null)":ex.TargetSite.ToString();
            ErrorCode = getProtectedProperty<int>("HResult", ex, default(int)).ToString();
        }


        public List<ApiProperty> Properties { get; set; }

       
        public ApiException InnerException { get; set; }

       
        public string Message { get; set; }

       
        public string Source { get; set; }

       
        public string StackTrace { get; set; }

       
        public string HelpLink { get; set; }

       
        public string TargetSite { get; set; }


        public List<ApiProperty> Data { get; set; }

        /// <summary>
        ///  A coded value that is assigned to a specific exception. Often the HRESULT from the attached exception 
        /// </summary>
       
        public string ErrorCode { get; set; }

        /// <summary>
        /// .Net data type of exception
        /// </summary>
       
        public string ExceptionType { get; set; }

        /// <summary>
        /// Get's the innermost exception or a reference to this instance if there are no inner exceptions
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public ApiException BaseException
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
            var retEx = new ApiException()
            {
                ErrorCode = this.ErrorCode,
                ExceptionType = this.ExceptionType,
                HelpLink = this.HelpLink,
                Message = this.Message,
                Source = this.Source,
                StackTrace = this.StackTrace,
                TargetSite = this.TargetSite
            };
            if (InnerException != null)
            {
                retEx.InnerException = (ApiException)InnerException.Clone();
            }
            if (Data != null && Data.Count > 0)
            {
                foreach (var item in Data)
                {
                    retEx.Data.Add(item.Clone() as ApiProperty);
                }
            }

            if (Properties != null && Properties.Count > 0)
            {
                foreach (var prop in Properties)
                {
                    retEx.Properties.Add(prop.Clone() as ApiProperty);
                }
            }
            return retEx;
        }
        
        public override string ToString()
        {
            return ApiException.expand(this);
        }
        public string ToString(int initialOffset)
        {
            return ApiException.expand(this, initialOffset);
        }
    }
}
