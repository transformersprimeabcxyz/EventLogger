using HashTag.Diagnostics;
using HashTag.Diagnostics.Config;
using HashTag.Diagnostics.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogEventProcessor_TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            var lg = LogEventLoggerFactory.NewLogger<Program>();

            var msg = lg.Error.Write("this is an error message");
            
            
            

//            var fileConfig = (string)loadConfig();
            
//            var baseSettings = @"{
//    ""EnvironmentKey"": ""HashTag.Application.Environment2"",
//    ""AppNameKey"": ""HashTag.Application.Name2"",
// ""PipeLine"": [
//        ""4000""],
//  ""Writers"": [ {
//            ""Name"": ""2400"",
//            ""Config"": {
//                ""switchValue"": ""Verbose""
//            }
//        }]
//            }";

//            var fileJson = JObject.Parse(fileConfig);
//            var baseJson = JObject.Parse(baseSettings);
//            fileJson.Merge(baseJson);
//            var mergedSettings = fileJson.ToString(Formatting.Indented);

//            var tt = JsonConvert.DeserializeObject<LogProcessorConfiguration>(fileConfig);
//            var y = JsonConvert.SerializeObject(tt, Formatting.Indented);

           // Console.WriteLine(y);
            Console.ReadKey();
        }

        private static string loadConfig()
        {
            var x = File.ReadAllText("LogEventConfig.json");
            return x;
        }
    }
}
/*
   "EnvironmentKey": "HashTag.Application.Environment2",
    "AppNameKey": "HashTag.Application.Name2",
*/