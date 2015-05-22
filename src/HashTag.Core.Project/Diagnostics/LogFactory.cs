using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class LogFactory : ILogFactory
    {
        private static ConcurrentDictionary<string, Log> _dictionary = new ConcurrentDictionary<string, Log>();
        public static string _defaultLogName { get; set; }
        /// <summary>
        /// Create a new log instance with application name.  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <returns></returns>
        public ILog NewLog(SourceLevels allowedLogLevels =  SourceLevels.All)
        {
            if (string.IsNullOrEmpty(_defaultLogName) == true)
            {
                _defaultLogName = CoreConfig.ApplicationName;
            }

            return NewLog(_defaultLogName);
        }


        /// <summary>
        /// Create a default logger using full name of type (pattern from NHibernate)
        /// </summary>
        /// <param name="logLevels">List of levels (flags) of message severity to log</param>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public ILog NewLog(Type type, SourceLevels logLevels=SourceLevels.All)
        {
            return NewLog(type.FullName, logLevels);

        }


        /// <summary>
        /// Create a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:All)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>Created log</returns>
        public ILog NewLog(string logName)
        {
            return NewLog(logName, CoreConfig.Log.ApplicationLogLevels);
        }

        /// <summary>
        /// Create a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns>Created log or cached instance if already created one in application domain</returns>
        public ILog NewLog(string logName, SourceLevels allowedSourceLevels=SourceLevels.All)
        {
            if (_dictionary.ContainsKey(logName) == true)
            {
                _dictionary[logName]._logLevels = allowedSourceLevels;
                return _dictionary[logName];
            }
            else
            {
                Log log = new Log(logName, allowedSourceLevels);
                _dictionary[logName] = log;
                return log;
            }
        }

        private static ILogFactory _logFactory;
        public static ILogFactory Create
        {
            get
            {
                if (_logFactory == null)
                {
                    _logFactory = new LogFactory();
                }
                return _logFactory;
            }
        }


        public virtual ILog NewLog(object logNameFromObjectsType,SourceLevels allowedSourceLevels=SourceLevels.All)
        {
            return NewLog(logNameFromObjectsType.GetType(),allowedSourceLevels);
        }

    }

    public class LogFactory<T> :LogFactory,ILogFactory<T>
    {

        public virtual new ILog NewLog()
        {
            return base.NewLog(typeof(T));
        }

        public virtual new ILog NewLog(string logName)
        {
            return base.NewLog(logName);
        }

        public virtual new ILog NewLog(Type logNameFromType)
        {
            return base.NewLog(logNameFromType);
        }
        public virtual new ILog NewLog(object logNameFromObjectsType)
        {
            return base.NewLog(logNameFromObjectsType);
        }
    }
}
