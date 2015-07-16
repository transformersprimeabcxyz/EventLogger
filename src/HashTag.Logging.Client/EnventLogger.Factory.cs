using System;
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
    public partial class EventLogger
    {
        private static ConcurrentDictionary<string, EventLogger> _registeredLogs = new ConcurrentDictionary<string, EventLogger>();
        private static volatile bool isInitialized=false;
        public static string _defaultLogName { get; set; }

        private static EventOptions _options;

        /// <summary>
        /// Sets the configuration options that will be passed to each event builder
        /// </summary>
        public static EventOptions DefaultConfig {
            get
            {
                if (_options == null)
                {
                    _options = new EventOptions();
                }
                return _options;
            }
            set { _options = value; }
        }
        
        /// <summary>
        /// Set/reset configuration options for subsequent event log creation.  Usually done very early on in application boostrapping
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(EventOptions config=null)
        {
            if (config != null)
            {
                DefaultConfig = (EventOptions)config.Clone();
            }
            else
            {
                DefaultConfig = new EventOptions();
            }
            isInitialized = true;
        }

        /// <summary>
        /// If configuration is dirty then apply configuration.
        /// </summary>
        public static void EnsureInitialized()
        {
            if (isInitialized == false)
            {
                Initialize(DefaultConfig);
                isInitialized = true;                
            }
        }

        /// <summary>
        /// GetLogger a new log instance with application name. 
        /// </summary>
        /// <returns></returns>
        public static IEventLogger GetLogger(SourceLevels allowedLogLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            if (string.IsNullOrEmpty(_defaultLogName) == true)
            {
                _defaultLogName = DefaultConfig.ApplicationName;
            }

            return GetLogger(_defaultLogName);
        }

        /// <summary>
        /// Returns a logger that is named the fully qualified type name of T
        /// </summary>
        /// <typeparam name="T">Type from which to get logger name</typeparam>
        /// <param name="allowedLevels">Lowest severity created log will emit events to event store</param>
        /// <returns></returns>
        public static IEventLogger GetLogger<T>(SourceLevels allowedLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            return GetLogger(typeof(T), allowedLevels);
        }

        /// <summary>
        /// GetLogger a default logger using full name of type (pattern from log4Net)
        /// </summary>
        /// <param name="logLevels">List of levels (flags) of message severity to log</param>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public static IEventLogger GetLogger(Type type, SourceLevels logLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            return GetLogger(type.FullName, logLevels);

        }


        /// <summary>
        /// GetLogger a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:All)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>GetLoggerd log</returns>
        public static IEventLogger GetLogger(string logName)
        {
            EnsureInitialized();    
            return GetLogger(logName, DefaultConfig.SourceLevels);
        }

        /// <summary>
        /// GetLogger a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns>GetLoggerd log or cached instance if already GetLoggerd one in application domain</returns>
        public static IEventLogger GetLogger(string logName, SourceLevels allowedSourceLevels = SourceLevels.All)
        {
            EnsureInitialized();    
            if (_registeredLogs.ContainsKey(logName) == true)
            {
                _registeredLogs[logName]._logLevels = allowedSourceLevels;
                return _registeredLogs[logName];
            }
            else
            {
                EventLogger log = new EventLogger(logName, DefaultConfig);
                _registeredLogs[logName] = log;
                return log;
            }
        }

        /// <summary>
        /// Get a new logger based on the type of <paramref name="logNameFromObjectsType"/>
        /// </summary>
        /// <param name="logNameFromObjectsType">Object to get logger name for</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns></returns>
        public static IEventLogger GetLogger(object logNameFromObjectsType, SourceLevels allowedSourceLevels = SourceLevels.All)
        {
            return GetLogger(logNameFromObjectsType.GetType(), allowedSourceLevels);
        }
    }


}
