using HashTag.Diagnostics;
using System;
using System.Diagnostics;
namespace HashTag.Logging.Client.Configuration
{
    public class ClientConfig:ICloneable
    {
        private static string _hostName = Environment.MachineName;

        public ClientConfig()
        {
            HostName = _hostName;
        }
        public const bool IGNORECASE_FLAG = false;

        public string ApplicationName { get; set; }

        public string ActiveEnvironment { get; set; }

        public string HostName { get; set; }

        public ILogEventProcessor Processor { get; set; }

        public SourceLevels SourceLevels { get; set; }

        public object Clone()
        {
            return new ClientConfig()
            {
                ActiveEnvironment = this.ActiveEnvironment,
                ApplicationName = this.ApplicationName,
                Processor = this.Processor,
                SourceLevels = this.SourceLevels,
                HostName = HostName

            };
        }
    }
}
