using HashTag.Diagnostics;
using HashTag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HashTag.MVC.Elmah.RestClientDemo
{
    /*
     Developer centric objectives
     
     * Quickly get logging working in an application
     * KISS for common logging use-cases
     * Consistent message types for events
     * Resonable defaults
     * Consistent event id's so they can be querable in database, grep, etc.
     * Helpers to simplify building messages (e.g. no string.format)
     * Expose "hidden" exception details not exposed by ToString()
     * Flexible enough for most logging and instrumentation scenarios
     
     Application centric objectives
     
     * Logging subsystem does not slow down application (internal multi-threaded block-based queues)
     * Zero-configuration creates low barrier to entry
     * Correlate messages within call-stack
     * Correlate messages across load balancers
     * Serializable exceptions
     * Integration with existing config keys (e.g. environment name, application name)
     * Auto-collect critical system properties on exceptional messages
     * Default to use standard .Net Diagnostics trace system (BTW same system entlib uses)
     * Uses default standard .Net diagnostics trace configuration
     * Supports mocking for automated testing scenarios
     * No application impact on sub-system failures
    
     Architectural centric objectives
     
     * Uses multi-threaded asynchronous block-based buffer to improve efficiency across slow connections (e.g. database, REST/WCF services, syslog)
     * OnWriteFailure alternate logging pipeline
     * Supports three persistence channels (application/primary, local/backup, internal) each independently configured
     * Operational configuration (e.g. buffer size, default connector, etc.) controlled by .config file and/or compiled properties depending on team preferences
     * Replace internal .Net TraceSource connector with 3rd party connector (write natively to log4Net, etc).
     * Simplified log event control.  More complex scenarios should filter through 3rd party logger
     * Default extension is through standard custom .Net TraceListeners
     * LogBuilder can be married to a different logging subsystem
     * Flexible implementation (via .Net trace source configuration)
      
     ELMAH & HashTag.Logging
     
     ELMAH collects unhandled exceptions from a web-based applications.
            "unhandled"    -    if you app detects and recovers from an exception logs have no knowledge of it
            "exceptions"   -    while possible, it's not generally a best-practice to log non-exception text
                           -    exceptions are ToString() thus may mask important properties not normally exposed
            "web-based"    -    ELMAH is implemented as a handler.  Does not support non-web apps (e.g. console, WinForms, Java, etc.)
     ELMAH  module is bi-directional while System.Diagnostics is one-way
     ELMAH  captures a sub-set (but the most important sub-set) of properties HT.Logging does
     ELMAH  defaults to direct database/text file store
            ELMAH error messages are not natively JSON serializable to .Net WebAPI
     ELMAH  does not support fail-over writing
     
     There is a place for both systems in a standard application development instrumentation ecosystem.
     
     HT.Log events on several levels (not just Error)
           free form messages
           expanded exception properties
           collects superset of contextual properties on exceptional messages
           functions on web, winforms, console apps
           supports multi-message/multi-stack correlation
     
     ELMAH, HashTag.Logging & Centralized Store
     
     HT.Logging.ELMAH.dbProxy reads and writes 
        records directly from database (ala traditional ELMAH) except database
        is in HT.LogEvent format.  The application is not 
        aware it is not interacting with traditional ELMAH DBMS.
        (Database connection string in .config)
     
     HT.Logging.ELMAH.WebProxy reads and writes JSON to a HashTag.Logging.EventServer.
        Only EventServer HTTP end-point in .config without any credentials
     
     TBD? Can ELMAH have more than one connection stored?
     
     Things To Collect On Message
 
.WithId	    Identifier of this message (e.g. '101', '34334').  Used in IT departments where there is a preference to log by number instead of by category and/or name. If not provided system will use a combination of Severity+Priority Helpful in searching and filtering log store for a specific type of message
.WithCode	Textual (N09099, AP3933) or numeric (9932, 5321) that identifies this message. Used in environments where there are codes that represent messages.
.ForModule	On complex systems all log messages might be sub-classed (e.g. BatchProcessor1)	
.InGroup	One or more names to group message by (one message might be in group database, A/P, webservice)	Allows for groups and searching types of messages together
.Collect	Any contextual information provided to make message more understandable (e.g method parameters, record ids)	
.Catch	    Store detailed exception information into message. Automatically expands all inner exceptions, ex.Data collection, and any public properties not usually exposed by .ToString	
.CaptureHttp	Extract specific HTTP information and store it in message's HttpContext collection. Automatically called on Exceptional messages
.Reference	Simple data point to help identify context in message.	Often a record id from database so log reader knows the record triggering event
.CaptureMachineContext	Load a standard set of machine settings into message.	Automatically called on exceptional messages
.CaptureIdentity	Collect standard set of different identity points(e.g. thread, HttpContext.User, Application.User, etc)	

 LogEvent Core Properties (R) required (*) recommended (A) auto for all messages (E) auto-collected on exceptional messages
    (R) TimeStamp (string)          (A)
    (*) Severity (string)           (A)
    (*) ActiveEnvironment           (A)
    (R) ApplicationKey              (A)
    (R) MessageText                 (A/inferred)
    (*) MachineName		            (A)
    (*) [UserContext.Default]	    (A)
    [Exception.Message]             (E)
    [Exception.Type]                (E)
    [Exception.Source]              (E)
    [Exception.base.Message]        (E)
    [Exception.base.Type]           (E)
    [Exception.base.Source]         (E)
    UserReference (object)          
      
 Log Event Supplemental - HTTP
    ServerVariables                 (E)
    QueryString                     (E)
    Cookies                         (E)
    Form                            (E)
    Header                          (E)

Log Event Supplemental - User Context
    ThreadPrincipal                 (E)
    HttpUser                        (E)
    EnvUserName                     (E)
    UserDomain                      (E)
    AppDomainIdentity               (E)
    IsInteractive                   (E)
    DefaultUser                     (E/inferred)
     
Log Event Supplemental 
    ActivityId (guid)	            (A)
    Categories
    EventId (numeric/inferred)      (A)    
    EventCode
    Priority (string)
    PriorityValue (numeric)
    SeverityValue (numeric)
    LoggerName                      (A)
    ApplicationSubKey
    UUID                            (A)
    Title                           (A)
  
 Log Event Supplemental - Exceptions (List)
    HttpEventCode                   (E)
    HttpStatusValue                 (E)
    HttpStatusCode                  (E)
    HttpHtmlMessage                 (E)
    Properties                      (E)
    InnerException                  (E)
    MessageText                     (E)
    Source                          (E)
    StackTrace                      (E)
    HelpLink                        (E)
    TargetSite                      (E)
    Data                            (E)
    ErrorCode                       (E)
    ExceptionType                   (E)
  
Log Event Supplemental - Machine Context
    CommandLine                     (E)
    Is64BitOperatingSystem          (E)
    Is64BitProcess                  (E)
    OsVersion                       (E)
    ProcessorCount                  (E)
    AppFolder                       (E)
    DomainConfigFile                (E)
    DomainAppName                   (E)
    DomainId                        (E)
    DomainCtxIdentity               (E)
    DomainAppIdentity               (E)
    DomainAssmVersion               (E)
    DomainAssmName                  (E)
    HostName                        (E)
    AppDomainName                   (E)
    ProcessId                       (E)
    ProcessName                     (E)
    ManagedThreadName               (E)
    Win32ThreadId                   (E)
    StackTrace                      (E)
    WorkingMemoryBytes              (E)
    IPAddressList                   (E)
    ClassName                       (E)
    MethodName                      (E)
 
  */
    public class LogDemo
    {
        IEventLogger _log = LogEventLoggerFactory.NewLogger<LogDemo>();

        public void DoSomething()
        {
            _log.Verbose.Write("remember something");
        }

        public void SimpleMessages(string sourceFile, Guid recordId)
        {
            try
            {
                _log.Info.Write("Uploading file {0}", sourceFile);
            }
            catch (Exception ex)
            {
                _log.Error.Write(ex);
                throw;
            }
        }

        public void ExceptionWithCustomText(string sourceFile, Guid recordId)
        {
            int lineNumber = 0;
            try
            {
                _log.Warning.Write("Uploading file {0}", sourceFile);
            }
            catch (Exception ex)
            {
                _log.Error.Catch(ex).Write("Error happened on line {0}",lineNumber);
            }

        }

        public void MessageWithAdditionalProperties(string sourceFile, Guid recordId)
        {
            int lineNumber = 0;
            try
            {
                _log.Verbose
                    .Collect("Field12","some data")
                    .Write("Uploading file {0}", sourceFile);
            }
            catch (Exception ex)
            {
                _log.Error.Catch(ex).Write("Error happened on line {0}", lineNumber);
            }

        }
        public void ExceptionWithAdditionalData(string sourceFile, Guid recordId)
        {
            try
            {
                _log.Critical
                    .ForModule("MilkShakeMixer")
                    .CaptureHttp(HttpCaptureFlags.Url|HttpCaptureFlags.Session)
                    .Write("Oreo's are gone!");
            }
            catch(Exception ex)
            {
                ex.Data.Add("sourceFile", sourceFile); //native exception data
                ex.AddData("recordId", recordId); //HashTag extension for readability, and string.format capability

                _log.Error.Catch(ex).Write();
            }
        }
        public void MessagesWithMetaData(string sourceFile, Guid recordId)
        {
            try
            {
                _log.Start.Reference(recordId).Write();
                //do something very long here
                _log.Stop
                    .Reference(recordId)
                    .Collect("Total Records", 2323)
                    .Collect("Elapsed Time", 123)
                    .Write("Finished processing file at: {0:HH:mm:ss.fff}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _log.Error
                    .Reference(recordId)
                    .Collect("sourceFile",sourceFile)
                    .Collect("recordId",recordId)
                    .Catch(ex).Write();
            }
        }
        public void MessageWithFormalIdentifier(string sourceFile, Guid recordId)
        {
                _log.Warning.WithCode("AP3030").Reference(recordId).Write();
        }
    }


    public abstract class MyBaseClass
    {
        private IEventLoggerFactory _factory;
        private IEventLogger __internalLog;
        protected IEventLogger _log
        {
            get
            {
                if (__internalLog != null) return __internalLog;
                __internalLog = _factory.NewLogger(this);
                return __internalLog;
            }
        }
        public MyBaseClass()
        {
          //  _factory = new LogEventLoggerFactory();
        }
        public MyBaseClass(IEventLoggerFactory logFactory)
        {
            _factory = logFactory;
        }

    }
    
    public class LogDemoWithBase : MyBaseClass
    {
        public void DoSomething()
        {
            _log.Verbose.Write("remember something");
        }

        public void SimpleMessages(string sourceFile, Guid recordId)
        {
            try
            {
                _log.Info.Write("Uploading file {0}", sourceFile);
            }
            catch (Exception ex)
            {
                _log.Error.Write(ex.ToString());
            }
        }
    }

    public class LogDemoWithBaseAndDI : MyBaseClass
    {
        /// <summary>
        /// Injected logger. Can be used with automated testing scenarios
        /// </summary>
        /// <param name="log"></param>
        public LogDemoWithBaseAndDI(IEventLoggerFactory logFactory)
            : base(logFactory)
        {

        }

        /// <summary>
        /// Injected parameters with no injected logger
        /// </summary>
        /// <param name="injectedProperty"></param>
        public LogDemoWithBaseAndDI(IAppDomainSetup injectedProperty):base()
        {

        }
    }

}