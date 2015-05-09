using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Determines data to be retrieved by HTTPContextCollector
    /// </summary>
    [Flags]
    public enum HttpCaptureFlags
    {
        /// <summary>
        /// 0 - no http context will be captured
        /// </summary>
        None = 0,

        /// <summary>
        /// Return Method and full URL GET http://mysite.com/admin.aspx?q1=12
        /// </summary>
        Url = 1,

        /// <summary>
        /// Return all form fields and their values (HttpContext.Params collection)
        /// </summary>
        Form = 2,

        /// <summary>
        /// Return all session keys and the ToString() of their values
        /// </summary>
        Session = 4,

        /// <summary>
        /// Return all application keys and the ToString() of their values
        /// </summary>
        AppCache = 8,

        /// <summary>
        /// HttpContext.Items (usually for handler communication
        /// </summary>
        Items = 16,

        /// <summary>
        /// Return all HttpContext values
        /// </summary>
        All = Url | Form | Session | AppCache | Items

    }
}
