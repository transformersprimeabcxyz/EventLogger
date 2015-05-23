#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Defines common fields the API request messages should have.  Inherit from this class for creating implementation specific common patterns.
    /// </summary>
    public class ApiRequestHeader
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ApiRequestHeader()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callerReference"></param>
        public ApiRequestHeader(string callerReference)
        {
            CallerReference = callerReference;
        }

        /// <summary>
        /// Any short tracking text caller wants to provide to service.  Service implementations should return this value in responses.  NOTE:  Service may truncate too long of reference.  
        /// Often used in asynchronouse scenarios where multiple requests are submitted before responses are received
        /// </summary>
        public string CallerReference { get; set; }

        /// <summary>
        /// The authenticated user sending this request.  Often set using an appliation wide filter.  Very handy when having to track who performed what action.  NOTE:  For security purposes
        /// ignored on Json serialization
        /// </summary>
        [JsonIgnore]
        public string Actor { get; set; }
    }
}
