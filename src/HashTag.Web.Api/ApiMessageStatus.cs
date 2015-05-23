#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Reflects how important the creator of this message considers the message in context
    /// </summary>
    public enum ApiMessageStatus
    {
        /// <summary>
        /// Message status was not set (0)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Debug level (100)
        /// </summary>
        Verbose = 100,

        /// <summary>
        /// Information somebody may want to pay attention to (200)
        /// </summary>
        Info = 200,

        /// <summary>
        /// Message reflects a succesful completion of an action.  Logical alias for 'Info' (210)
        /// </summary>
        Ok = 210,

        /// <summary>
        /// Very important and should be noted by the caller (300)
        /// </summary>
        Warning = 300,

        /// <summary>
        /// A bad condition occured, often a business error rather than a system error (e.g. 'Person already exists in this course') (400)
        /// </summary>
        Error = 400,

        /// <summary>
        /// An unexpected error occured, often a system exception or activity that might have left the system or data in an unstable state (500)
        /// </summary>
        Critical = 500
    }
}
