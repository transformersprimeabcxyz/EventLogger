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
    /// List of messages associated with an API request-response pair
    /// </summary>
    public partial class ApiMessages : List<ApiMessage>
    {
        /// <summary>
        /// Get a named group of messages
        /// </summary>
        /// <param name="messageReference">Name of field or group of messages.  Often used in returning field level validation messages.  Empty/Null values are considered response level messages</param>
        /// <returns></returns>
        [JsonIgnore]
        public List<ApiMessage> this[string messageReference]
        {
            get
            {
                return this.Where(item => string.Compare(item.Reference, messageReference) == 0).ToList();
            }
            set
            {
                base.AddRange(value);
            }
        }

        /// <summary>
        /// True if all messages are not in 'Warning' or more severe state
        /// </summary>
        public bool IsOk
        {
            get
            {
                var firstErrorMessage = this.FirstOrDefault(item => !item.IsOk);
                return firstErrorMessage == null;
            }
        }

        /// <summary>
        /// Exposes a fluent builder for adding a message to this collection.  NOTE:  calling 'Add' without invoking additional methods will add an empty record
        /// </summary>
        /// <returns></returns>
        public ApiMessageBuilder Add()
        {
            var msg = new ApiMessage();
            base.Add(msg);
            return ApiMessageBuilder.NewMessage(msg);            
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="displayMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApiMessageBuilder Add(string displayMessage, params object[] args)
        {
            var msg = new ApiMessage();
            base.Add(msg);
            return ApiMessageBuilder.NewMessage(msg).DisplayMessage(displayMessage, args);
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="displayMessage"></param>
        /// <param name="args"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ApiMessageBuilder Add(ApiMessageStatus status, string displayMessage = null, params object[] args)
        {
            var msg = new ApiMessage();
            base.Add(msg);
            return ApiMessageBuilder.NewMessage(msg).DisplayMessage(displayMessage, args).Status(status);
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="displayMessage"></param>
        /// <param name="args"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public ApiMessageBuilder Add(Exception ex, string displayMessage = null, params object[] args)
        {
            return Add().DisplayMessage(displayMessage, args).Catch(ex);
        }
    }
}
