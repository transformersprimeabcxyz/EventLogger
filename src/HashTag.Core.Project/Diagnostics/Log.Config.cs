using HashTag.Properties;
using HashTag.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public partial class Log
    {

        //public static void Configure()
        //{
        //    var results = Resource.EmbeddedFile("HashTag.Resources.DefaultLogConfig.json");
        //    var x = JsonConvert.DeserializeObject<dynamic>(results);
        //    var eventTypeName = x.EventProcessor.Type;
        //    var processorConfig = x.EventProcessor.Config;
        //    _processor = ProviderFactory<ILogEventProcessor>.GetInstance(eventTypeName.Value, processorConfig);

        //}

        //private static ILogEventProcessor _processor;
        //public static ILogEventProcessor Processor
        //{
        //    get
        //    {                
        //        return _processor;
        //    }
        //}
    }
}
