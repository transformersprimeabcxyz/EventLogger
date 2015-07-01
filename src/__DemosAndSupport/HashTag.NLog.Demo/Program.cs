using HashTag.Diagnostics;
using HashTag.Logging.Client.NLog.Extensions;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.NLog.Demo
{
    class Program
    {
       
        static void Main(string[] args)
        {
            //private static ILogger log = LogManager.GetLogger(typeof(Program).FullName);
            var log = LoggerFactory.NewLogger<Program>();

            log.Error.Write("something really really bad happened just now!");

            var xedd = new NLogSplunkTarget(); //forces copy of assembly

            var cn = new NLogEventConnector();
            cn.Initialize(null);
           // log.Error("something really really bad happened just now!");
           // var config = new LoggingConfiguration();
           // LogEventInfo inf = new LogEventInfo(LogLevel.Debug,log.Name,"asfassaf");
            
           // string sfas = JsonConvert.SerializeObject(config, Formatting.Indented);
           //// var ctx = HttpContext;
           //// var ctxCurrent = HttpContext.Request;
            try
            {
                var x = 1000;
                while (--x > -1)
                {
                    var y = x / x;
                }
            }
            catch (Exception ex)
            {
                log.Error.Write(ex);
            }

            Console.WriteLine("press Any key");
            Console.ReadKey();
        }
    }
}
