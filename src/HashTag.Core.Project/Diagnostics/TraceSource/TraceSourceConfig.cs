using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class TraceSourceConfig
    {
        public TraceSourceConfig()
        {
            Listeners = new List<TraceListenerConfig>();
            SwitchType = "System.Diagnostics.SourceSwitch";
            SwitchValue = SourceLevels.Warning.ToString();
        }

        public List<TraceListenerConfig> Listeners { get; set; }
        public string SwitchType { get; set; }
        public string SwitchValue { get; set; }
        public string Name { get; set; }
    }
}
