using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public static class DiagnosticExtensions
    {
        public static MessagePriority ToPriority(this TraceEventType eventType)
        {

            switch (eventType)
            {

                case TraceEventType.Critical: return MessagePriority.Highest; break;
                case TraceEventType.Error: return MessagePriority.VeryHigh; break;
                case TraceEventType.Warning: return MessagePriority.High; break;
                case TraceEventType.Information: return MessagePriority.Normal; break;
                case TraceEventType.Verbose: return MessagePriority.Low; break;
                case TraceEventType.Start: return MessagePriority.VeryLow; break;
                case TraceEventType.Stop: return MessagePriority.VeryLow; break;
                default:
                    return MessagePriority.Normal; break;
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
