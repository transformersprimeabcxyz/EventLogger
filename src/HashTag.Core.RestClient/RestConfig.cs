using HashTag.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;

namespace HashTag.Web.Http
{
    /// <summary>
    /// Configuration setttings for controlling the request and response of the RestClient
    /// </summary>
    public class RestConfig:HttpClientHandler,IDisposable
    {
        /// <summary>
        /// Default constructor using 
        /// </summary>
        public RestConfig()
        {          
            CallTimeOutMs = 30000;            
            base.MaxRequestContentBufferSize = int.MaxValue;
            DefaultBodyType = "application/json";
            ThrowOnHttpStatusCodeError = false;
            ThrowOnInternalError = false;
        }

        /// <summary>
        /// Throw an exception if call succeeds but result of HTTP call was &gt;= HttpStatusCode.400 (Default: false, Check ??? for details)
        /// </summary>
        public bool ThrowOnHttpStatusCodeError { get; set; }

        /// <summary>
        /// Throw an exception if some kind of internal processing error occured. (Default: false, Check ??? for details)
        /// </summary>
        public bool ThrowOnInternalError { get; set; }

        /// <summary>
        /// Maximum number of milliseconds until the client times out from calling a service.  Default: 30 seconds
        /// </summary>
        public double CallTimeOutMs { get; set; }

        /// <summary>
        /// Maximum number of bytes to buffer in response when reading buffer response.  
        /// This is a good default for most use-cases.  Default: int.Max
        /// </summary>
        public long MaximumReadBufferSize
        {
            get
            {
                return base.MaxRequestContentBufferSize;
            }
            set
            {
                base.MaxRequestContentBufferSize = value;
            }
        }

        MediaTypeFormatterCollection _formatters;
        /// <summary>
        /// List of available formatters for content type.
        /// </summary>
        public MediaTypeFormatterCollection Formatters
        {
            get
            {
                if (_formatters == null)
                {
                    var httpCfg = new HttpConfiguration();
                    if (httpCfg != null)
                    {
                        _formatters = httpCfg.Formatters;
                    }
                    if (_formatters == null)
                    {
                        _formatters = new MediaTypeFormatterCollection();
                        var jsonFormatter = new JsonMediaTypeFormatter()
                        {
                            UseDataContractJsonSerializer = false //use JSON.Net serialize by default
                        };
                        var xmlFormatter = new XmlMediaTypeFormatter()
                        {
                            UseXmlSerializer = false //use DataContract XML serializer by default
                        };
                        Formatters.Add(jsonFormatter);
                        Formatters.Add(xmlFormatter);
                    }
                    _formatters.XmlFormatter.UseXmlSerializer = false; //use DataContract XML serializer by default
                    _formatters.JsonFormatter.UseDataContractJsonSerializer = false; //use JSON.Net serialize by default
                }                
                return _formatters;
            }
            set
            {
                _formatters = value;
            }
        }

        /// <summary>
        /// Globally ignore all SLL errors.  NOTE: This value is global to the entire APP domain
        /// </summary>
        public static bool IgnoreInvalidSSLErrors
        {
            set
            {
                if (value == true)
                {
                    ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chian, SslPolicyErrors errors)=>{
                        return true;
                    };
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback=null;
                }
            }
        }

