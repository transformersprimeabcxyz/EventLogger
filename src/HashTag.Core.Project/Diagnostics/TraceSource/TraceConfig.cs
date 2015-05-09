using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class TraceConfig
    {
        public TraceConfig()
        {

        }

        public TraceSourceConfig InternalLogSourceConfig { get; set; }
        public TraceSourceConfig LastChanceLogSourceConfig { get; set; }
        public TraceSourceConfig ApplicationLogSourceConfig { get; set; }

        public string DefaultLogFolderName
        {
            get
            {
                var p3 = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                var p4 = Path.GetDirectoryName(p3);
                var folderName = Path.Combine(p4, "logs");
                if (Directory.Exists(folderName) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(folderName);
                        return folderName;
                    }
                    catch
                    {
                        return p4;
                    }
                }

                return folderName;
            }
        }


        public static TraceConfig GetDefaultConfig()
        {
            var config = new TraceConfig();
            config.InternalLogSourceConfig = new TraceSourceConfig()
           {
               SwitchValue = SourceLevels.Off.ToString(),
               Name = "HashTag.Internal"
           };
            config.InternalLogSourceConfig.Listeners.Add(new TraceListenerConfig()
                {
                    ListenerType = "System.Diagnostics.Console",
                    Name = "InternalConsoleListener"
                }
                );
            config.InternalLogSourceConfig.Listeners.Add(new TraceListenerConfig()
                {
                    ListenerType = "System.Diagnostics.DefaultTraceListener",
                    Name = "InternalDefaultListener"
                });


            config.LastChanceLogSourceConfig = new TraceSourceConfig()
            {
                SwitchValue = "Warning"
            };
            config.LastChanceLogSourceConfig.Listeners.Add(new TraceListenerConfig()
                {
                    ListenerType = "System.Diagnostics.Console",
                    Name = "LastChanceConsoleListener"
                }
            );
            config.LastChanceLogSourceConfig.Listeners.Add(new TraceListenerConfig()
            {
                ListenerType = "System.Diagnostics.DefaultTraceListener",
                Name = "LastChanceDefaultListener"
            });
            config.LastChanceLogSourceConfig.Listeners.Add(new TraceListenerConfig()
            {
                ListenerType = "System.Diagnostics.EventLogTraceListener",
                Name = "LastChanceDefaultListener"
            });

            config.ApplicationLogSourceConfig = new TraceSourceConfig()
            {
                SwitchValue = "Information",
                Name = CoreConfig.ApplicationName
            };

            config.LastChanceLogSourceConfig.Listeners.Add(new TraceListenerConfig()
            {
                ListenerType = "System.Diagnostics.DefaultTraceListener",
                Name = "ApplicationDefaultListener"
            });

            return config;
        }



    }
}
