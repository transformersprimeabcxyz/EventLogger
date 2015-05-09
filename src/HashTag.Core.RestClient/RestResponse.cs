using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using HashTag.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using HashTag.Diagnostics;

namespace HashTag.Web.Http
{
    /// <summary>
    /// Contains details of a single result of a REST service call
    /// </summary>
    public partial class RestResponse<T> : IDisposable
    {
        /// <summary>
        /// True if there are no rest client errors and the http web service did not return an error code (>=400)
        /// </summary>
        public bool IsOk
        {
            get
            {
                return ((int)StatusCode) < 400 && CallException == null && string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }

        /// <summary>
        /// True if response has a body received from client
        /// </summary>
        [JsonIgnore]
        public bool HasBody
        {
            get
            {
                return !Body.Equals(default(T));
            }
        }

        /// <summary>
        /// Any error message the client wants to set during its execution of the call.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        private string _systemMessage;
        /// <summary>
        /// Any low-level system message, exception text, warnings
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SystemMessage
        {
            get
            {
                return _systemMessage;
            }
            set
            {
                _systemMessage = value;
            }
        }

        /// <summary>
        /// Actual message at the HttpLevel (rarely directly accessed by client applications)
        /// </summary>
        public HttpResponseMessage Web { get; set; }

        /// <summary>
        /// String representation of the body of the response
        /// </summary>
        public virtual T Body { get; set; }

        /// <summary>
        /// Total body length in bytes
        /// </summary>
        [JsonIgnore]
        public long ContentLength { get; set; }

        /// <summary>
        /// DateTime when last byte was received from response stream
        /// </summary>
        public DateTime ReceivedDateTime { get; set; }

        /// <summary>
        /// List of key/value headers
        /// </summary>
        [JsonIgnore]
        public HttpResponseHeaders Headers
        {
            get
            {
                return Web != null && Web.Headers != null ? Web.Headers : default(HttpResponseHeaders);
            }
        }


        /// <summary>
        /// Internal exception (usually not WebException) client wants to assign to this session.
        /// </summary>
        public LogException ExecuteException { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RestResponse()
        {
            
        }

        /// <summary>
        /// Constructor.  Initialize class with content of exception and any embedded response stream data
        /// </summary>
        /// <param name="exception">Exception encountered when calling service</param>
        public RestResponse(HttpRequestException exception)
            : this()
        {
            CallException = new LogException(exception);
        }

        /// <summary>
        /// .Net WebException, if any, that occured during processing service response
        /// </summary>
        public LogException CallException { get; set; }

        /// <summary>
        /// Converts response into a well formatted string suitable for display and logging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        /// <summary>
        /// Load all the details of the .Net framework response into the RestResponse.  Called by HTTP Client when response is received
        /// </summary>
        /// <param name="httpClientResponse">Raw response returned from .Net HttpClient.SendAsync</param>
        internal void ReadHttpResponseMessageAsync(Task<HttpResponseMessage> httpClientResponse, RestSession<T> session)
        {
            try
            {
                session.Response.Web = httpClientResponse.Result;
                session.Request.Web = null;

                if (httpClientResponse.IsFaulted || httpClientResponse.Exception != null)
                {
                    if (httpClientResponse.Exception != null)
                    {
                        this.ErrorMessage = httpClientResponse.Exception.ToString();
                        session.ClientException = new LogException(httpClientResponse.Exception.Flatten());
                    }
                    this.ReceivedDateTime = DateTime.Now;
                    return; //this might be returning too early on some cases and we might miss detailed messages from HttpClient?
                }

                var msg = session.Response.Web;

                if (msg != null && msg.Content != null)
                {
                    ContentLength = msg.Content.Headers.ContentLength.HasValue ? msg.Content.Headers.ContentLength.Value : -1;
                    ReadBodyContent(msg);
                }
            }
            finally
            {
                this.ReceivedDateTime = DateTime.Now;
            }
        }

        protected virtual void ReadBodyContent(HttpResponseMessage msg)
        {
            if (typeof(T).Name == "String")
            {
                var readTask = msg.Content.ReadAsStringAsync();
                readTask.Wait();
                var result = readTask.Result;
                this.Body = (T)(object)result;
            }
            else
            {
                try
                {
                    this.Body = msg.Content.ReadAsAsync<T>().Result;
                    if (this.Body.Equals(default(T)))
                    {
                        var content = msg.Content.ReadAsStringAsync().Result ?? "(no content in body)";                    
                        this.SystemMessage = string.Format("Received body[0..{1}]: '{0}'",content.Substring(0,Math.Min(content.Length,999)),Math.Min(content.Length,999));
                    }
                }
                catch(Exception ex)
                {
                    this.ErrorMessage = ex.Message;
                    var content = msg.Content.ReadAsStringAsync().Result ?? "(no content in body)";
                    
                    this.SystemMessage = string.Format("Received body[0..{1}]: '{0}'",content.Substring(0,Math.Min(content.Length,999)),Math.Min(content.Length,999));
                    this.ExecuteException = new LogException(ex);
                }
            }
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                if (Web != null)
                {
                    Web.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~RestResponse()
        {
            Dispose(false);
        }
    }
}
