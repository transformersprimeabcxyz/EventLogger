using HashTag.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HashTag.Diagnostics
{
    public class LogHttpContext
    {
        public LogHttpContext()
        {

        }

        public LogHttpContext(HttpContext context, HttpCaptureFlags flags)
        {
            if (context == null) return;
            var request = context.Request;
            if (request == null)
            {
                return;
            }

            extractContextVariables(context,request,flags);            
        }

        private void extractContextVariables(HttpContext context, HttpRequest request, HttpCaptureFlags flags)
        {
            if ((flags & HttpCaptureFlags.ServerVariables) == HttpCaptureFlags.ServerVariables)
            {
                for(int x=0;x<request.ServerVariables.Count;x++)
                {          
                    
                    var k = request.ServerVariables.AllKeys[x];
                    var v = request.ServerVariables[k];
                    if (string.Compare("AUTH_PASSWORD",k,true )==0 && string.IsNullOrWhiteSpace(v))
                    {
                        v = "****";
                    }
                    this.ServerVariables.Add(k,v);
                }
            }

            if ((flags & HttpCaptureFlags.QueryString) == HttpCaptureFlags.QueryString)
            {
                foreach (string key in request.QueryString.Keys)
                {
                    QueryString.Add(key, request.QueryString[key]);
                }
            }

            if ((flags & HttpCaptureFlags.Cookies) == HttpCaptureFlags.Cookies)
            {
                for(int x = 0;x<request.Cookies.Count;x++)
                {
                    var cookie = request.Cookies[0];
                    Cookies.Add(cookie.Name, cookie.Value);
                }
            }
            if ((flags & HttpCaptureFlags.Form) == HttpCaptureFlags.Form)
            {
                foreach (string key in request.Form.Keys)
                {
                    Form.Add(key, request.Form[key]);
                }
            }      
      
            if ((flags & HttpCaptureFlags.Headers) == HttpCaptureFlags.Headers)
            {
                foreach (string key in request.Headers.Keys)
                {
                    Headers.Add(key, request.Headers[key]);
                }
            }
        }

        private PropertyBag _headers;
        public PropertyBag Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new PropertyBag();
                }
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }
        private PropertyBag _serverVariables;

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public PropertyBag ServerVariables 
        {
            get {
                if (_serverVariables == null)
                {
                    _serverVariables = new PropertyBag();
                }
                return _serverVariables;
            }
            set
            {
                _serverVariables = value;
            }
        }

        private PropertyBag _queryString;

        /// <summary>
        /// Gets a collection representing the Web query string variables
        /// captured as part of diagnostic data for the error.
        /// </summary>
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public PropertyBag QueryString 
        {
            get
            {
                if (_queryString == null)
                {
                    _queryString = new PropertyBag();
                }
                return _queryString;
            }
            set
            {
                _queryString = value;
            }
        }

        private PropertyBag _form;
        /// <summary>
        /// Gets a collection representing the form variables captured as 
        /// part of diagnostic data for the error.
        /// </summary>
        public PropertyBag Form
        {
            get
            {
                if (_form == null)
                {
                    _form = new PropertyBag();
                }
                return _form;
            }
            set
            {
                _form = value;
            }
        }

        private PropertyBag _cookies;
        /// <summary>
        /// Gets a collection representing the client cookies
        /// captured as part of diagnostic data for the error.
        /// </summary>

        public PropertyBag Cookies
        {
             get
            {
                if (_cookies == null)
                {
                    _cookies = new PropertyBag();
                }
                return _cookies;
            }
            set
            {
                _cookies = value;
            }
        }
    }
}
