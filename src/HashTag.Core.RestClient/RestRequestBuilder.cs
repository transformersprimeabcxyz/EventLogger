using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HashTag.Web.Http
{
    public class RestRequestBuilder
    {
        HttpRequestMessage message;

        public RestRequestBuilder()
        {
            message = new HttpRequestMessage();
        }

        public HttpRequestMessage GET(string url, params object[] args)
        {
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri(string.Format(url, args));
            return message;
        }
        public HttpRequestMessage PUT(string url, params object[] args)
        {
            message.Method = HttpMethod.Put;
            message.RequestUri = new Uri(string.Format(url, args));
            return message;
        }
        public HttpRequestMessage DELETE(string url, params object[] args)
        {
            message.Method = HttpMethod.Delete;
            message.RequestUri = new Uri(string.Format(url, args));
            return message;
        }
        public HttpRequestMessage POST(string url, params object[] args)
        {
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(string.Format(url, args));
            return message;
        }


        public RestRequestBuilder AuthorizeWith(string scheme, string parameter = null)
        {
            if (string.IsNullOrEmpty(parameter) == true)
            {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme);
            }
            else
            {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, parameter);
            }
            return this;
        }
        public RestRequestBuilder Cache(CacheControlHeaderValue cache)
        {
            message.Headers.CacheControl = cache;
            return this;
        }
        public RestRequestBuilder Expect(string name, string value)
        {
            if (message.Headers.Expect == null)
            {
                throw new NotImplementedException("TODO How to initialize message.Headers.Expect collection?");
            }
            message.Headers.Expect.Add(new NameValueWithParametersHeaderValue(name, value));
            return this;
        }

        public RestRequestBuilder Warning(int code, string agentOrHost, string text="(no-warn-text)", params object[] args)
        {
            if (message.Headers.Warning == null)
            {
                throw new NotImplementedException("TODO How to initialize message.Warning collection?");
                //message.Headers.Warning = new HttpHeaderValueCollection<WarningHeaderValue>();
            }
            message.Headers.Warning.Add(new WarningHeaderValue(code,agentOrHost,string.Format(text,args)));
            return this;
        }
        public RestRequestBuilder ProxyWith(string scheme, string parameter = null)
        {
            if (string.IsNullOrEmpty(parameter) == true)
            {
                message.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme);
            }
            else
            {
                message.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, parameter);
            }
            return this;
        }
        public RestRequestBuilder From(string fromHeader, params object[] args)
        {
            message.Headers.From = string.Format(fromHeader, args);
            return this;
        }
        public RestRequestBuilder Host(string hostHeader, params object[] args)
        {
            message.Headers.Host = string.Format(hostHeader, args);
            return this;
        }



    }
}
