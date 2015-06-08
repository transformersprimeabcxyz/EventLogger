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
using HashTag.Collections;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Facade to set values on the underlying message to be persisted to log
    /// </summary>
    public class LogEventBuilder
    {
        Func<LogEvent,Guid> _writeAction;
        LogEvent _message;

        internal LogEventBuilder(LogEvent messageToBuild, Func<LogEvent,Guid> writeAction)
        {
            _writeAction = writeAction;
            _message = messageToBuild;
        }

        /// <summary>
        /// Persist all built up properties to persistent store.  This MUST be called for peristance to take place and MUST be last method in fluent chain
        /// </summary>
        /// <param name="message">Message text of log message.  May include any standard string.format paramters</param>
        /// <param name="args">Any argument to supply to <paramref name="message"/></param>
        public LogEvent Write(string message, params object[] args)
        {
            _message.MessageText = TextUtils.StringFormat(message, args);
            if (_writeAction != null)
            {
                _message.Fix();
                _writeAction(_message);
            }
            return _message;
        }

        /// <summary>
        /// Persist an object to log.  Uses <paramref name="messageData"/>.ToString() to serialize the message.   This MUST be called for peristance to take place and MUST be last method in fluent chain
        /// </summary>
        /// <param name="messageData">Data to be persisted to log.</param>
        public LogEvent Write(object messageData = null)
        {
            
            if (messageData != null)
            {
                if (messageData is Exception)
                {
                    return Catch(messageData as Exception).Write();
                }
                Write(messageData.ToString());
            }
            else
            {
                _message.Fix();
                if (_writeAction != null)
                {
                    _writeAction(_message);
                }
            }
            return _message;
        }

        public LogEvent Write(Exception ex, string message=null, params object[] args)
        {
            Catch(ex);
            if (string.IsNullOrWhiteSpace(message))
            {
                return Write((object)null);
            }
            else
            {
                return Write(message, args);
            }
        }
        
        /// <summary>
        /// Returns a reference to message being constructed by builder.  Conceptually similar to StringBuilder.ToString() except exposes actual object instead of a copy
        /// </summary>
        /// <param name="message">Message text of log message.  May include any standard string.format paramters</param>
        /// <param name="args">Any argument to supply to <paramref name="message"/></param>
        public LogEvent  Message(string message, params object[] args)
        {
            _message.MessageText = TextUtils.StringFormat(message, args);
            
            return _message;
        }

        /// <summary>
        /// Returns a reference to message being constructed by builder.  Conceptually similar to StringBuilder.ToString() except exposes actual object instead of a copy
        /// </summary>
        /// <param name="messageData">Data to be persisted to log.</param>
        public LogEvent Message(object messageData = null)
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
        public LogEventBuilder WithId(int eventId)
        {
            _message.EventId = eventId;
            return this;
        }
            
        /// <summary>
        /// Textual (N09099, AP3933) or numeric (9932, 5321) that identifies this message.  These codes, if used, should be unique within a Source but might be unique across several sources.  Message Codes uniquely identify a particular event. Each event source can define its own numbered events 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public LogEventBuilder WithCode(string code, params object[] args)
        {
            _message.EventCode = string.Format(code, args);
            return this;
        }

        /// <summary>
        /// Set sub application key.  The application key is set in HashTag.Application.Name .config key
        /// </summary>
        /// <param name="moduleName">Name of sub-part of application</param>
        /// <returns></returns>
        public LogEventBuilder ForModule(string moduleName)
        {
            _message.ApplicationSubKey = moduleName;
            return this;
        }

        /// <summary>
        /// Add a name to a group (e.g. database, A/P). May have more than one
        /// </summary>
        /// <param name="groupName">Name of group (category)</param>
        /// <returns></returns>
        public LogEventBuilder InGroup(string groupName)
        {
            _message.Categories.Add(groupName);
            return this;
        }

        /// <summary>
        /// Add a new value to the KeyValue pair collection for this message
        /// </summary>
        /// <param name="key">Name for item.</param>
        /// <param name="value">Value to add.  Uses <paramref name="value"/>.ToString() to store value</param>
        /// <returns></returns>
        public LogEventBuilder Collect(string key, object value) // extended properties
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
        public LogEventBuilder Catch(Exception ex)
        {
            _message.AddException(ex);
           
            return this;
        }

        public LogEventBuilder Fix()
        {
            _message.Fix();
            return this;
        }
    

        /// <summary>
        /// Extract specific HTTP information and store it in message's HttpContext collection.
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <param name="flags">Determines which information to collect from context</param>
        /// <returns></returns>
        public LogEventBuilder CaptureHttp(HttpCaptureFlags flags)
        {
            _message.HttpContext = new LogHttpContext(HttpContext.Current, flags);
            return this;
        }

        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public LogEventBuilder Reference(string reference, params object[] args)
        {
            _message.Reference = (object)string.Format(reference, args);
            return this;
        }

        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>.  Value is <paramref name="reference"/>.ToString()
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder Reference(object reference)
        {
            _message.Reference = (object)reference.ToString();
            return this;
        }

        /// <summary>
        /// Extract HTTP information and store it in message's HttpContext collection (Default: Url and Form variables).
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureHttp()
        {
            return CaptureHttp(HttpCaptureFlags.All);
        }

        /// <summary>
        /// Explictly set the title of message.  Used when overriding default title format.  May include standard String.Format() format specifications
        /// </summary>
        /// <param name="message">Text of title</param>
        /// <param name="args">Any arguments to supply to <paramref name="message"/></param>
        /// <returns></returns>
        public LogEventBuilder TitleAs(string message, params object[] args)
        {
            _message.Title = TextUtils.StringFormat(message, args);
            return this;
        }

        /// <summary>
        /// Retrieve operating system and network settings (e.g. thread id, host name, IP addresses, process identifiers)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureMachineContext()
        {
            _message.MachineContext = new LogMachineContext();
            return this;
        }

        /// <summary>
        /// Retrieve HTTP and Thread identity (names, etc)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureIdentity()
        {
            _message.UserContext = new LogUserContext();
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
