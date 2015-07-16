using HashTag.Diagnostics;
using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;
using System;
namespace HashTag.Logging.Client.Interfaces
{
    /// <summary>
    /// Represents building up a single log event for later persistance
    /// </summary>
    public interface IEventBuilder
    {
        /// <summary>
        /// Configuration options this builder should use when creating and presisting an event.  Automatically
        /// set by EventLogger factory but may be overrided for special cases
        /// </summary>
        EventOptions Config { get; set; }

        /// <summary>
        /// Capture all Http variables as set in Config (default: All) Auto-captured on Error level and more severe
        /// </summary>
        /// <returns></returns>
        IEventBuilder CaptureHttp();

        /// <summary>
        /// Capture all Http variables as set in <paramref name="flags"/>
        /// </summary>
        /// <param name="flags">Composite set of groups of flags to capture</param>
        /// <returns></returns>
        IEventBuilder CaptureHttp(HttpCaptureFlags flags);

        /// <summary>
        /// Capture various possible identity fields active within framework. Auto-captured on Error level and more severe
        /// </summary>
        /// <returns></returns>
        IEventBuilder CaptureIdentity();

        /// <summary>
        /// Capture details about this machine (OS, memory, etc.) and low level runtime context (threading info)
        /// </summary>
        /// <returns></returns>
        IEventBuilder CaptureMachineContext();

        /// <summary>
        /// Add an exception (recursive inner exceptions and exception public properties) to events list of errors.  Multiple calls result in a list of exceptions. Auto-captured on Error level and more severe
        /// </summary>
        /// <param name="ex">Starndard .Net exception</param>
        /// <returns></returns>
        IEventBuilder Catch(Exception ex);

        /// <summary>
        /// Add any key-value pair information to associate with event.  Often use for loop counters, object identifiers, start/stop statistics.
        /// </summary>
        /// <param name="key">Identifier for this key-value pair</param>
        /// <param name="value"><paramref name="value"/>.ToString() to store with key</param>
        /// <returns></returns>
        IEventBuilder Collect(string key, object value);

        /// <summary>
        /// Set's a sub-system identifier for this message (e.g. MyAccountSystem, Module=CheckPrintingService).  To set the 'Module'
        /// for all events, use the appropiate .config setting.  Uses .config setting if not set here.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        IEventBuilder ForModule(string moduleName);

        /// <summary>
        /// An identifer that is often used to correlate between several messages or used to identify a single record in a 
        /// multiple record process (e.g. record id, row id, filename, etc)
        /// </summary>
        /// <param name="reference"><paramref name="reference"/>.ToString()</param>
        /// <returns></returns>
        IEventBuilder Reference(object reference);

        /// <summary>
        /// An identifer that is often used to correlate between several messages or used to identify a single record in a 
        /// multiple record process (e.g. record id, row id, filename, etc)
        /// </summary>
        /// <param name="reference">Textual identifier used for reference</param>
        /// <param name="args">Any standard string.format arguments to supply to <paramref name="reference"/></param>
        /// <returns></returns>
        IEventBuilder Reference(string reference, params object[] args);

        /// <summary>
        /// Creates a textual event id for this message (e.g. 'AP3000').  Often used in code based team, or for 
        /// identifying a specific message (e.g. 'Account is overdrawn during final processing')
        /// </summary>
        /// <param name="code">Textual portion of the code</param>
        /// <param name="args">Any standard string.format arguments to supply to <paramref name="code"/></param>
        /// <returns></returns>
        IEventBuilder WithCode(string code, params object[] args);

        /// <summary>
        /// Creates a numerical event id for this message (e.g. 4030, Exception.HResult).  Often used for simplifing query in 
        /// event store or in ETW integration.  This value identifies the message (e.g. 'Account is overdrawn') not the instance of the message (Event.UUID).
        /// If not provided system will automatically set one based on EventType (Error, Warning, Verbose) and EventPriority (High, Low, Lower, Highest)
        /// </summary>
        /// <param name="eventId">Value to associate with this message</param>
        /// <returns></returns>
        IEventBuilder WithId(int eventId);

        /// <summary>
        /// Sets how urgent the caller considers this message.  System will determine default priority based on EventType (e.g. Error-High)
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IEventBuilder WithPriority(LogEventPriority priority);
        

        /// <summary>
        /// Send an event to the configured log event store. This must be called at the end of any _log.[Level] fluent chain
        /// </summary>
        /// <param name="ex">Exception to add to message before writing.  Short-cut for .Catch(ex) if all you are doing is logging exception</param>
        /// <param name="message">Text to add to event.  If not, and <paramref name="ex"/> is specified, use <paramref name="ex"/>.Message as event's message</param>
        /// <param name="args">Any standard string.format() arguments to supply to message</param>
        /// <returns>A copy of the actual event that the build sent to the configured log store</returns>
        LogEvent Write(Exception ex, string message = null, params object[] args);

        /// <summary>
        /// Send an event to the configured log event store. This must be called at the end of any _log.[Level] fluent chain
        /// </summary>
        /// <param name="messageData">Event's message will be <paramref name="messageData"/>.ToString()</param>
        /// <returns></returns>
        LogEvent Write(object messageData = null);

        /// <summary>
        /// Send an event to the configured log event store. This must be called at the end of any _log.[Level] fluent chain
        /// </summary>
        /// <param name="message">Text that becomes the event's message</param>
        /// <param name="args">Any standard string.format() arguments to supply to message</param>
        /// <returns></returns>
        LogEvent Write(string message, params object[] args);



    }
}
