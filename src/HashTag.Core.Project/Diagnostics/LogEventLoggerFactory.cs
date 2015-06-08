﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class LogEventLoggerFactory 
    {
        private static ConcurrentDictionary<string, Log> _dictionary = new ConcurrentDictionary<string, Log>();
        public static string _defaultLogName { get; set; }

        private static ILogEventProcessor _eventProcessor = new LogEventProcessor();


        /// <summary>
        /// Create a new log instance with application name.  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <returns></returns>
        public static IEventLogger NewLogger(SourceLevels allowedLogLevels =  SourceLevels.All)
        {
            if (string.IsNullOrEmpty(_defaultLogName) == true)
            {
                _defaultLogName = CoreConfig.ApplicationName;
            }

            return NewLog(_defaultLogName);
        }

        public static IEventLogger NewLogger<T>(SourceLevels allowedLevels = SourceLevels.All)
        {
            return NewLogger(typeof(T), allowedLevels);
        }

        /// <summary>
        /// Create a default logger using full name of type (pattern from log4Net)
        /// </summary>
        /// <param name="logLevels">List of levels (flags) of message severity to log</param>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public static IEventLogger NewLogger(Type type, SourceLevels logLevels = SourceLevels.All)
        {
            return NewLogger(type.FullName, logLevels);

        }


        /// <summary>
        /// Create a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:All)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>Created log</returns>
        public static IEventLogger NewLog(string logName)
        {
            return NewLogger(logName, CoreConfig.Log.ApplicationLogLevels);
        }

        /// <summary>
        /// Create a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns>Created log or cached instance if already created one in application domain</returns>
        public static IEventLogger NewLogger(string logName, SourceLevels allowedSourceLevels = SourceLevels.All)
        {
            if (_dictionary.ContainsKey(logName) == true)
            {
                _dictionary[logName]._logLevels = allowedSourceLevels;
                return _dictionary[logName];
            }
            else
            {
                Log log = new Log(logName, allowedSourceLevels)
                {
                    Write = _eventProcessor.Submit
                };
                _dictionary[logName] = log;
                return log;
            }
        }

        
        public static IEventLogger NewLogger(object logNameFromObjectsType,SourceLevels allowedSourceLevels=SourceLevels.All)
        {
            return NewLogger(logNameFromObjectsType.GetType(),allowedSourceLevels);
        }

    }

  
}