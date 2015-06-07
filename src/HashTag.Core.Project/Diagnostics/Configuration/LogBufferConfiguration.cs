using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    public class LogBufferConfiguration
    {
        public LogBufferConfiguration()
        {
            BufferAutoFlushSize = 300;
            FailOverSize = 1000;
            CacheTimeOutMs = 1000;
            BufferWriteTimeOutMs = 10000;
            FlushFilters = new List<List<ConfigProvider>>();
        }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue BufferAutoFlushSize { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue FailOverSize { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue BufferSweepMs { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue CacheTimeOutMs { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue BufferWriteTimeOutMs { get; set; }

        [JsonConverter(typeof(FilterConverter))]
        public List<List<ConfigProvider>> FlushFilters { get; set; }

    }
}
