#pragma warning disable 1591

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{

    public class ApiMessageBuilder
    {
        private ApiMessage _msg;
        public ApiMessageBuilder()
        {
            _msg = new ApiMessage();
        }

        public static ApiMessageBuilder NewMessage()
        {
            return new ApiMessageBuilder();
        }
        public static ApiMessageBuilder NewMessage(ApiMessage msg)
        {
            return new ApiMessageBuilder(msg);
        }

        public ApiMessageBuilder(ApiMessage msg)
        {
            _msg = msg;
        }

        public ApiMessageBuilder Catch(Exception ex = null)
        {
            if (ex == null)
            {
                _msg.Exception = null;
            }
            else
            {
                _msg.Exception = new ApiException(ex);
            }

            return this;
        }

        public ApiMessageBuilder Reference(string referenceText, params object[] args)
        {
            if (referenceText == null)
            {
                _msg.Reference = referenceText;
            }
            else
            {
                _msg.Reference = string.Format(referenceText, args);
            }
            return this;
        }
        public ApiMessageBuilder SystemMessage(string message, params object[] args)
        {
            if (message == null)
            {
                _msg.SystemMessage = null;
            }
            else
            {
                _msg.SystemMessage = string.Format(message, args);
            }
            return this;
        }
        public ApiMessageBuilder DisplayMessage(string message, params object[] args)
        {
            if (message == null)
            {
                _msg.DisplayMessage = null;
            }
            else
            {
                _msg.DisplayMessage = string.Format(message, args);
            }
            return this;
        }
        public ApiMessageBuilder MessageCode(string code, params object[] args)
        {
            if (code == null)
            {
                _msg.MessageCode = null;
            }
            else
            {
                _msg.MessageCode = string.Format(code, args);
            }
            return this;
        }
        public ApiMessageBuilder Status(ApiMessageStatus status)
        {
            _msg.Status  = status;
            return this;
        }

        public ApiMessage Message
        {
            get
            {
                return _msg;
            }
        }
    }
}

