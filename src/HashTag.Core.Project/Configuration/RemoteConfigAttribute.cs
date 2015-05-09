using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Configuration
{
    public class RemoteConfigAttribute
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string SettingUUID { get; set; }
    }
}
