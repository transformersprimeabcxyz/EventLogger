using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Configuration;
using HashTag.Diagnostics;

namespace HashTag.Web.Http
{

    /// <summary>
    /// Opens a channel to a REST based service.  Can be used to access other kinds of HTTP end-points but may need some trial and error.
    /// </summary>
    public partial class RestClient : HttpClient, IDisposable
    {
        private RestConfig _activeConfig;

        /// <summary>
        /// Default Constructor.  Uses hard-coded configfuration
        /// </summary>
        public RestClient()
            : base()
        {
            _activeConfig = new RestConfig();
        }

        /// <summary>
        /// Constructor.  Sets configuration BaseUrl property.  Use '[connectionStringName]'  to read url from .config &lt;connectionStrings&gt; section
        /// </summary>
        /// <param name="urlStem">Part of url that will be prepended to every request.  Allows for shorter relative urls in actual VERB calls</param>
        /// <param name="args">Any arguments needed to supply to <paramref name="urlStem"/></param> e.g 'http://{0}/myservice/api' &lt;= 'devserver:2324'
        public RestClient(string urlStem, params object[] args)
            : this()
        {
            urlStem = resolveUrl(urlStem);
            _activeConfig.BaseUrl = string.Format(urlStem, args);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Settings that control how client connects to service end-point</param>
        public RestClient(RestConfig config)
        {
            _activeConfig = config;
        }

        /// <summary>
        /// Settings that conrol how client connects to service end-point.  Modify thesse settings
        /// to customize behaviors and/or connect to internal events
        /// </summary>
        public RestConfig CurrentConfiguration
        {
            get
            {
                return _activeConfig;
            }
            set
            {
                _activeConfig = value;
            }
        }

        /// <summary>
        /// Use Fluent interface to build up a request to send to http endpoint
        /// </summary>
        public RestRequestBuilder Request
        {
            get
            {
                return new RestRequestBuilder();
            }
        }

        /// <summary>
        /// Low level doing all the heavy lifting of connecting to service, transmitting message, and handling response.  Uses default configuration within client
        /// NOTE:  Do not normally call this method directly.  Use one of the VERB overloads.
        /// </summary>
        /// <param name="session">Session with hydrated request object</param>
        /// <returns>Session with response (and/or exceptions) populated</returns>
        /// <remarks>NOTE:  NOT tested with multi-part or attachment inbound messages</remarks>
        public RestSession<T> EXECUTE<T>(RestSession<T> session)
        {
            return EXECUTE<T>(session, CurrentConfiguration);
        }
        /// <summary>
        /// Low level doing all the heavy lifting of connecting to service, transmitting message, and handling response.
        /// NOTE:  Do not normally call this method directly.  Use one of the VERB overloads.
        /// </summary>
        /// <param name="session">Session with hydrated request object</param>
        /// <param name="sessionConfig">Active configuration to use for this execution instance</param>
        /// <returns>Session with response (and/or exceptions) populated</returns>
        /// <remarks>NOTE:  NOT tested with multi-part or attachment inbound messages</remarks>
        public RestSession<T> EXECUTE<T>(RestSession<T> session, RestConfig sessionConfig)
        {
            session.Response = session.Response ?? new RestResponse<T>();
            try
            {
                using (HttpClient client = sessionConfig.Client)
                {
                    var clientChannel = client.SendAsync(session.Request.Web) //send content to HTTP endpoint
                        .ContinueWith(sendTask => //response received from server
                        {
                            if (sendTask.IsFaulted == true || sendTask.Exception != null)
                            {
                                if (sessionConfig.OnSessionError != null)
                                {
                                    sessionConfig.OnSessionError(session, sendTask.Exception.Flatten());
                                }
                                return;
                            }
                            if (sessionConfig.ThrowOnHttpStatusCodeError == true)
                            {
                                sendTask.Result.EnsureSuccessStatusCode();
                            }

                            session.Response.ReadHttpResponseMessageAsync(sendTask, session);

                        });

                    if (clientChannel.Wait((int)sessionConfig.CallTimeOutMs) == false)
                    {
                        session.Response.ErrorMessage = string.Format("Call to {0} timed out after {1}ms", session.Request.Web.RequestUri.ToString(), client.Timeout.Milliseconds);
                        session.Response.Web.StatusCode = HttpStatusCode.GatewayTimeout;
                        return session;
                    }
                }
            }
            catch (Exception ex)
            {
                session.ClientException = (ex is AggregateException) ? new LogException(((AggregateException)ex).Flatten()) : new LogException(ex);
                session.Response.Web.StatusCode = HttpStatusCode.InternalServerError;

                if (sessionConfig.OnSessionError != null)
                {
                    sessionConfig.OnSessionError(session, ex);
                }
                return session;
            }
            return session;
        }

        /// <summary>
        /// Execute a GET request on the client. May contain standard string.format arguments (e.g. "/myService/{id}").  Returns body as string
        /// </summary>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Body as string</returns>
        public RestSession<string> GET(string url, params object[] args)
        {
            return GET<string>(url, args);
        }

        /// <summary>
        /// Execute a GET request on the client returning session.Body&lt;T&gt;. May contain standard string.format arguments (e.g. "/myService/{id}"
        /// </summary>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <typeparam name="T">Type of expected response in body</typeparam>
        /// <returns>Hydrated result of call</returns>
        public RestSession<T> GET<T>(string url, params object[] args)
        {
            return __sendWithoutBody(HttpMethod.Get, new RestSession<T>(), url, args);
        }
        /// <summary>
        /// Sends an object to service using PUT. Returns Body as string
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Body as string</returns>
        public RestSession<string> POST(object body, string url, params object[] args)
        {
            return POST<string>(body, url, args);
        }

        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <typeparam name="T">Type of expected response in body</typeparam>        
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>
        public RestSession<T> POST<T>(object body, string url, params object[] args)
        {
            return __sendWithBody<T>(HttpMethod.Post, new RestSession<T>(), body, url, args);
        }
        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <typeparam name="T">Type of expected response in body</typeparam>        
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>
        public RestSession<T> POST<T>(string body, string url, params object[] args)
        {
            return __sendWithStringBody<T>(HttpMethod.Post, new RestSession<T>(), body, url, args);
        }

        /// <summary>
        /// Sends an string to service using POST.  Returns Body as string
        /// </summary>
        /// <param name="body">String of data (e.g JSon or XML)</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Body as string</returns>
        public RestSession<string> POST(string body, string url, params object[] args)
        {
            return POST<string>(body, url, args);

        }

        /// <summary>
        /// Sends an object to service using DELETE (this is not a normal use-case)
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>        
        public RestSession<string> DELETE(object body, string url, params object[] args)
        {
            return DELETE<string>(body, url, args);
        }

        /// <summary>
        /// Sends an object to service using DELETE (this is not a normal use-case)
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>        
        public RestSession<T> DELETE<T>(object body, string url, params object[] args)
        {
            return __sendWithBody(HttpMethod.Delete, new RestSession<T>(), body, url, args);
        }

        /// <summary>
        /// Sends an string to service using DELETE (this is not a common use-case)
        /// </summary>
        /// <param name="body">String of data (e.g JSon or XML)</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of somekind</returns>
        public RestSession<string> DELETE(string body, string url, params object[] args)
        {
            return DELETE(body, url, args);
        }
        /// <summary>
        /// Sends an string to service using DELETE (this is not a common use-case)
        /// </summary>
        /// <param name="body">String of data (e.g JSon or XML)</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of somekind</returns>
        public RestSession<T> DELETE<T>(string body, string url, params object[] args)
        {
            return __sendWithStringBody<T>(HttpMethod.Delete, new RestSession<T>(), body, url, args);
        }
        /// <summary>
        /// Sends calls DELETE on a service. Returns Body as string
        /// </summary>
        /// <param name="body">String of data (e.g JSon or XML)</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Body as string</returns>
        public RestSession<string> DELETE(string url, params object[] args)
        {
            return DELETE<string>(url, args);
        }

        /// <summary>
        /// Sends calls DELETE on a service 
        /// </summary>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of somekind</returns>
        public RestSession<T> DELETE<T>(string url, params object[] args)
        {
            return (RestSession<T>)__sendWithoutBody(HttpMethod.Delete, new RestSession<T>(), url, args);
        }

        /// <summary>
        /// Sends files (actually named binary blobs) to a service using mulitpart/form-data
        /// </summary>
        /// <param name="attachments">List of filename,bits to post to service.  There may be several up to a common sense limit</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Result of the upload.  Attachments are sent as a block so there are not indivual return status</returns>
        public RestSession<T> UPLOAD<T>(Dictionary<string, byte[]> attachments, string url, params object[] args)
        {
            var session = new RestSession<T>();
            try
            {
                session.Request = new RestRequest();

                var webRequest = _activeConfig.NewRequest(HttpMethod.Delete, string.Format(url, args));
                var content = new MultipartFormDataContent();
                foreach (var attachment in attachments)
                {
                    content.Add(new ByteArrayContent(attachment.Value), "file", attachment.Key);
                }
                session.Request.Web.Content = content;
                session.Request.Web = webRequest;


                return EXECUTE(session);
            }
            catch (Exception ex)
            {
                session.ClientException = new LogException(ex);
                if (CurrentConfiguration.OnSessionError != null)
                {
                    CurrentConfiguration.OnSessionError(session, ex);
                }
                return session;
            }
        }

        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>        
        public RestSession<string> PUT(object body, string url, params object[] args)
        {
            return PUT<string>(body, url, args);
        }

        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <typeparam name="T">Type of expected response in body</typeparam>        
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>
        public RestSession<T> PUT<T>(object body, string url, params object[] args)
        {
            return (RestSession<T>)__sendWithBody(HttpMethod.Put, new RestSession<T>(), body, url, args);
        }

        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>        
        public RestSession<string> PUT(string body, string url, params object[] args)
        {
            return PUT<string>(body, url, args);
        }

        /// <summary>
        /// Sends an object to service using PUT
        /// </summary>
        /// <param name="body">Serializable hydrated object to send</param>
        /// <param name="url">Full or relative url of service method.  May contain standard string.format arguments (e.g. "/myService/{id}" )</param>
        /// <param name="args">Any arguments to supply to <paramref name="url"/>.  Optional</param>
        /// <typeparam name="T">Type of expected response in body</typeparam>        
        /// <returns>Hydrated result of call.  Some services might return a body of some kind</returns>
        public RestSession<T> PUT<T>(string body, string url, params object[] args)
        {
            return (RestSession<T>)__sendWithStringBody(HttpMethod.Put, new RestSession<T>(), body, url, args);
        }

        private RestSession<T> __sendWithBody<T>(HttpMethod method, RestSession<T> session, object body, string url, params object[] args)
        {
            try
            {
                session.Request = new RestRequest();
                session.Request.Body = _activeConfig.Serialize(body);

                var webRequest = _activeConfig.NewRequest(method, string.Format(url, args));
                webRequest.Properties["test1"] = "where did this go?";
                webRequest.Content = makeContent(body, new MediaTypeHeaderValue(CurrentConfiguration.DefaultBodyType));
                webRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(CurrentConfiguration.DefaultBodyType);

                session.Request.Web = webRequest;

                return EXECUTE<T>(session);
            }
            catch (Exception ex)
            {
                session.ClientException = new LogException(ex);
                if (CurrentConfiguration.OnSessionError != null)
                {
                    CurrentConfiguration.OnSessionError(session, ex);
                }
                return session;
            }
        }

        private HttpContent makeContent(object body, MediaTypeHeaderValue mediaTypeHeaderValue)
        {
            return new ObjectContent(body.GetType(), body, _activeConfig.Formatters.FindWriter(body.GetType(), mediaTypeHeaderValue));
        }

        private RestSession<T> __sendWithStringBody<T>(HttpMethod method, RestSession<T> session, string body, string url, params object[] args)
        {
            try
            {
                session.Request = new RestRequest();

                var webRequest = _activeConfig.NewRequest(method, string.Format(url, args));
                webRequest.Content = new StringContent(body);
                webRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(CurrentConfiguration.DefaultBodyType);
                session.Request.Web = webRequest;

                return EXECUTE<T>(session);
            }
            catch (Exception ex)
            {
                session.ClientException = new LogException(ex);
                if (CurrentConfiguration.OnSessionError != null)
                {
                    CurrentConfiguration.OnSessionError(session, ex);
                }
                return session;
            }
        }
        private RestSession<T> __sendWithoutBody<T>(HttpMethod method, RestSession<T> session, string url, params object[] args)
        {
            try
            {
                session.Request = new RestRequest();

                var webRequest = _activeConfig.NewRequest(method, string.Format(url, args));
                session.Request.Web = webRequest;



                return EXECUTE<T>(session);
            }
            catch (Exception ex)
            {
                session.ClientException = new LogException(ex);
                if (CurrentConfiguration.OnSessionError != null)
                {
                    CurrentConfiguration.OnSessionError(session, ex);
                }
                return session;
            }
        }

        private string resolveUrl(string url)
        {
            var resolvedUrl = url.Trim();
            if (url.StartsWith("[") == true)
            {
                var result = ConfigurationManager.ConnectionStrings[url]; //look for "[myservice]" key
                if (result != null)
                {
                    resolvedUrl = result.ConnectionString;
                }
                else
                {
                    url = url.Replace("[", "").Replace("]", "");
                    result = ConfigurationManager.ConnectionStrings[url]; //look for "myservice" key
                    if (result != null)
                    {
                        resolvedUrl = result.ConnectionString;
                    }
                    else
                    {
                        throw ExceptionFactory.New<ConfigurationErrorsException>("Unable to resolve url '{0}' using .config file", url);
                    }
                }
            }
            return resolvedUrl;
        }

        //var fileContent = new ByteArrayContent(attachment.Value); //this code is an alternative if UPLOAD doesn't work.  There isn't an UPLOAD use case yet
        //fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //{
        //    FileName = attachment.Key                        
        //};
        //content.Add(content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDisposing"></param>
        public new void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_activeConfig != null)
                {
                    _activeConfig.Dispose();
                }
            }
            base.Dispose(isDisposing);
            GC.SuppressFinalize(this);
        }
        public new void Dispose()
        {
            Dispose(true);
        }
        ~RestClient()
        {
            Dispose(false);
        }
    }
}
