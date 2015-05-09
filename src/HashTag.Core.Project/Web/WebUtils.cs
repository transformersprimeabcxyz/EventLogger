/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using System.Collections.Specialized;
using System.Security.Principal;
using HashTag.Properties;

namespace HashTag.Web
{
    /// <summary>
    /// Miscellaneous collection of web-centric methods
    /// </summary>
    public static class WebUtils
    {
        public static T Request<T>(string key)
        {

            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException(CoreResources.MSG_Web_MissingContext);
            }
            string result = HttpContext.Current.Request[key];
            if (result == null)
            {

            }
            return Transform.ConvertValue<T>(result);

        }
        /// <summary>
        /// Encode a string to make is safe for rendering on a web page
        /// </summary>
        /// <param name="plainText">Unescaped text</param>
        /// <returns>Encoded text (or plainText if being called outside of an active HTTPContext)</returns>
        public static string HttpEncode(string plainText)
        {
            if (HttpContext.Current == null) return plainText;
            return HttpContext.Current.Server.HtmlEncode(plainText);
        }
        private const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";

        
        public static NameValueCollection ToList(HttpRequest request=null)
        {
            NameValueCollection list = new NameValueCollection();
            if (request == null)
            {
                if (HttpContext.Current != null)
                {
                    request = HttpContext.Current.Request;
                }
            }
            if (request == null)
            {
                list.Add("", CoreResources.MSG_Web_Request_NotFound);
                return list;
            }
            for (int x = 0; x < request.ServerVariables.Count; x++)
            {
                string key = request.ServerVariables.AllKeys[x];
                if (key == "ALL_RAW" || key == "ALL_HTTP") continue;             
                string value = request.ServerVariables.Get(x);
             
                list.Add(string.Format("Server[{0}]", key), value);
            }

            list.Add(ToList(request.Cookies));

            return list;
        }

        public static NameValueCollection ToList(HttpCookieCollection cookies)
        {
            var list = new NameValueCollection();
            if (cookies == null) return list;
            return ToList(cookies, list);            
        }
        public static NameValueCollection ToList(HttpCookieCollection cookies, NameValueCollection list)
        {
            for(int x=0;x<cookies.Count;x++)
            {
                var cookie = cookies[x];
                var cookieId = string.Format("Cookie[{0}:{1}]", cookie.Name, x);
                list.Add(string.Format("{0}.Name", cookieId), cookie.Name);
                list.Add(string.Format("{0}.Value", cookieId), (cookie.Value == null) ? ("(null)") : cookie.Value);
                list.Add(string.Format("{0}.Expires",cookieId),string.Format("{0:yyyy-mm-ddTHH:MM:ss}",cookie.Expires));
                list.Add(string.Format("{0}.HttpOnly", cookieId), cookie.HttpOnly.ToString());
                list.Add(string.Format("{0}.Path", cookieId), cookie.Path);
                list.Add(string.Format("{0}.Secure", cookieId), cookie.Secure.ToString());
                
                if (cookie.Values != null)
                {
                    for (int y = 0; y < cookie.Values.Count; y++)
                    {
                        list.Add(string.Format("{0}.Values[{1}:{2}]", cookieId, cookie.Values.Keys[y], y), cookie.Values[y]);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Make a BASE authorization header
        /// </summary>
        /// <param name="userName">Parameter for authorization header</param>
        /// <param name="password">Parameter for authorization header</param>
        /// <returns>Header string suitable for embedding to HTTP Request header</returns>
        public static string MakeBasicAuthorizationHeader(string userName, string password)
        {
            string credentials = String.Format("{0}:{1}", userName, password);
            byte[] bytes = Encoding.ASCII.GetBytes(credentials);
            string base64 = Convert.ToBase64String(bytes);
            string authorization = String.Concat("Basic ", base64);
            return authorization;
        }
    } //class WebUtils
}