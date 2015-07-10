using HashTag.Diagnostics;
using HashTag.Logging.Client.NLog.Extensions;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Ninject.Activation;
using Logger = NLog.Logger;


namespace HashTag.NLog.Demo
{
    class Program
    {

        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var errorThrower = kernel.Get<IErrorThrower>();
            errorThrower.ThrowException();

            var errorThrower2 = kernel.Get<IErrorThrower2>();
            errorThrower2.ThrowException();

            Console.WriteLine("press Any key");
            Console.ReadKey();
        }


        private static void throwNewException(IEventLogger logger)
        {
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
                logger.Error.Write(ex);
            }
        }
    }

    public interface IErrorThrower
    {
        void ThrowException();
    }
    public interface IErrorThrower2
    {
        void ThrowException();
    }
    public class ErrorThrower : IErrorThrower
    {
        private IEventLogger _logger;
        public ErrorThrower(IEventLogger logger)
        {
            _logger = logger;
        }

        public void ThrowException()
        {
            _logger.Error.Write("something really really bad happened just now! by {0}", _logger.LogName);
        }
    }

    public class ErrorThrower2 : IErrorThrower2
    {
        private IEventLogger _logger;
        public ErrorThrower2(IEventLogger logger)
        {
            _logger = logger;
        }

        public void ThrowException()
        {
            
            _logger.Error.Write("22something really really bad happened just now! by {0}",_logger.LogName);
        }
    }

    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IErrorThrower>().To<ErrorThrower>();
            Bind<IErrorThrower2>().To<ErrorThrower2>();  

            Bind<IEventLogger>().ToMethod(g => EventLogger.GetLogger(g.Request.Target.Member.DeclaringType.FullName)).InTransientScope();
        }
    }

}
