#pragma warning disable 1591

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Represents a bit of reponse metadata text this API wishes to include in the response.  Normally do not use this class for returning client data, instead return client data within the body of the message.  ApiMessages are helpful for returning
    /// system messages and error messages or other out-of-band contextual information (e.g. "Duplicate record exception on table FOO_2","You are not authorized to eat french-fries")
    /// </summary>
    /// <remarks>Similar to IDataErrorInfo implmentations with extensions</remarks>
    public partial class ApiMessage
    {

        private ApiMessageStatus? _msgStatus;

        /// <summary>
        /// String severitiy of this message (e.g. 'error','warning','info').
        /// </summary>
        public string StatusCode
        {
            get
            {
                return Status.ToString();
            }
        }

        protected ApiMessageStatus? InternalStatus
        {
            get
            {
                return _msgStatus;
            }
        }

        /// <summary>
        /// Determines how important the creator of this message considers it. (default: Info)  If not explicitly set returns Error if Exception is set, otherwise return Info
        /// </summary>
        public ApiMessageStatus Status
        {
            get
            {
                if (_msgStatus.HasValue) return _msgStatus.Value;

                return Exception != null ? ApiMessageStatus.Error : ApiMessageStatus.Ok;
            }
            set
            {
                _msgStatus = value;
            }
        }

        /// <summary>
        /// (optional) System wide identifier (e.g. AP10001, HR92922, ERR.10) that identifies this message and/or message state (success, error).  
        /// Often used in routing, response strategies, and especially localization as a key to resource files.  Leave empty to have serializer ignore it.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MessageCode { get; set; }

        /// <summary>
        /// Text to display to user
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayMessage { get; set; }

        private string _systemMessage = null;
        /// <summary>
        /// Detailed system level information (error messages, reference numbers, etc) but not generally displayed to user.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SystemMessage
        {
            get
            {
                if (_systemMessage == null)
                {
                    if (Exception != null)
                    {
                        return Exception.Message;
                    }
                }
                return _systemMessage;
            }
            set
            {
                _systemMessage = value;
            }
        }


        /// <summary>
        /// Serialized exception details associated with this message.  Often used in core-to-core calls where 
        /// application developers might need service error details to help troubleeshoot service.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ApiException Exception { get; set; }

        /// <summary>
        /// Any reference to identify this message to caller.  Often field name or text connecting an field or property to a message.  Leave blank for response level messages
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }

        /// <summary>
        /// Return TRUE if Status of message is set to 'Info' or less severe
        /// </summary>
        public virtual bool IsOk
        {
            get
            {
                if (Status >= ApiMessageStatus.Error) return false;

                return true;
            }
        }

        /// <summary>
        /// Craete a new message.  Use the 'Message' property to retrieve backing mesage that is being built
        /// </summary>
        /// <returns></returns>
        public static ApiMessageBuilder NewMessage()
        {
            return ApiMessageBuilder.NewMessage();
        }
    }

}
