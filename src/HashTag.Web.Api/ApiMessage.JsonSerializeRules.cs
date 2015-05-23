using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    public partial class ApiMessage
    {
        /// <summary>
        /// Json.Net serializing instructions
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeException()
        {
            return Exception != null;

        }

        /// <summary>
        /// Json.Net serializing instructions
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeStatus()
        {
            return _msgStatus != null;

        }
        /// <summary>
        /// Json.Net serializing instructions
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeSystemMessage()
        {
            return (IsOk == false && _systemMessage != null) ? true : false;
        }
        /// <summary>
        /// Json.Net serializing instructions
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeStatusCode()
        {
            return ShouldSerializeStatus();
        }
    }
}
