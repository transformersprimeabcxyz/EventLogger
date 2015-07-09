﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HashTag.Diagnostics.Models;
using Newtonsoft.Json;
using HashTag.Logging.Client.Configuration;

namespace HashTag.Diagnostics
{
    public class LoggerFactory
    {
        private static ConcurrentDictionary<string, Logger> _registeredLogs = new ConcurrentDictionary<string, Logger>();
        private static volatile bool isInitialized=false;
        public static string _defaultLogName { get; set; }

        private static LoggingOptions _options;
        public static LoggingOptions DefaultConfig {
            get
            {
                if (_options == null)
                {
                    _options = new LoggingOptions();
                }
                return _options;
            }
            set { _options = value; }
        }
        
        public static void Initialize(LoggingOptions config=null)
        {
            if (config != null)
            {
                DefaultConfig = (LoggingOptions)config.Clone();
            }
            else
            {
                DefaultConfig = new LoggingOptions();
            }
            isInitialized = true;
        }


        public static void EnsureInitialized()
        {
            if (isInitialized == false)
            {
                Initialize(DefaultConfig);
                isInitialized = true;                
            }
        }

        /// <summary>
        /// Create a new log instance with application name. 
        /// </summary>
        /// <returns></returns>
        public static IEventLogger NewLogger(SourceLevels allowedLogLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            if (string.IsNullOrEmpty(_defaultLogName) == true)
            {
                _defaultLogName = DefaultConfig.ApplicationName;
            }

            return NewLogger(_defaultLogName);
        }

        public static IEventLogger NewLogger<T>(SourceLevels allowedLevels = SourceLevels.All)
        {
            EnsureInitialized();    
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
            EnsureInitialized();    
            return NewLogger(type.FullName, logLevels);

        }


        /// <summary>
        /// Create a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:All)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>Created log</returns>
        public static IEventLogger NewLogger(string logName)
        {
            EnsureInitialized();    
            return NewLogger(logName, DefaultConfig.SourceLevels);
        }

        /// <summary>
        /// Create a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns>Created log or cached instance if already created one in application domain</returns>
        public static IEventLogger NewLogger(string logName, SourceLevels allowedSourceLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            if (_registeredLogs.ContainsKey(logName) == true)
            {
                _registeredLogs[logName]._logLevels = allowedSourceLevels;
                return _registeredLogs[logName];
            }
            else
            {
                Logger log = new Logger(logName, DefaultConfig);
                _registeredLogs[logName] = log;
                return log;
            }
        }

        public static IEventLogger NewLogger(object logNameFromObjectsType, SourceLevels allowedSourceLevels = SourceLevels.All)
        {

            return NewLogger(logNameFromObjectsType.GetType(), allowedSourceLevels);
        }
    }


}
