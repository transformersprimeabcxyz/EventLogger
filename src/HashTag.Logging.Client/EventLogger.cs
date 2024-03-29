﻿/**
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
using HashTag.Diagnostics.Models;


namespace HashTag.Diagnostics
{
    /// <summary>
    /// Base class for writing messages to persistant store.
    /// </summary>
    public sealed partial class EventLogger : IEventLogger
    {

        private string _logName;

        private EventOptions _loggerConfig;

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
        internal EventLogger(string logName, EventOptions config)
        {
            _logName = logName;
            _loggerConfig = (EventOptions)config.Clone();
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
        public IEventBuilder Critical
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Critical).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Error' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public IEventBuilder Error
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Error).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Warning' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public IEventBuilder Warning
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Warning).CaptureHttp(_loggerConfig.OnErrorHttpCaptureFlags).CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Information' level message.
        /// </summary>
        public IEventBuilder Info
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Information).CaptureIdentity();
            }
        }

        /// <summary>
        /// Build a 'Verbose' level message.
        /// </summary>
        public IEventBuilder Verbose
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Verbose).CaptureIdentity();
            }
        }

        /// <summary>
        /// Build a 'Operation starting' message.
        /// </summary>
        public IEventBuilder Start
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Start).CaptureIdentity();
            }
        }


        /// <summary>
        /// Build a 'Operation stopping' message.
        /// </summary>
        public IEventBuilder Stop
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Stop).CaptureIdentity();
            }
        }

        private IEventBuilder newLogMessageBuilder(TraceEventType eventType)
        {
            var evtBuilder = new LogEventBuilder(_loggerConfig, _logName)
            {
               
            };
            
            return evtBuilder.AsEventType(eventType);
        }

        /// <summary>
        /// Writes a hydrated event to configured event store
        /// </summary>
        /// <param name="evt">Event to persist to logger's event store</param>
        public void Write(LogEvent evt)
        {
            _loggerConfig.EventStoreConnector.Submit(evt);
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
        ~EventLogger()
        {

            Dispose(false);
        }
    }
}