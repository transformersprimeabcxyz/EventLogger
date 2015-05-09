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
using HashTag.Text;


namespace HashTag.Diagnostics
{
    /// <summary>
    /// Base class for writing messages to persistant store.
    /// </summary>
    public sealed partial class Log : ILog
    {

        private string _logName;
        private static string _envName = CoreConfig.ActiveEnvironment;
        private static string _appName = CoreConfig.ApplicationName;
        private static string _machineName = Environment.MachineName;
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
        public LogMessageBuilder Critical
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Critical).CaptureHttp().CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Error' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public LogMessageBuilder Error
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Error).CaptureHttp().CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Warning' level message.  Also captures some key HttpContext, Identity, and Machine parameters.
        /// </summary>
        public LogMessageBuilder Warning
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Warning).CaptureHttp().CaptureIdentity().CaptureMachineContext();
            }
        }

        /// <summary>
        /// Build a 'Information' level message.
        /// </summary>
        public LogMessageBuilder Info
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Information);
            }
        }

        /// <summary>
        /// Build a 'Verbose' level message.
        /// </summary>
        public LogMessageBuilder Verbose
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Verbose);
            }
        }

        /// <summary>
        /// Build a 'Operation starting' message.
        /// </summary>
        public LogMessageBuilder Start
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Start);
            }
        }


        /// <summary>
        /// Build a 'Operation stopping' message.
        /// </summary>
        public LogMessageBuilder Stop
        {
            get
            {
                return newLogMessageBuilder(TraceEventType.Stop);
            }
        }

        private LogMessageBuilder newLogMessageBuilder(TraceEventType eventType)
        {
            var msg = new LogMessage();

            if (string.IsNullOrEmpty(_logName) == false)
            {
                msg.Categories.Add(_logName);
            }
            if (string.IsNullOrEmpty(_appName) == false && string.Compare(_appName, _logName) != 0)
            {
                msg.Categories.Add(_appName);
            }

            if (_envName != null && _envName.Length > 0)
            {
                msg.ActiveEnvironment = _envName;
            }

            msg.ActivityId = CoreConfig.ActivityId;
            msg.Severity = eventType; //also sets priority and default event id if either is 0
            msg.ApplicationKey = _appName;
            msg.LoggerName = _logName;
            msg.TimeStamp = DateTime.Now;

            return new LogMessageBuilder(msg, Write);
        }

        /// <summary>
        /// Persist message to logging sub-system.  Called by log message builder
        /// </summary>
        public void Write(LogMessage message)
        {
            //filter not supported message levels
            if (!_logLevels.IsEnabled(message.Severity)) return;

            _messageBuffer.SubmitMessage(message); //send message to internal async message cache and immediately return
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
        ~Log()
        {

            Dispose(false);
        }
    }
}