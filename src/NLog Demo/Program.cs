using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace NLog_Demo
{
    class Program
    {
        private static ILogger _log = LogManager.GetLogger(typeof (Program).FullName);
        static void Main(string[] args)
        {
            _log.Error("this is something really really bad!");
        }
    }
}
