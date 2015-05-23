#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Provides common fields for API requests.  Inherit from this class to build custom models
    /// </summary>
    public class ApiRequestBase
    {
        /// <summary>
        /// Create a 'Header' property on the request
        /// </summary>
        [JsonProperty(PropertyName="Header")]
        public ApiRequestHeader Header { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApiRequestBase()
        {
            Header = new ApiRequestHeader();         
        }
    }

    /// <summary>
    /// Request that contains a 'Body' element
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    public class ApiRequestBase<TBody>:ApiResponseBase
    {
        public ApiRequestBase():base()
        {

        }

        /// <summary>
        /// Content of request
        /// </summary>
        public TBody Body { get; set; }
    }
}
