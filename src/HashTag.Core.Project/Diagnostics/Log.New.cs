/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Log
    {
        private static ConcurrentDictionary<string, Log> _dictionary = new ConcurrentDictionary<string, Log>();
        private static string _defaultKey;
        private static LogMessageBuffer _messageBuffer;
        private volatile static bool _isInitialized = false;

        /// <summary>
        /// Initialize global (i.e. static) fields for logging subsystem
        /// </summary>
        private static void initialize()
        {
            if (_messageBuffer != null)
            {
                _messageBuffer.Dispose();
                _messageBuffer = null;
            }
            _messageBuffer = new LogMessageBuffer();
            _messageBuffer.PersistMessageHandler = saveBlockOfMessages;

            _isInitialized = true;
        }

        /// <summary>
        /// The message buffer is sending a block of messages to be persisted
        /// </summary>
        /// <param name="logMessageBlock"></param>
        private static void saveBlockOfMessages(List<LogMessage> logMessageBlock)
        {
            var persistTask = Task.Factory.StartNew(() =>
                {
                    var logSourceLevels = CoreConfig.Log.ApplicationLogLevels;
                    logMessageBlock.RemoveAll(msg => !logSourceLevels.IsEnabled(msg.Severity));
                    if (logMessageBlock.Count > 0)
                    {
                        try
                        {
                            CoreConfig.Log.ActiveConnector.PersistMessages(logMessageBlock);
                        }
                        catch (Exception ex)
                        {
                            //TODO handle persister exception here
                        }
                    }
                });
            persistTask.Wait(CoreConfig.Log.MessageBufferWriteTimeoutMs);
        }



        /// <summary>
        /// Create a new log instance with application name.  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <returns></returns>
        public static ILog NewLog()
        {
            if (string.IsNullOrEmpty(_defaultKey) == true)
            {
                _defaultKey = CoreConfig.ApplicationName;
            }

            return NewLog(_defaultKey);
        }

        /// <summary>
        /// Create a default logger using full name of type (pattern from NHibernate).  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public static ILog NewLog(Type type)
        {
            return NewLog(type.FullName);
        }

        /// <summary>
        /// Create a default logger using full name of type (pattern from NHibernate).  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <param name="logLevels">List of levels (flags) of message severity to log</param>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public static ILog NewLog(Type type, SourceLevels logLevels)
        {
            return NewLog(type.FullName, logLevels);

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logName"></param>
        private Log(string logName)
        {
            _logName = logName;
        }

        /// <summary>
        /// Create a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>Created log</returns>
        public static ILog NewLog(string logName)
        {
            return NewLog(logName, CoreConfig.Log.ApplicationLogLevels);
        }

        private SourceLevels _logLevels;

        /// <summary>
        /// Create a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="logLevels">Amount of logging this log will do</param>
        /// <returns>Created log or cached instance if already created one in application domain</returns>
        public static ILog NewLog(string logName, SourceLevels logLevels)
        {
            if (_dictionary.ContainsKey(logName) == true)
            {
                _dictionary[logName]._logLevels = logLevels;
                return _dictionary[logName];
            }
            else
            {
                Log log = new Log(logName, logLevels);
                _dictionary[logName] = log;
                return log;
            }
        }

        private Log()
        {
            if (!_isInitialized)
            {
                initialize();
            }


            _logLevels = CoreConfig.Log.ApplicationLogLevels;
        }

        private Log(string sourceName, SourceLevels levels)
            : this()
        {
            LogName = sourceName;
            _logLevels = levels;
        }

    }
}