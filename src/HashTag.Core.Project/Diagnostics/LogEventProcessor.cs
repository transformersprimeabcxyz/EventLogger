using HashTag.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    public class LogEventProcessor
    {
        private static LogEventProcessorSettings _settings;
        public static Func<LogEventProcessorSettings> DefaultSettings { get; set; }
        public static AsyncBuffer<LogEvent> _buffer = new AsyncBuffer<LogEvent>(writeEvents);
        static LogEventProcessor()
        {

        }
        private static void writeEvents(List<LogEvent> eventBlock)
        {
            removeUnecessaryEvents(eventBlock, _settings.Filters);
            foreach (ILogEventWriter writer in _settings.Pipeline)
            {
                if (writer.WriteBlock(eventBlock,_settings) == true) break;
            }
        }

        private static void removeUnecessaryEvents(List<LogEvent> eventBlock, List<ILogEventFilter> filters)
        {
            if (filters == null || filters.Count == 0) return;

            if (eventBlock == null || eventBlock.Count == 0) return;

            foreach(var filter in filters)
            { 
                eventBlock.RemoveAll(evt=>!filter.ShouldLog(evt));
            }
        }

        public LogEvent Submit(LogEvent le)
        {
            _buffer.Submit(le);
            if (shouldFlushBuffer(le,_settings.ShouldFlushBufferFilters))
            {
                _buffer.Flush();
            }
            return le;
        }

        private bool shouldFlushBuffer(LogEvent le, List<ILogEventFilter> filters)
        {
            if (le == null || filters == null || filters.Count == 0) return true;
            return filters.Any(filter => filter.ShouldLog(le) == true);
        }

    }

    public class LogEventProcessorSettings
    {
        public LogEventProcessorSettings()
        {
            Pipeline = new List<ILogEventWriter>();
            Filters = new List<ILogEventFilter>();

            MaxInternalBufferSize = 300;
            BufferSweepMs = 1000;
            CacheTimeOutMs = 1000;
        }

        public int MaxInternalBufferSize { get; set; }
        public int BufferSweepMs { get; set; }
        public int CacheTimeOutMs { get; set; }

        public List<ILogEventWriter> Pipeline { get; set; }
        public List<ILogEventFilter> Filters { get; set; }
        public List<ILogEventFilter> ShouldFlushBufferFilters { get; set; }

    }
}
