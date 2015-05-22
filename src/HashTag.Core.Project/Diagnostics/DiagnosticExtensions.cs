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
        public static bool IsEnabled(this SourceLevels sourceLevel, TraceEventType eventType)
        {
            SourceLevels traceLevelEquivilant = SourceLevels.Off;
            switch(eventType)
            {
                case TraceEventType.Critical: traceLevelEquivilant = SourceLevels.Critical; break;
                case TraceEventType.Error: traceLevelEquivilant = SourceLevels.Error; break;
                case TraceEventType.Information: traceLevelEquivilant = SourceLevels.Information; break;
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Transfer:
                case TraceEventType.Resume: traceLevelEquivilant = SourceLevels.ActivityTracing; break;
                case TraceEventType.Verbose: traceLevelEquivilant = SourceLevels.Verbose; break;
                case TraceEventType.Warning: traceLevelEquivilant = SourceLevels.Warning; break;                
            }

            return (sourceLevel & traceLevelEquivilant) == traceLevelEquivilant;
        }
    }
}
