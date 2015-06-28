using HashTag.Diagnostics;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using HashTag.Logging.Client.TraceSource.Extensions;
using System.Collections.Generic;
namespace HashTag.Logging.Client.Configuration
{
    public class LoggingOptions : ICloneable
    {
        private static string _hostName = Environment.MachineName;

        public LoggingOptions()
        {
            HostName = _hostName;
            OnErrorHttpCaptureFlags = HttpCaptureFlags.All;
            SourceLevels = SourceLevels.All;
            ConfigKeys = new Keys();
        }
        public LoggingOptions(Keys applicationKeys)
            : this()
        {
            ConfigKeys = applicationKeys;
        }
        public LoggingOptions(string branding):this()
        {
            ConfigKeys = new Keys(branding);
        }

        public class Keys
        {
            public Keys(string branding)
            {
                BrandingPrefix = branding;

                ApplicationName = string.Format("{0}.Application.Name", branding);
                ActiveEnvironment = string.Format("{0}.Application.Environment", branding);
                ConnectorType = string.Format("{0}.Logging.ConnectorType", branding);
                SourceLevels = string.Format("{0}.Logging.DefaultSourceLevels", branding);
                HttpCaptureFlags = string.Format("{0}.Logging.HttpCaptureFlags", branding);
            }
            public Keys()
                : this("HashTag")
            {

            }
            public string ApplicationName { get; set; }
            public string ActiveEnvironment { get; set; }
            public string ConnectorType { get; set; }
            public string SourceLevels { get; set; }
            public string HttpCaptureFlags { get; set; }
            public string BrandingPrefix { get; set; }

            public List<string> SupportedKeys
            {
                get
                {
                    return new List<string>()
                    {
                        ApplicationName,
                        ActiveEnvironment,
                        ConnectorType,
                        SourceLevels,
                        HttpCaptureFlags
                    };
                }
            }

        }

        public Keys ConfigKeys { get; set; }

        public const bool IGNORECASE_FLAG = false;

        [JsonIgnore]
        public string HostName { get; set; }

        private ILogStoreConnector _connector = null;
        private object connectorLock = new object();

        [JsonIgnore]
        public ILogStoreConnector LogConnector
        {
            get
            {
                if (_connector != null)
                {
                    return _connector;
                }
                lock (connectorLock)
                {
                    if (_connector != null)
                    {
                        return _connector;
                    }

                    var connectorType = findConnectorType();

                    _connector = HashTag.Reflection.ProviderFactory<ILogStoreConnector>.GetInstance(connectorType);
                }
                return _connector;
            }
            set
            {
                lock (connectorLock)
                {
                    if (_connector != null)
                    {
                        _connector.Stop();
                        _connector.Flush();
                        _connector = null;
                    }
                    _connector = value;
                }
            }
        }

        private string findConnectorType()
        {
            if (!string.IsNullOrWhiteSpace(this.ConnectorType))
            {
                return this.ConnectorType;
            }

            if (ConfigKeys != null &&
                !string.IsNullOrWhiteSpace(ConfigKeys.ConnectorType) &&
                !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[ConfigKeys.ConnectorType])
            )
            {
                return ConfigurationManager.AppSettings[ConfigKeys.ConnectorType];
            }

            var defaultName = typeof(TraceSourceConnector).AssemblyQualifiedName;
            return defaultName;
        }


        private string _connectorType;
        public string ConnectorType
        {
            get
            {
                return _connectorType;
            }
            set
            {
                if (string.Compare(_connectorType, value, true) == 0) return;
                LogConnector = null;
                _connectorType = value;
            }
        }


        public SourceLevels SourceLevels { get; set; }

        public HttpCaptureFlags OnErrorHttpCaptureFlags { get; set; }

        public object Clone()
        {
            return new LoggingOptions()
            {
                ActiveEnvironment = this.ActiveEnvironment,
                ApplicationName = this.ApplicationName,
                SourceLevels = this.SourceLevels,
                HostName = HostName,
                ConnectorType = this.ConnectorType
            };
        }

        public static LoggingOptions GetDefaultConfig()
        {
            return null;
        }
        string _activeEnvironment = string.Empty;
        /// <summary>
        /// Active environment as configured in .config file (HashTag.Application.Environment)
        /// </summary>
        [JsonIgnore]
        public string ActiveEnvironment
        {
            get
            {
                if (string.IsNullOrEmpty(_activeEnvironment) == false)
                {
                    return _activeEnvironment;
                }
                _activeEnvironment = ConfigurationManager.AppSettings[ConfigKeys.ActiveEnvironment];

                if (string.IsNullOrEmpty(_activeEnvironment) == true)
                {
                    _activeEnvironment = System.Environment.MachineName;
                }

                return _activeEnvironment;
            }
            set
            {
                _activeEnvironment = value;
            }
        }

        private string _applicationKey;
        /// <summary>
        /// Unique key for this application.  Often used in event logging and error handling (optional: HashTag.Application.Name)
        /// </summary>
        [JsonIgnore]
        public string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationKey) == false)
                {
                    return _applicationKey;
                }

                _applicationKey = ConfigurationManager.AppSettings[ConfigKeys.ApplicationName];
                if (string.IsNullOrEmpty(_applicationKey) == true)
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                    {
                        asm = Assembly.GetCallingAssembly();
                    }
                    if (asm == null)
                    {
                        asm = Assembly.GetExecutingAssembly();
                    }
                    _applicationKey = asm.GetName().Name;
                }
                return _applicationKey;
            }
            set
            {
                _applicationKey = value;
            }
        }
    }
}
