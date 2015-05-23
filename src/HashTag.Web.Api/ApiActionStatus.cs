#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Reflects the outcome of a call to action. Logically simple result of a WebApi controller.action call but often returned from a service layer and thus could refect
    /// the outcome of a service call.  Can be used when response does not relfect an HttpStatusCode, in replace of an HttpStatusCode, or to supplement other status codes
    /// </summary>
    public enum ApiActionStatus
    {
        /// <summary>
        /// 200
        /// </summary>
        Success=200,

        /// <summary>
        /// 500
        /// </summary>
        Error=500,

        /// <summary>
        /// 400
        /// </summary>
        Denied=400
    }
}
