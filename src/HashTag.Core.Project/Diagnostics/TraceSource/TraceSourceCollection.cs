using HashTag.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// List of configured .Net TraceSources
    /// </summary>
    public class TraceSourceCollection
    {
        private static volatile ConcurrentDictionary<string, HashTagSource> _sources = new ConcurrentDictionary<string, HashTagSource>();

        private static bool _isInitialized = false;


        public static bool Configure(TraceConfig config)
        {
            var xmlConfig = GetConfigXml();
            
             configureApplicationListener(config, xmlConfig);

            return true;
        }

        private static void configureApplicationListener(TraceConfig config, XmlNode xmlConfig)
        {
            var internalNode = xmlConfig.SelectNodes("source[contains(@name,'ApplicationLog}')]");
            var dir = CoreConfig.Log.LogfilePath;
            var configFileName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var logFilePath = Path.Combine(dir, configFileName) + ".log";
            Log.Internal.Write("Configuring application log - trace source");
            if (internalNode.Count == 0) // create default internal source
            {
                Log.Internal.Write("Application traces ource not configured. Building default");
                var intSource = new TraceSource("{HashTag.ApplicationLog}");
                var intListener = new JsonFileTraceListener();
                intListener.LogFileName = logFilePath;
                Log.Internal.Write("Log file set to: {0}", intListener.LogFileName);
                Log.Internal.Write("{0} bound to source {1}", intListener.GetType().FullName, intSource.Name);

                intSource.Listeners.Clear();
                intSource.Listeners.Add(intListener);
                intSource.Switch.Level = CoreConfig.Log.ApplicationLogLevels;
                Log.Internal.Write("Source levels set to: {0}",intSource.Switch.Level.ToString());
                var htSource = new HashTagSource()
                {
                    NetSource = intSource,
                    SourceType = TraceSourceTypes.Application,
                    Name = intSource.Name 
                };

                _sources[intSource.Name] = htSource;
            }
            else  //merge existing attributes
            {
                var sourceName = internalNode[0].Attributes["name"].Value;
                var switchValue = internalNode[0].Attributes["switchValue"] == null?"":internalNode[0].Attributes["switchValue"].Value;
                var ts = new TraceSource(sourceName);
                if (string.IsNullOrEmpty(switchValue))
                {
                    ts.Switch.Level = CoreConfig.Log.ApplicationLogLevels;
                    Log.Internal.Write("Source level not configured. Using: {0}", ts.Switch.Level.ToString());
                }
                else
                {
                    SourceLevels level = CoreConfig.Log.ApplicationLogLevels;
                    if (Enum.TryParse<SourceLevels>(switchValue, out level) == false)
                    {
                        level = CoreConfig.Log.ApplicationLogLevels;
                        Log.Internal.Write(TraceEventType.Error, "Unable to convert switchValue '{0}' to valid System.Diagnostics.SourceLevels value.  Using default: {1}",
                            switchValue,
                            level.ToString()
                            );
                        
                    }
                    ts.Switch.Level = level;
                
                }
                var xmlListeners = internalNode[0].SelectNodes("listeners/add");
                if (xmlListeners.Count == 0) //no listeners configured so add default listener
                {
                    try
                    {
                        Log.Internal.Write("Listeners for source {0} not configured. Building default", ts.Name);
                        var listener = new JsonFileTraceListener();
                        listener.Name = "ApplicationFileTraceListener";
                        listener.LogFileName = logFilePath;
                        Log.Internal.Write("Listener {0} bound to {1}", listener.GetType().FullName, ts.Name);
                        Log.Internal.Write("Logfile Path: {0}", logFilePath);
                        ts.Listeners.Clear();
                        ts.Listeners.Add(listener);
                    }
                    catch (Exception ex)
                    {
                        Log.Local.Write(ex);
                        throw;
                    }
                }

                Log.Internal.Write("Source level set to {0}", ts.Switch.Level.ToString());
                
                var htSource = new HashTagSource()
                {
                    NetSource = ts,
                    SourceType = TraceSourceTypes.Application
                };
                _sources[sourceName] = htSource;
            }
        }

       
        public static XmlNode GetConfigXml()
        {
            var cfgFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(cfgFile);
            var node = xmlDoc.SelectSingleNode("//configuration/system.diagnostics/sources");
            return node != null ? node : xmlDoc.CreateElement("dummy");
        }

        private static TraceSource _defaultSource;
        public TraceSource ApplicationSource
        {
            get
            {
                if (_defaultSource != null) return _defaultSource;
                _defaultSource = _sources.First(s => s.Value.SourceType == TraceSourceTypes.Application).Value.NetSource;
                return _defaultSource;
            }
        }

       

        private static ConcurrentDictionary<string, TraceSource> _resolvedSources = new ConcurrentDictionary<string, TraceSource>();
        public TraceSource this[string name]
        {
            get
            {
                if (_isInitialized == false)
                {
                    Configure(TraceConfig.GetDefaultConfig());
                }
                if (_resolvedSources.ContainsKey(name) == true)
                {
                    return _resolvedSources[name];
                }

                if (_sources.ContainsKey(name) == true)
                {
                    var ts = _sources[name].NetSource;
                    _resolvedSources[name] = ts;
                    return ts;
                }
                throw new KeyNotFoundException();
            }
        }

        private TraceSource findSource(string name)
        {
            if (_sources.ContainsKey(name) == true)
            {
                return _sources[name].NetSource;
            }
            return ApplicationSource;
        }

    }
}
