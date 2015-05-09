using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using HashTag.Diagnostics;

namespace HashTag.Web.Http
{
   
    /// <summary>
    /// All details about a single request/response
    /// </summary>
    public class RestSession<TBodyType>:IDisposable, IRestSession<TBodyType>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RestSession() { }

        /// <summary>
        /// Returns True if successfully contacted service and received a reply
        /// </summary>
        public bool IsCallOk
        {
            get
            {
                return ClientException == null;
            }
        }

        /// <summary>
        /// Returns true if service call to web service was successful *AND* the returned StatusCode from the service is &lt; 400
        /// </summary>
        public bool IsOk
        {
            get
            {
                return IsCallOk && Response != null && Response.IsOk;
            }
        }

        /// <summary>
        /// Convenience Wrapper.  Get's status code of session's response or default of InternalServerError (500)
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                if (this.Response  != null)
                {
                    return Response.StatusCode;
                }

                return HttpStatusCode.Unused;
            }
        }

        /// <summary>
        /// Convenience Wrapper.  Returns body of *Response* or null if response is not populated
        /// </summary>
        /// <exception cref="RestResponseBodyNotAvailableException">Thrown when Session is in indeterminate state and cannot reliably return body</exception>
        [JsonIgnore]
        public virtual TBodyType Body
        {
            get
            {
                if (!CanRetrieveBody)
                {
                    throw new RestResponseBodyNotAvailableException(this.ToString(),"Body cannot be retrieved from session or response when either is in an error state.  Session details are included as part of this exception");
                }

                if (this.Response != null)
                {
                    return Response.Body;
                }
                return default(TBodyType); // missing body might not be an error so don't throw an exception
            }
        }

        /// <summary>
        /// True if body can be reliably retrieved because session is not in an error state
        /// </summary>
        public bool CanRetrieveBody
        {
            get
            {
                return (this.IsCallOk == true
                    || this.ClientException != null
                    || (this.Response != null && this.Response.SystemMessage != null)
                    || (this.Response != null && this.Response.CallException != null)
                    || (this.Response != null && this.Response.ExecuteException != null));
            }
        }

        /// <summary>
        /// Time it took for request/response including reading all response bytes from stream
        /// </summary>
        public virtual TimeSpan ElapsedTime
        {

            get
            {
                if (Request != null && Response != null)
                {
                    return Response.ReceivedDateTime - Request.SentDateTime;
                }
                if (Request != null && Response == null)
                {
                    return DateTime.Now - Request.SentDateTime;
                }

                return new TimeSpan(0);

            }
            set
            {
                // for serialization compatibility
            }
        }

        /// <summary>
        /// Internal exception that happened when building/sending/receiving HttpRequest
        /// </summary>
        public LogException ClientException { get; set; }

        /// <summary>
        /// Request part of this session.  May be null if request has not been set
        /// </summary>
        public RestRequest Request { get; set; }

        /// <summary>
        /// Response part of this session.  May be null if response has not yet been received
        /// </summary>
        public RestResponse<TBodyType> Response { get; set; }

        /// <summary>
        /// Formats session into a well defined string
        /// </summary>
        /// <returns>Session as a display compatible string</returns>
        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this,Formatting.Indented);
            }
            catch (Exception ex)
            {
                var s = ex.Expand();
                return s;
            }
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                if (Request != null)
                {
                    Request.Dispose();
                }
                if (Response != null)
                {
                    Response.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~RestSession()
        {
            Dispose(false);
        }
    }
}
