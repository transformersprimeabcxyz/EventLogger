#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    public partial class ApiResponseHeader
    {
        /// <summary>
        /// Display Links array on Json object (present on non-empty only)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeLinks()
        {
            return _links != null && _links.Count > 0;
        }

        /// <summary>
        /// Dsipaly messages (present on non-empty only)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeMessages()
        {
            return _messages != null && _messages.Count > 0;
        }

        /// <summary>
        /// Display action status numerical value (present on explicitly set)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeActionStatus()
        {
            return _actionStatus.HasValue;
        }

        /// <summary>
        /// Display action status textual code (present on explicitly set)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeActionStatusCode()
        {
            return ShouldSerializeActionStatus();
        }

        /// <summary>
        /// Display api status HTTP numerical value (present on explicitly set)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeApiStatus()
        {
            return true; //default to serialize
        }

        /// <summary>
        /// Display api status HTTP textual code (present on explicitly set)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeApiStatusCode()
        {
            return ShouldSerializeApiStatus();
        }
    }
}
