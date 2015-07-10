using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;
using System.Collections;
using HashTag.Diagnostics;
using System.Diagnostics;
namespace HashTag.Logging.Client.Elmah
{
    public class ElmahErrorLog:ErrorLog
    {
        private static IEventLogger _log;
        private static volatile bool _isInitiailized = false;
        private static object _initializeLock = new object();

        public ElmahErrorLog()
        {

        }
        
        public ElmahErrorLog(IDictionary config)
        {

        }

        public ElmahErrorLog(string connectionString)
        {
        }

        public override ErrorLogEntry GetError(string id)
        {
            return new ErrorLogEntry(this, id, null);
        }

        public override int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList)
        {
            return 0;
        }

        public override string Log(Error error)
        {
            ensureInitialized();
            var logEvent = _log.Error.Write(error.Exception);
            return logEvent.UUID.ToString();
        }

        private static void ensureInitialized()
        {
            if (_isInitiailized == false || _log == null)
            {
                lock(_initializeLock)
                {
                    if (_isInitiailized == false || _log == null)
                    {
                        _log = EventLogger.GetLogger<ElmahErrorLog>(SourceLevels.All);
                        _isInitiailized = true;
                    }
                }
            }
        }
    }
}
