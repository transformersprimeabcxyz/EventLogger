using HashTag.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HashTag.Web.Http
{
    /// <summary>
    /// Contains details of single HTTP request from REST client
    /// </summary>
    [Serializable]
    public class RestRequest:IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RestRequest()
        {
            this.SentDateTime = DateTime.Now;
           // Headers = new PropertyBag();
        }

        /// <summary>
        /// Constructor.  Initialize this class from HTTP WebRequest
        /// </summary>
        /// <param name="request">HTTP WebRequest</param>
        public RestRequest(HttpRequestMessage request)
            : this()
        {
            
        }

        /// <summary>
        /// Full URL including transport (e.g. http://myservices/account/1234)
        /// </summary>
        public string Address
        {
            get
            {
                return (Web!=null)?Web.RequestUri.AbsoluteUri.ToString():"(not defined)";
            }
        }

        /// <summary>
        /// MIME content type that is sent with request
        /// </summary>
        public string ContentType
        {
            get
            {
                return (Web != null && Web.Content != null && Web.Content.Headers != null) ? Web.Content.Headers.ContentType.MediaType : "(not defined)";
            }
        }

        /// <summary>
        /// DateTime when content was submitted to Web
        /// </summary>
        public DateTime SentDateTime { get; set; }

        /// <summary>
        /// Length (in bytes) of body as submitted to Uri
        /// </summary>
        public long ContentLength
        {
            get
            {
                return (Web != null && Web.Content != null && Web.Content.Headers != null) ? Web.Content.Headers.ContentLength.Value : -1L;
            }
        }

        /// <summary>
        /// Http Verb (GET,POST,DELETE,PUT) used for this request
        /// </summary>
        public string Method
        {
            get
            {
                return (Web != null) ? Web.Method.ToString() : "(not defined)";
            }
        }

        /// <summary>
        /// List of headers as supplied sent in request
        /// </summary>
        public HttpRequestHeaders Headers {
            get
            {
                return (Web != null && Web.Headers != null)?Web.Headers:default(HttpRequestHeaders);
            }
        }

        /// <summary>
        /// Textual representation of body sent to service
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// Actual message at the HttpLevel (rarely directly accessed by client applications)
        /// </summary>
        [JsonIgnore]
        public HttpRequestMessage Web { get; set; }

        /// <summary>
        /// Formatted string of request.  Not suitable for serialization
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }


        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
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
        ~RestRequest()
        {
            Dispose(false);
        }
    }
}
