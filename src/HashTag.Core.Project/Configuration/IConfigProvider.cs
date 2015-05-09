using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Configuration
{
    public interface  IConfigProvider
    {

        List<RemoteConfigSetting> LoadSettings(Dictionary<string,string> remoteParams=null);
    }
}
