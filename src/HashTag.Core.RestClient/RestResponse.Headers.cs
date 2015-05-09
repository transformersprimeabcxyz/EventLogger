using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HashTag.Web.Http
{
    /// <summary>
    /// Convenience methods to access response headers
    /// </summary>
    public partial class RestResponse<T>
    {
        /// <summary>
        /// Standard .Net StatusCode as received from HTTP
        /// </summary>
        [JsonIgnore]
        public HttpStatusCode StatusCode
        {
            get
            {
                return Web != null ? Web.StatusCode : HttpStatusCode.Unused;
            }
        }

        /// <summary>
        /// Status text as received from HTTP
        /// </summary>
        public string StatusText
        {
            get
            {
                return StatusCode.ToString();
            }
        }
        /// <summary>
        /// Standard MIME type (e.g. application/json) for the body
        /// </summary>
        public string ContentType
        {
            get
            {
                return (Web != null & Web.Content != null && Web.Content.Headers != null && Web.Content.Headers.ContentType != null) ? Web.Content.Headers.ContentType.MediaType : "(not defined)";
            }
        }
        /// <summary>
        /// Returned value of the LocationHeader on response (esp. if there is a 201 or 302). Or null if Location is not in header's collection (Convenience method)
        /// </summary>
        public Uri Location()
        {
            return Headers.Location;
        }
        public bool LocationExists()
        {
            return Headers.Location != null;
        }

        public bool WarningExists()
        {
            return Headers != null && Headers.Warning != null && Headers.Warning.Count > 0;
        }

        /// <summary>
        /// Returns first warning on the HTTP header or null if not found
        /// </summary>
        public WarningHeaderValue WarningFirst()
        {
            if (WarningExists() == false) return default(WarningHeaderValue);
            return Web.Headers.Warning.FirstOrDefault();
        }

        /// <summary>
        /// List of warning codes on the HTTP header or empty list if not found
        /// </summary>
        public List<int> WarningIds()
        {
            return Web.Headers.Warning.Select(w => w.Code).ToList();
        }

        public bool EntityIdExists()
        {
            return Headers != null && Headers.ETag != null;
        }
        public TIdType EntityIdGet<TIdType>()
        {
            return Transform.ConvertValue<TIdType>(Headers.ETag.Tag, default(TIdType));
        }
        public string EntityId()
        {
            return EntityIdGet<string>();
        }

        public bool AuthenticateExists()
        {
            return Headers != null && Headers.WwwAuthenticate != null;
        }

        public List<string> AuthenticateSchemes()
        {
            return Headers.WwwAuthenticate.Select(a => a.Scheme).ToList();
        }

        public AuthenticationHeaderValue AuthenticateGet(string schemeName)
        {
            return Headers.WwwAuthenticate.Where(a => string.Compare(schemeName, a.Scheme, true) == 0).First();
        }
        public AuthenticationHeaderValue AuthenticateFirst()
        {
            return Headers.WwwAuthenticate.First();
        }
    }


}
