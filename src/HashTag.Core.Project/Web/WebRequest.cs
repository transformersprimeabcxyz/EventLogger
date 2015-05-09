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
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using HashTag;
using System.IO;
namespace HashTag.Web
{
    /// <summary>
    /// Lightweight DTO clone of the HTTP context.  Useful for sending HTTP context information to non-hosted clients and libraries (e.g. automated test runners)
    /// </summary>
    public class WebRequest
    {
        public NameValueCollection QueryString { get; set; }
        public NameValueCollection Form { get; set; }
        public NameValueCollection ServerVariables { get; set; }
        public NameValueCollection Params { get; set; }
        public NameValueCollection Headers { get; set; }
        public Stream InputStream { get; set; }

        /// <summary>
        /// Default constructor.  Used primarily for instantiation in non-http environments (e.g. automated test runners)
        /// </summary>
        public WebRequest()
        {

        }

        /// <summary>
        /// Create a light weight clone of an incomming web request. NOTE: InputStream maps to the actual input stream of the HttpRequest.InputStream
        /// </summary>
        /// <param name="request">Hydrated request to 'clone'</param>
        public WebRequest(System.Web.HttpRequest request)
        {
            Headers = request.Headers.Clone();
            Params = request.Params.Clone();
            QueryString = request.QueryString.Clone();
            ServerVariables = request.ServerVariables.Clone();
            Form = request.Form.Clone();
            InputStream = request.InputStream;
        }

        /// <summary>
        /// Create a light weight clone of an incomming web request. NOTE: InputStream maps to the actual input stream of the HttpRequest.InputStream
        /// </summary>
        /// <param name="request">Hydrated request to 'clone'</param>
        /// <returns>Copy of commonly used collections of the request</returns>
        public static WebRequest Map(HttpRequest request)
        {
            return new WebRequest(request);
        }
    }
}