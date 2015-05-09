using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Configuration
{
    public class RemoteConfigSetting
    {
        public RemoteConfigSetting()
        {
            Attributes = new List<RemoteConfigAttribute>();
        }
        public RemoteConfigSetting(RemoteConfigSetting serviceConfig)
            : this()
        {
            UUID = serviceConfig.UUID;
            foreach (var serviceAttribute in serviceConfig.Attributes)
            {
                Attributes.Add(mapper(serviceAttribute));
            }
        }

        private RemoteConfigAttribute mapper(RemoteConfigAttribute serviceAttribute)
        {
            return new RemoteConfigAttribute()
            {
                Name = serviceAttribute.Name,
                SettingUUID = serviceAttribute.SettingUUID,
                UUID = serviceAttribute.UUID,
                Value = serviceAttribute.Value
            };
        }
        public string UUID { get; set; }
        public List<RemoteConfigAttribute> Attributes { get; set; }
        public RemoteConfigAttribute this[string attributeName]
        {
            get
            {
                return Attributes.First(a => string.Compare(attributeName, a.Name) == 0);
            }
        }
        public string SettingGroup
        {
            get
            {
                return this["groupName"].Value;
            }
        }
    }
}
