/**
/// HashTag.Core Library
/// Copyright © 2005-2012
**/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using HashTag.Logging.Client.Configuration;
using HashTag.Logging.Client.Interfaces;


namespace HashTag.Diagnostics
{
    /// <summary>
    /// Base class for writing messages to persistant store.
    /// </summary>
    public sealed partial class Logger : IEventLogger
    {

        private string _logName;

        private LoggingOptions _loggerConfig;

        private volatile static bool _isInitialized = false;
        internal SourceLevels _logLevels { get; set; }

        /// <summary>
        /// Initialize global (i.e. static) fields for logging subsystem
        /// </summary>
        private static void initialize()
        {
            _isInitialized = true;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logName"></param>
        internal Logger(string logName, LoggingOptions config)
        {
            _logName = _logName;
            _loggerConfig = (LoggingOptions)config.Clone();
        }


        /// <summary>
        /// Name of this particular log.
        /// </summary>
        public string LogName
        {
            get { return _logName; }
            set { _logName = value; }
        }

        /// <summary>
        /// Build a 'Critical' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public ILogEventBuilder Critical
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Critical).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Error' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public ILogEventBuilder Error
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Error).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Warning' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public ILogEventBuilder Warning
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Warning).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Information' level message.
        /// </summary>
        public ILogEventBuilder Info
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Information);
            }
        }

        /// <summary>
        /// Build a 'Verbose' level message.
        /// </summary>
        public ILogEventBuilder Verbose
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Verbose);
            }
        }

        /// <summary>
        /// Build a 'Operation starting' message.
        /// </summary>
        public ILogEventBuilder Start
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Start);
            }
        }


        /// <summary>
        /// Build a 'Operation stopping' message.
        /// </summary>
        public ILogEventBuilder Stop
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Stop);
            }
        }

        private ILogEventBuilder newLogMessageBuilder(TraceEventType eventType)
        {
            var evtBuilder = new LogEventBuilder(_loggerConfig)
            {

            };
            
            return null;
        }


        private bool _isDisposed = false;
        /// <summary>
        /// Close log and flush any buffered log messages
        /// </summary>
        /// <param name="isDisposing">True if being called from explict Dispose().  False if being called from destructor</param>
        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true && _isDisposed == false)
            {

                _isDisposed = true;
            }
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Logger()
        {

            Dispose(false);
        }
    }
}