using HashTag.Diagnostics;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace HashTag.Logging.Client.Configuration
{
    /// <summary>
    /// Shapes the default behavior of the logging client
    /// </summary>
    public class LoggingOptions : ICloneable
    {
        private static string _hostName = Environment.MachineName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoggingOptions()
        {
            HostName = _hostName;
            OnErrorHttpCaptureFlags = HttpCaptureFlags.All;
            SourceLevels = SourceLevels.All;
            ConfigKeys = new Keys();
            ConnectorType = "HashTag.Logging.Client.NLog.Extensions.NLogEventConnector, HashTag.Logging.Client";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationKeys">Customized .config keys to use when loading configuration settings.  Often used in branding scenarios</param>
        public LoggingOptions(Keys applicationKeys)
            : this()
        {
            ConfigKeys = applicationKeys;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="branding">Company prefix to use on keys.  Often used in branding scenarios</param>
        public LoggingOptions(string branding):this()
        {
            ConfigKeys = new Keys(branding);
        }

        /// <summary>
        /// Configuration keys to retrieve logging client values from
        /// </summary>
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
                ApplicationModule = string.Format("{0}.Application.Module", branding);
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
            public string ApplicationModule { get; set; }

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
                        HttpCaptureFlags,
                        ApplicationModule,
                    };
                }
            }
        }

        /// <summary>
        /// Current .config keys to use when retrieving settings from .config file.  Referenced when a 
        /// new EventLogger is instantiated
        /// </summary>
        public Keys ConfigKeys { get; set; }

        public const bool IGNORECASE_FLAG = false;


        /// <summary>
        /// BIOS name of computer generating this event
        /// </summary>
        [JsonIgnore]
        public string HostName { get; set; }

        private IEventStoreConnector _connector = null;
        private object connectorLock = new object();

        /// <summary>
        /// Returns the connector that takes a log event and persists it to disk.  This
        /// property is what allows logging client to support a different number of
        /// 3rd party log persisters (e.g. log4net, nlog, .Net TraceSource)
        /// </summary>
        [JsonIgnore]
        public IEventStoreConnector LogConnector
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

                    _connector = HashTag.Reflection.ProviderFactory<IEventStoreConnector>.GetInstance(connectorType);
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

            //TODO intialize a default connector
            throw new ConfigurationErrorsException("Unable to determine a LogEventConnector");
        }

        
        private string _connectorType;
        /// <summary>
        /// Fully qualified type, assembly of connect EventBuilder will write event to
        /// </summary>
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

        /// <summary>
        /// Lowest level of messages EventLogger will write to logging store
        /// </summary>
        public SourceLevels SourceLevels { get; set; }

        /// <summary>
        /// HTTP values to auto-capture on Exception and more severe events
        /// </summary>
        public HttpCaptureFlags OnErrorHttpCaptureFlags { get; set; }

        public object Clone()
        {
            return new LoggingOptions()
            {
                ActiveEnvironment = this.ActiveEnvironment,
                ApplicationName = this.ApplicationName,
                SourceLevels = this.SourceLevels,
                HostName = this.HostName,
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
                _activeEnvironment = resolveKey(ConfigKeys.ActiveEnvironment);

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

                _applicationKey = resolveKey(ConfigKeys.ApplicationName);
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

        private string _applicationModule;
        /// <summary>
        /// Subsystem within an applications (MyAccountApp::AccountsPayable)
        /// </summary>
        [JsonIgnore]
        public string ApplicationModule
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationModule) == false)
                {
                    return _applicationModule;
                }

                _applicationModule = resolveKey(ConfigKeys.ApplicationModule);

                return _applicationModule;
            }
            set
            {
                _applicationModule = value;
            }
        }

        private string resolveKey(string key)
        {
            return resolveKey(ConfigurationManager.AppSettings[key], ConfigurationManager.AppSettings[key]);
            
        }
        private string resolveKey(string nextKey, string currentValue)
        {
            while(!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[nextKey]))
            {
                currentValue = ConfigurationManager.AppSettings[nextKey];
                nextKey = currentValue;
            }
            return currentValue;
        }
    }
}
