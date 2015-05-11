using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using HashTag.Text;
using HashTag.Web;
using System.Security;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Facade to set values on the underlying message to be persisted to log
    /// </summary>
    public class LogMessageBuilder
    {
        Action<LogMessage> _writeAction;
        LogMessage _message;

        internal LogMessageBuilder(LogMessage messageToBuild, Action<LogMessage> writeAction)
        {
            _writeAction = writeAction;
            _message = messageToBuild;
        }

        /// <summary>
        /// Persist all built up properties to persistent store
        /// </summary>
        /// <param name="message">Message text of log message.  May include any standard string.format paramters</param>
        /// <param name="args">Any argument to supply to <paramref name="message"/></param>
        public LogMessage Write(string message, params object[] args)
        {
            _message.MessageText = TextUtils.StringFormat(message, args);
            if (_writeAction != null)
            {
                _writeAction(_message);
            }
            return _message;
        }

        /// <summary>
        /// Persist an object to log.  Uses <paramref name="messageData"/>.ToString() to serialize the message
        /// </summary>
        /// <param name="messageData">Data to be persisted to log.</param>
        public LogMessage Write(object messageData = null)
        {
            if (messageData != null)
            {
                Write(messageData.ToString());
            }
            else
            {
                if (_writeAction != null)
                {
                    _writeAction(_message);
                }
            }
            return _message;
        }

        
        /// <summary>
        /// Persist all built up properties to persistent store
        /// </summary>
        /// <param name="message">Message text of log message.  May include any standard string.format paramters</param>
        /// <param name="args">Any argument to supply to <paramref name="message"/></param>
        public LogMessage  Message(string message, params object[] args)
        {
            _message.MessageText = TextUtils.StringFormat(message, args);
            
            return _message;
        }

        /// <summary>
        /// Persist an object to log.  Uses <paramref name="messageData"/>.ToString() to serialize the message
        /// </summary>
        /// <param name="messageData">Data to be persisted to log.</param>
        public LogMessage Message(object messageData = null)
        {
            if (messageData != null)
            {
                _message.MessageText = messageData.ToString();
            }
            return _message;
        }

        /// <summary>
        /// Identifier of this message (e.g. '101', '34334').  Used in IT departments where there is a preference to log by number instead of by category and/or name.
        /// If not provided system will use a combination of Severity+Priority
        /// </summary>
        /// <param name="eventId">User defined numerical id of message</param>
        /// <returns></returns>
        public LogMessageBuilder WithId(int eventId)
        {
            _message.EventId = eventId;
            return this;
        }

        /// <summary>
        /// Set sub application key.  The application key is set in HashTag.Application.Name .config key
        /// </summary>
        /// <param name="moduleName">Name of sub-part of application</param>
        /// <returns></returns>
        public LogMessageBuilder ForModule(string moduleName)
        {
            _message.ApplicationSubKey = moduleName;
            return this;
        }

        /// <summary>
        /// Add a name to a group.  Groups map to EntLib categories and can be used for log filtering
        /// </summary>
        /// <param name="groupName">Name of group (category)</param>
        /// <returns></returns>
        public LogMessageBuilder InGroup(string groupName)
        {
            _message.Categories.Add(groupName);
            return this;
        }

        /// <summary>
        /// Add a new value to the the ExtendedProperties KeyValue pair collection for this message
        /// </summary>
        /// <param name="key">Name for item.</param>
        /// <param name="value">Value to add.  Uses <paramref name="value"/>.ToString() to store value</param>
        /// <returns></returns>
        public LogMessageBuilder Collect(string key, object value) // extended properties
        {
            _message.Properties.Add(key, value.ToString());
            return this;
        }

        /// <summary>
        /// Store detailed exception information into message.  NOTE: If message text is not supplied and
        /// there is an <paramref name="ex"/> stored, then this message will use <paramref name="ex"/>.Message
        /// as the message text.  This makes logging generic exceptions very easy to do.
        /// </summary>
        /// <param name="ex">Hydrated exception to store</param>
        /// <returns></returns>
        public LogMessageBuilder Catch(Exception ex)
        {
            _message.AddException(ex);
            HttpException httpException = ex as HttpException;

            if (httpException != null)
            {
                _message.Properties.Add("HTTP_STATUS_CODE", httpException.GetHttpCode().ToString());
                _message.Properties.Add("HTTP_HTML_ERROR_MESSAGE", tryGetHtmlErrorMessage(httpException));
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks>From Elmah::Error.cs</remarks>
        private static string tryGetHtmlErrorMessage(HttpException e)
        {
            try
            {
                return e.GetHtmlErrorMessage();
            }
            catch (SecurityException se)
            {
                Trace.WriteLine(se);
                return null;
            }
        }

        /// <summary>
        /// Extract specific HTTP information and store it in message's HttpContext collection
        /// </summary>
        /// <param name="flags">Determines which information to collect from context</param>
        /// <returns></returns>
        public LogMessageBuilder CaptureHttp(HttpCaptureFlags flags)
        {
           // _message.HttpContext = WebUtils.ExpandCurrentContext(flags);
            return this;
        }

        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public LogMessageBuilder Reference(string reference, params object[] args)
        {
            _message.Reference = (object)string.Format(reference, args);
            return this;
        }

        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>.  Value is <paramref name="reference"/>.ToString()
        /// </summary>
        /// <returns></returns>
        public LogMessageBuilder Reference(object reference)
        {
            _message.Reference = (object)reference.ToString();
            return this;
        }

        /// <summary>
        /// Extract HTTP information and store it in message's HttpContext collection (Default: Url and Form variables).
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogMessageBuilder CaptureHttp()
        {
            return CaptureHttp(HttpCaptureFlags.Url | HttpCaptureFlags.Form);
        }

        /// <summary>
        /// Explictly set the title of message.  Used when overriding default title format.  May include standard String.Format() format specifications
        /// </summary>
        /// <param name="message">Text of title</param>
        /// <param name="args">Any arguments to supply to <paramref name="message"/></param>
        /// <returns></returns>
        public LogMessageBuilder TitleAs(string message, params object[] args)
        {
            _message.Title = TextUtils.StringFormat(message, args);
            return this;
        }

        /// <summary>
        /// Retrieve operating system and network settings (e.g. thread id, host name, IP addresses, process identifiers)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogMessageBuilder CaptureMachineContext()
        {
            _message.MachineContext = new MachineContext();
            return this;
        }

        /// <summary>
        /// Retrieve HTTP and Thread identity (names, etc)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogMessageBuilder CaptureIdentity()
        {
            if (_message.UserContext == null)
            {
                _message.UserContext = new System.Collections.Specialized.NameValueCollection();
            }
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
            {
                _message.UserContext.Add("HttpUser", string.Format("{0}, IsAuthenticated: {1}", HttpContext.Current.User.Identity.Name, HttpContext.Current.User.Identity.IsAuthenticated));
            }
            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null)
            {
                _message.UserContext.Add("ThreadUser", string.Format("{0}, IsAuthenticated: {1}", Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.IsAuthenticated));
            }
            return this;
        }

        /// <summary>
        /// Hide ToString() from Intellisense in client application
        /// </summary>
        /// <returns></returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Hide Equals() from Intellisense in client application
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Hide GetHashCode from Intellisense in client application
        /// </summary>
        /// <returns></returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
