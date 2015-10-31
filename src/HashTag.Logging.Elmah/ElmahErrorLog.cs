using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using HashTag.Diagnostics;
using System.Diagnostics;
namespace HashTag.Logging.Client.Elmah
{
    /// <summary>
    /// Proxy to forward elmah generated events to default log event store.  This
    /// allows all elmah events to be handled through a single, consistent pipeline.
    /// Store can filter on HashTag.Logging.Client.Elmah source type
    /// </summary>
    public class ElmahErrorLog:ErrorLog
    {
        private static IEventLogger _log;
        private static volatile bool _isInitiailized = false;
        private static object _initializeLock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        public ElmahErrorLog()
        {

        }
        
        /// <summary>
        /// Constructor with configuration. (no-op)
        /// </summary>
        /// <param name="config">configuration passed from Elmah logging system</param>
        public ElmahErrorLog(IDictionary config)
        {

        }

        /// <summary>
        /// Constructor. (no-op)
        /// </summary>
        /// <param name="connectionString"></param>
        public ElmahErrorLog(string connectionString)
        {

        }

        /// <summary>
        /// Read from store and return Elmah error. (no-op)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Empty log entry so error isn't thrown if attempt is made to read from Elmah</returns>
        public override ErrorLogEntry GetError(string id)
        {
            return new ErrorLogEntry(this, id, null);
        }

        /// <summary>
        /// Read from store and return list of errors. (no-op)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="errorEntryList"></param>
        /// <returns>Empty list so no error is thrown if attempt is made to read from Elmah</returns>
        public override int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList)
        {
            return 0;
        }

        /// <summary>
        /// Write Elmah error to configured EventBuilder pipeline
        /// </summary>
        /// <param name="error"></param>
        /// <returns>Value that references the newly created EventBuilder object</returns>
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
