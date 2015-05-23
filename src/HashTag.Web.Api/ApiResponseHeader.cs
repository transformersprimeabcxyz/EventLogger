#pragma warning disable 1591

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Defines a standard 'header' package that can be returned from an API end-point message.  Used in 
    /// conjunction with the primary returned object.  Can be used in inheritance or composition models 
    /// </summary>
    public partial class ApiResponseHeader : ApiMessage
    {
        /// <summary>
        /// Default Contstructor
        /// </summary>
        public ApiResponseHeader()
        {

        }

        private HttpStatusCode? _httpStatus;

        /// <summary>
        /// The value of this status from the API perspective. 'OK', 'NotFound', etc. Often parallel to API returned StatusCode in HTTP Header but sometimes it will diverge
        /// </summary>
        /// <remarks>
        /// Typically an API might use one or the other of HttpStatus or ApiStatus
        /// </remarks>
        public string HttpCode
        {
            get
            {
                return HttpStatus.ToString();
            }
        }

        /// <summary>
        /// The value of this status from the API perspective. '200', '404', etc. Often parallel to API returned StatusCode in HTTP Header but sometimes it will diverge
        /// If not set, will attempt to derive status from message state
        /// </summary>
        public HttpStatusCode HttpStatus
        {
            get
            {
                if (_httpStatus.HasValue) return _httpStatus.Value;

                // attempt to find default using most severe of header.actionstatus, base.messagestatus, any defined messages                
                HttpStatusCode messageStatus=HttpStatusCode.OK;  //default each potential error source to OK
                HttpStatusCode actionStatus = HttpStatusCode.OK;
                HttpStatusCode baseStatus = HttpStatusCode.OK;
                                
                if (_messages != null)
                {
                    messageStatus = _messages.IsOk ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                }
                
                baseStatus =base.IsOk ? HttpStatusCode.OK : HttpStatusCode.BadRequest;

                if (_actionStatus != null)
                {
                    switch (_actionStatus.Value)
                    {
                        case ApiActionStatus.Denied: actionStatus = HttpStatusCode.Unauthorized; break;
                        case ApiActionStatus.Error: actionStatus= HttpStatusCode.BadRequest; break;
                        case ApiActionStatus.Success: actionStatus= HttpStatusCode.OK; break;
                        default:
                            throw new InvalidOperationException(string.Format("Unexpected {0}.{1} value", _actionStatus.Value.GetType().Name, _actionStatus.Value.ToString()));
                    }
                }
                
                //now find maximum of error sources
                var maxStatus = Math.Max((int)messageStatus, Math.Max((int)baseStatus, (int)actionStatus));

                return (HttpStatusCode)maxStatus;
            }
            set
            {
                _httpStatus = value;
            }
        }

        /// <summary>
        /// Returns string version of status value enumeration
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ActionStatusCode
        {
            get
            {
                return ActionStatus.ToString();
            }
        }


        private ApiActionStatus? _actionStatus;
        /// <summary>
        /// Returns relative success of this call. If null probes for reasonable guess based on response state
        /// </summary>
        public ApiActionStatus ActionStatus
        {
            get
            {
                if (_actionStatus.HasValue) return _actionStatus.Value;
                if (_httpStatus.HasValue)
                {
                    if (_httpStatus.Value == HttpStatusCode.Unauthorized || _httpStatus.Value == HttpStatusCode.ProxyAuthenticationRequired || _httpStatus.Value == HttpStatusCode.Forbidden)
                    {
                        return ApiActionStatus.Denied;
                    }
                    if (_httpStatus.Value >= HttpStatusCode.BadRequest)
                    {
                        return ApiActionStatus.Error;
                    }
                    return ApiActionStatus.Success;
                }

                if (_messages != null)
                {
                    return _messages.IsOk ? ApiActionStatus.Success : ApiActionStatus.Error;
                }

                if (base.InternalStatus != null)
                {
                    switch (base.InternalStatus.Value)
                    {
                        case ApiMessageStatus.Critical:
                        case ApiMessageStatus.Error:
                        case ApiMessageStatus.Warning: return ApiActionStatus.Error;
                        case ApiMessageStatus.Ok:
                        case ApiMessageStatus.Info:
                        case ApiMessageStatus.Verbose: return ApiActionStatus.Success;
                        default:
                            throw new InvalidOperationException(string.Format("Unexpected {0}.{1} value", base.InternalStatus.Value.GetType().Name, base.InternalStatus.Value.ToString()));
                    }
                }
                return ApiActionStatus.Success;
            }
            set
            {
                _actionStatus = value;
            }
        }

        private ApiMessages _messages;
        /// <summary>
        /// Messages that could pertain to sub-portions of the the response (e.g. field validation errors)
        /// </summary>
        public ApiMessages Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ApiMessages();
                }
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }


        /// <summary>
        /// Generic text provided by caller's request that identifies this call.  A request-reposne correlation id.  Often used when
        /// caller makes several async calls to service and caller needs to track each response
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CallerReference { get; set; }

        /// <summary>
        /// Return TRUE header == OK AND there are no non-OK messages in Messages list.
        /// </summary>
        public override bool IsOk
        {
            get
            {
                return base.IsOk
                    && HttpStatus < HttpStatusCode.BadRequest
                    && ActionStatus == ApiActionStatus.Success
                    && (_messages == null || _messages.IsOk);
            }
        }

        private ApiLinks _links;

        /// <summary>
        /// List of links related to this response (often parent, child, next, prev, etc.)
        /// </summary>
        public ApiLinks Links
        {
            get
            {
                if (_links == null)
                {
                    _links = new ApiLinks();
                }
                return _links;
            }
        }

        /// <summary>
        /// Convenience method to resolve links.  Normally called only once per response
        /// </summary>
        /// <param name="serviceRootUrl"></param>
        public void ResolveLinks(string serviceRootUrl)
        {
            if (serviceRootUrl == null || _links == null || _links.Count == 0) return;

            _links.Resolve(serviceRootUrl);
        }

        /// <summary>
        /// Well formatted, human readable, representation of this header
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }


    }
}
