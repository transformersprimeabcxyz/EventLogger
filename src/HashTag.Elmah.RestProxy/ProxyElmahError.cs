﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HashTag.Elmah.RestProxy
//{
//    public class ProxyElmahError
//    {
//         private readonly Exception _exception;
//        private string _applicationName;
//        private string _hostName;
//        private string _typeName;
//        private string _source;
//        private string _message;
//        private string _detail;
//        private string _user;
//        private DateTime _time;
//        private int _statusCode;
//        private string _webHostHtmlMessage;
//        private IDictionary<string,string> _serverVariables;
//        private  IDictionary<string,string>  _queryString;
//        private  IDictionary<string,string>  _form;
//        private  IDictionary<string,string>  _cookies;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Error"/> class.
//        /// </summary>

//        public ProxyElmahError() {}

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Error"/> class
//        /// from a given <see cref="Exception"/> instance.
//        /// </summary>

//        public ProxyElmahError(Exception e) : 
//            this(e, null) {}

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Error"/> class
//        /// from a given <see cref="Exception"/> instance and 
//        /// <see cref="HttpContext"/> instance representing the HTTP 
//        /// context during the exception.
//        /// </summary>

//        public ProxyElmahError(Exception e, HttpContext context)
//        {
//            if (e == null)
//                throw new ArgumentNullException("e");

//            _exception = e;
//            Exception baseException = e.GetBaseException();

//            //
//            // Load the basic information.
//            //

//            _hostName = Environment.TryGetMachineName(context);
//            _typeName = baseException.GetType().FullName;
//            _message = baseException.Message;
//            _source = baseException.Source;
//            _detail = e.ToString();
//            _user = Mask.NullString(Thread.CurrentPrincipal.Identity.Name);
//            _time = DateTime.Now;

//            //
//            // If this is an HTTP exception, then get the status code
//            // and detailed HTML message provided by the host.
//            //

//            HttpException httpException = e as HttpException;

//            if (httpException != null)
//            {
//                _statusCode = httpException.GetHttpCode();
//                _webHostHtmlMessage = TryGetHtmlErrorMessage(httpException);
//            }

//            //
//            // If the HTTP context is available, then capture the
//            // collections that represent the state request as well as
//            // the user.
//            //

//            if (context != null)
//            {
//                IPrincipal webUser = context.User;
//                if (webUser != null
//                    && Mask.NullString(webUser.Identity.Name).Length > 0)
//                {
//                    _user = webUser.Identity.Name;
//                }

//                HttpRequest request = context.Request;

//                _serverVariables = CopyCollection(request.ServerVariables);

//                if (_serverVariables != null)
//                {
//                    // Hack for issue #140:
//                    // http://code.google.com/p/elmah/issues/detail?id=140
 
//                    const string authPasswordKey = "AUTH_PASSWORD";
//                    string authPassword = _serverVariables[authPasswordKey];
//                    if (authPassword != null) // yes, mask empty too!
//                        _serverVariables[authPasswordKey] = "*****";
//                }

//                _queryString = CopyCollection(request.QueryString);
//                _form = CopyCollection(request.Form);
//                _cookies = CopyCollection(request.Cookies);
//            }
//        }

//        private static string TryGetHtmlErrorMessage(HttpException e)
//        {
//            Debug.Assert(e != null);

//            try
//            {
//                return e.GetHtmlErrorMessage();
//            }
//            catch (SecurityException se) 
//            {
//                // In partial trust environments, HttpException.GetHtmlErrorMessage() 
//                // has been known to throw:
//                // System.Security.SecurityException: Request for the 
//                // permission of type 'System.Web.AspNetHostingPermission' failed.
//                // 
//                // See issue #179 for more background:
//                // http://code.google.com/p/elmah/issues/detail?id=179
                
//                Trace.WriteLine(se);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets the <see cref="Exception"/> instance used to initialize this
//        /// instance.
//        /// </summary>
//        /// <remarks>
//        /// This is a run-time property only that is not written or read 
//        /// during XML serialization via <see cref="ErrorXml.Decode"/> and 
//        /// <see cref="ErrorXml.Encode(Error,XmlWriter)"/>.
//        /// </remarks>

//        public Exception Exception
//        {
//            get { return _exception; }
//        }

//        /// <summary>
//        /// Gets or sets the name of application in which this error occurred.
//        /// </summary>

//        public string ApplicationName
//        { 
//            get { return Mask.NullString(_applicationName); }
//            set { _applicationName = value; }
//        }

//        /// <summary>
//        /// Gets or sets name of host machine where this error occurred.
//        /// </summary>
        
//        public string HostName
//        { 
//            get { return Mask.NullString(_hostName); }
//            set { _hostName = value; }
//        }

//        /// <summary>
//        /// Gets or sets the type, class or category of the error.
//        /// </summary>
        
