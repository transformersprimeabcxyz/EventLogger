using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.HashTag.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.NLog.Demo
{
    class Program
    {
        private static ILogger log = LogManager.GetLogger(typeof(Program).FullName);
        static void Main(string[] args)
        {
            var xedd = new NLogSplunkTarget(); //forces copy of assembly
           
            log.Error("something really really bad happened just now!");
            var config = new LoggingConfiguration();
            LogEventInfo inf = new LogEventInfo(LogLevel.Debug,log.Name,"asfassaf");
            
            string sfas = JsonConvert.SerializeObject(config, Formatting.Indented);
           // var ctx = HttpContext;
           // var ctxCurrent = HttpContext.Request;
            try
            {
                var x = 1000;
                while (--x > -1)
                {
                    var y = x / x;
                }
            }
            catch(Exception ex)
            {
             //   var inf = new LogEventInfo(LogLevel.Fatal, log.Name, null);
             //   inf.Exception = ex;
             //   inf.Level = LogLevel.Fatal;
             //   inf.Properties.Add("prop1", "val1");
             ////   inf.LoggerShortName = log.Name;
             //   inf.LoggerName = log.Name;
             //  // inf.Context.Add("other stuff","aval");
             //   var g = ex.TargetSite.Name;
                ex.Data.Add("my key", "my value");
                //var s = JsonConvert.SerializeObject(inf, Formatting.Indented);
                //log.Log(inf);
                log.Fatal(ex,"some message");
            }


            Console.WriteLine("press Any key");
            Console.ReadKey();
        }
    }
}
