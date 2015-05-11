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
                for (int x = 0; x < request.QueryString.Count; x++)
                {

                    var k = request.QueryString.AllKeys[x];
                    var v = request.QueryString[k];
                   
                    this.QueryString.Add(k, v);
                }
            }

            if ((flags & HttpCaptureFlags.Cookies) == HttpCaptureFlags.Cookies)
            {
                for (int x = 0; x < request.Cookies.Count; x++)
                {

                    var k = request.Cookies.AllKeys[x];
                    var v = request.Cookies[k].Value;

                    this.Cookies.Add(k, v);
                }
            }
            if ((flags & HttpCaptureFlags.Form) == HttpCaptureFlags.Form)
            {
                for (int x = 0; x < request.Form.Count; x++)
                {

                    var k = request.Form.AllKeys[x];
                    var v = request.Form[k];
                   
                    this.Form.Add(k, v);
                }
            }      
      
            if ((flags & HttpCaptureFlags.Headers) == HttpCaptureFlags.Headers)
            {
                for (int x = 0; x < request.Headers.Count; x++)
                {

                    var k = request.Headers.AllKeys[x];
                    var v = request.Headers[k];

                    this.Headers.Add(k, v);
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
