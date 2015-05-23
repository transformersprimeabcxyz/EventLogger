#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    public partial class ApiResponseBase
    {
        /// <summary>
        /// Create a 'Header' property on the response class (Default: true)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeHeader()
        {
            return true; //default to serialize
        
        }

        /// <summary>
        /// Create an IsOk property on the response class 'Response.IsOk' instead of 'Response.Header.IsOk' (Default: false)
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeIsOk()
        {            
            return false; //default to serialize
        }
    }
}