        private string _baseUrl = null;
        /// <summary>
        /// Base scheme://host[:port] to be prepended to every releative REST request.  BaseUrl.Get will never have trailing slash
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.EndsWith("/") == true)
                    {
                        value = value.Substring(0, value.Length - 1);
                    }
                }
                _baseUrl = value;
            }
        }

        /// <summary>
        /// If not null, all messages will be sent out with this value as ACCEPT header. 
        /// </summary>
        public string DefaultBodyType { get; set; }

        private static ConcurrentDictionary<string, HttpClient> _clientCache = new ConcurrentDictionary<string, HttpClient>();
        /// <summary>
        /// Returns an HTTP level client configured with values from this instance
        /// </summary>
        public virtual HttpClient Client
        {
            get
            {
                if (_clientCache.ContainsKey(BaseUrl) == false)
                {
                    var client = RegisterClient(BaseUrl);
                    _clientCache[BaseUrl] = client;
                }
                return _clientCache[BaseUrl];
            }
        }

        public virtual HttpClient RegisterClient(string baseUrl)
        {
            HttpClient client = null;
            
            if (OnCreateClientHandler != null)
            {
                var handler = OnCreateClientHandler(this); //setup proxy server, authentication, etc.
                client = new HttpClient(handler);
            }
            else
            {
                client = new System.Net.Http.HttpClient(this);
            }

            client.Timeout = TimeSpan.FromMilliseconds(CallTimeOutMs);            
            
            if (string.IsNullOrEmpty(BaseUrl) == false)
            {
                client.BaseAddress = new Uri(BaseUrl);
            }

            if (string.IsNullOrWhiteSpace(DefaultBodyType) == false)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(DefaultBodyType));
            }

            if (OnClientCreated != null)
            {
                OnClientCreated(client);
            }
            return client;
        }

        /// <summary>
        /// Create a request based on values in this configuration instance
        /// </summary>
        /// <param name="verb">The GET, PUT, POST, DELETE action</param>
        /// <param name="urlFragment">Fully quailified or relative path to service.  If relative then config.BaseUrl is used for full service resolution</param>
        /// <returns></returns>
        public virtual HttpRequestMessage NewRequest(HttpMethod verb, string urlFragment)
        {            
            var message = new HttpRequestMessage();
            message.Method = verb;
            message.RequestUri = ResolveUrl(urlFragment);
            
            if (OnRequestHeadersBuilt != null)
            {
                OnRequestHeadersBuilt(message);               
            }
            return message;
        }

        protected virtual Uri ResolveUrl(string urlFragment)
        {
            var urlString = "";
            urlFragment = urlFragment.Trim();
            if (urlFragment.StartsWith("http",StringComparison.InvariantCultureIgnoreCase)==true) //assume full scheme://host[:port]/path is provided
            {
                return new Uri(urlFragment);
            }
            
            if (urlFragment.StartsWith("/")) //remove leading slash from fragment
            {
                urlFragment = urlFragment.Substring(1,urlFragment.Length-1);
            }

            if (string.IsNullOrWhiteSpace(urlFragment)==false)
            {
                urlString = string.Format("{0}/{1}", BaseUrl, urlFragment);
            }
            else
            {
                urlString = BaseUrl;
            }
            return new Uri(urlString);
        }

        /// <summary>
        /// Fired after default headers are built on request.  Use to add/modify request header collection
        /// </summary>
        public Action<HttpRequestMessage> OnRequestHeadersBuilt;

        /// <summary>
        /// Executed when there is some kind of exeption thrown from client.  Atttach handler for logging etc. 
        /// NOTE: HTTP Status codes do not throw exceptions.  It is callers responsiblity to handle
        /// error codes (eg. 404, 500) in the OnSessionComplete handler
        /// </summary>
        public Action<object, Exception> OnSessionError;

        /// <summary>
        /// Executed when there is a successfull send/receive from service.  Atttach handler for logging etc
        /// </summary>
        public Action<object> OnSessionComplete;

        /// <summary>
        /// Initializes handler for cookies/proxy/credentials etc.  Called before internal client is instantiated
        /// </summary>
        public Func<RestConfig, HttpClientHandler> OnCreateClientHandler;

        /// <summary>
        /// Called after a client has been created so customizations might be applied
        /// </summary>
        public Action<HttpClient> OnClientCreated;
   
        public new void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                base.Dispose();
                // remove managed resources (e.g. drain event handlers)
            }
            GC.SuppressFinalize(this);
        }
        public new void Dispose()
        {
            Dispose(true);
        }
        ~RestConfig()
        {
            Dispose(false);
        }

        internal string Serialize(object body)
        {
            var defaultFormatter = new MediaTypeHeaderValue(DefaultBodyType);
            var w = Formatters.FindWriter(body.GetType(), defaultFormatter);
            using (var ms = new MemoryStream())
            {
                var task = w.WriteToStreamAsync(body.GetType(), body, ms,null, null);
                task.Wait();
                ms.Flush();
                return Transform.StreamToString(ms);
            }
        }
    }
}
