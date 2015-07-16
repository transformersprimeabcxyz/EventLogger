using System.Diagnostics;

namespace HashTag.Diagnostics
{
    public static class DiagnosticExtensions
    {
        /// <summary>
        /// Get default Priority from a TraceEventType
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static LogEventPriority ToPriority(this TraceEventType eventType)
        {

            switch (eventType)
            {

                case TraceEventType.Critical: return LogEventPriority.Highest;
                case TraceEventType.Error: return LogEventPriority.VeryHigh;
                case TraceEventType.Warning: return LogEventPriority.High; 
                case TraceEventType.Information: return LogEventPriority.Normal;
                case TraceEventType.Verbose: return LogEventPriority.Lowest; 
                case TraceEventType.Start: return LogEventPriority.Low; 
                case TraceEventType.Stop: return LogEventPriority.Low;
                default:
                    return LogEventPriority.Normal;
            }
        }

    }
}