//        public string Type
//        { 
//            get { return Mask.NullString(_typeName); }
//            set { _typeName = value; }
//        }

//        /// <summary>
//        /// Gets or sets the source that is the cause of the error.
//        /// </summary>
        
//        public string Source
//        { 
//            get { return Mask.NullString(_source); }
//            set { _source = value; }
//        }

//        /// <summary>
//        /// Gets or sets a brief text describing the error.
//        /// </summary>
        
//        public string Message 
//        { 
//            get { return Mask.NullString(_message); }
//            set { _message = value; }
//        }

//        /// <summary>
//        /// Gets or sets a detailed text describing the error, such as a
//        /// stack trace.
//        /// </summary>

//        public string Detail
//        { 
//            get { return Mask.NullString(_detail); }
//            set { _detail = value; }
//        }

//        /// <summary>
//        /// Gets or sets the user logged into the application at the time 
//        /// of the error.
//        /// </summary>
        
//        public string User 
//        { 
//            get { return Mask.NullString(_user); }
//            set { _user = value; }
//        }

//        /// <summary>
//        /// Gets or sets the date and time (in local time) at which the 
//        /// error occurred.
//        /// </summary>
        
//        public DateTime Time 
//        { 
//            get { return _time; }
//            set { _time = value; }
//        }

//        /// <summary>
//        /// Gets or sets the HTTP status code of the output returned to the 
//        /// client for the error.
//        /// </summary>
//        /// <remarks>
//        /// For cases where this value cannot always be reliably determined, 
//        /// the value may be reported as zero.
//        /// </remarks>
        
//        public int StatusCode 
//        { 
//            get { return _statusCode; }
//            set { _statusCode = value; }
//        }

//        /// <summary>
//        /// Gets or sets the HTML message generated by the web host (ASP.NET) 
//        /// for the given error.
//        /// </summary>
        
//        public string WebHostHtmlMessage
//        {
//            get { return Mask.NullString(_webHostHtmlMessage); }
//            set { _webHostHtmlMessage = value; }
//        }

//        /// <summary>
//        /// Gets a collection representing the Web server variables
//        /// captured as part of diagnostic data for the error.
//        /// </summary>
        
//        public NameValueCollection ServerVariables 
//        { 
//            get { return FaultIn(ref _serverVariables);  }
//        }

//        /// <summary>
//        /// Gets a collection representing the Web query string variables
//        /// captured as part of diagnostic data for the error.
//        /// </summary>
        
//        public NameValueCollection QueryString 
//        { 
//            get { return FaultIn(ref _queryString); } 
//        }

//        /// <summary>
//        /// Gets a collection representing the form variables captured as 
//        /// part of diagnostic data for the error.
//        /// </summary>
        
//        public NameValueCollection Form 
//        { 
//            get { return FaultIn(ref _form); }
//        }

//        /// <summary>
//        /// Gets a collection representing the client cookies
//        /// captured as part of diagnostic data for the error.
//        /// </summary>

//        public NameValueCollection Cookies 
//        {
//            get { return FaultIn(ref _cookies); }
//        }

//        /// <summary>
//        /// Returns the value of the <see cref="Message"/> property.
//        /// </summary>

//        public override string ToString()
//        {
//            return this.Message;
//        }

//        /// <summary>
//        /// Creates a new object that is a copy of the current instance.
//        /// </summary>

//        object ICloneable.Clone()
//        {
//            //
//            // Make a base shallow copy of all the members.
//            //

//            Error copy = (Error) MemberwiseClone();

//            //
//            // Now make a deep copy of items that are mutable.
//            //

//            copy._serverVariables = CopyCollection(_serverVariables);
//            copy._queryString = CopyCollection(_queryString);
//            copy._form = CopyCollection(_form);
//            copy._cookies = CopyCollection(_cookies);

//            return copy;
//        }

//        private static NameValueCollection CopyCollection(NameValueCollection collection)
//        {
//            if (collection == null || collection.Count == 0)
//                return null;

//            return new NameValueCollection(collection);
//        }

//        private static NameValueCollection CopyCollection(HttpCookieCollection cookies)
//        {
//            if (cookies == null || cookies.Count == 0)
//                return null;

//            NameValueCollection copy = new NameValueCollection(cookies.Count);

//            for (int i = 0; i < cookies.Count; i++)
//            {
//                HttpCookie cookie = cookies[i];

//                //
//                // NOTE: We drop the Path and Domain properties of the 
//                // cookie for sake of simplicity.
//                //

//                copy.Add(cookie.Name, cookie.Value);
//            }

//            return copy;
//        }

//        private static NameValueCollection FaultIn(ref NameValueCollection collection)
//        {
//            if (collection == null)
//                collection = new NameValueCollection();

//            return collection;
//        }
//    }

//}