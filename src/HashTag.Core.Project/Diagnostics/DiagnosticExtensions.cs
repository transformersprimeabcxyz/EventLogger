using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        public static bool IsEnabledFor(this SourceLevels sourceLevel, TraceEventType eventType)
        {
            if (sourceLevel == SourceLevels.Off) return false;
            if (sourceLevel == SourceLevels.All) return true;

            SourceLevels convertedSourceLevel = eventType.ToSourceLevels();

            return sourceLevel >= convertedSourceLevel;
        }
        
        public static bool AreEqual(this SourceLevels sourceLevel, TraceEventType eventType)
        {
            
            var convertedSourceLevel = eventType.ToSourceLevels();
            return (sourceLevel & convertedSourceLevel) == convertedSourceLevel;
        }

        public static SourceLevels ToSourceLevels(this TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical: return SourceLevels.Critical; 
                case TraceEventType.Error: return SourceLevels.Error; 
                case TraceEventType.Information: return SourceLevels.Information; 
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Transfer:
                case TraceEventType.Resume: return SourceLevels.ActivityTracing; 
                case TraceEventType.Verbose: return SourceLevels.Verbose; 
                case TraceEventType.Warning: return SourceLevels.Warning; 
            }
            return SourceLevels.Off;
        }
    }
}
