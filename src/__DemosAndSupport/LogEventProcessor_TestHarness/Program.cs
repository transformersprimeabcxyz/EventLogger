using HashTag.Diagnostics;
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
            var lg = LoggerFactory.NewLogger<Program>();
            var msg = lg.Error.Write("this is an error message");
            Console.ReadKey();
        }

        private static string loadConfig()
        {
            var x = File.ReadAllText("LogEventConfig.json");
            return x;
        }
    }
}
