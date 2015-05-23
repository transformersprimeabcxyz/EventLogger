#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Provides a common set of properties for all responses
    /// </summary>
    public partial class ApiResponseBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ApiResponseBase()
        {
            Header = new ApiResponseHeader();
        }

        /// <summary>
        /// Container for header meta-data fields
        /// </summary>
        public ApiResponseHeader Header { get; set; }

        /// <summary>
        /// True if header status is clear
        /// </summary>
        public bool IsOk
        {
            get
            {
                return Header.IsOk;
            }
        }

        /// <summary>
        /// Human readable version of this response
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    /// <summary>
    /// Creates a common set of response properties and a strongly typed 'Body' field that contains a returned object
    /// </summary>
    /// <typeparam name="TBody">Type of body content</typeparam>
    public class ApiResponseBase<TBody>:ApiResponseBase 
    {
        /// <summary>
        /// Default constructor and attempts to create an instance of TBody. Null if fails.
        /// </summary>
        public ApiResponseBase()
        {
            if (typeof(TBody).IsPrimitive == true || typeof(TBody).FullName == "System.String")
            {
                Body = default(TBody);
            }
            else
            {
                try
                {
                    Body = Activator.CreateInstance<TBody>();
                }
                catch
                {
                    //in these cases caller must initialize field;
                 //   throw new InvalidOperationException(string.Format("Unable to create an instance of '{0}'. {1}", typeof(T).FullName, ex.Expand()));
                }
            }
        }

        /// <summary>
        /// Initializes response with an error
        /// </summary>
        /// <param name="ex">Exception to be associated with this response</param>
        /// <param name="displayMessage"></param>
        /// <param name="args"></param>
        public ApiResponseBase(Exception ex, string displayMessage=null, params object[] args):base()
        {            
            SetError(ex,displayMessage,args);
        }

        /// <summary>
        /// Adds an error to this message.  More than exception can be associated with this response
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="displayMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApiMessageBuilder SetError(Exception ex, string displayMessage = null, params object[] args)
        {
            return Header.Messages.Add().Catch(ex).DisplayMessage(displayMessage,args);            
        }
        
        /// <summary>
        /// Returned object to caller.  Likely, but not guaranteed, to be initialized
        /// </summary>
        public TBody Body {get;set;}

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
