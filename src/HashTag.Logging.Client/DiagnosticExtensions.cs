using System.Diagnostics;

namespace HashTag.Diagnostics
{
    public static class DiagnosticExtensions
    {
        public static LogEventPriority ToPriority(this TraceEventType eventType)
        {

            switch (eventType)
            {

                case TraceEventType.Critical: return LogEventPriority.Highest;
                case TraceEventType.Error: return LogEventPriority.VeryHigh;
                case TraceEventType.Warning: return LogEventPriority.High; 
                case TraceEventType.Information: return LogEventPriority.Normal;
                case TraceEventType.Verbose: return LogEventPriority.Low; 
                case TraceEventType.Start: return LogEventPriority.VeryLow; 
                case TraceEventType.Stop: return LogEventPriority.VeryLow;
                default:
                    return LogEventPriority.Normal;
            }
        }

    }
}
