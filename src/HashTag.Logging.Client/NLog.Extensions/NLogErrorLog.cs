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
namespace HashTag.Logging.Client.NLog.Extensions
{
    public class NLogConnectorForElmah:ErrorLog
    {
        private static ILogger _log = LogManager.GetLogger(typeof(NLogConnectorForElmah).FullName);
        public NLogConnectorForElmah()
        {

        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorLog"/> class
        /// using a dictionary of configured settings.
        /// </summary>
        public NLogConnectorForElmah(IDictionary config)
        {
            //if (config == null)
            //    throw new ArgumentNullException("config");

            //string connectionString = ConfigManager.ConnectionString("Elmah");

            ////
            //// If there is no connection string to use then throw an 
            //// exception to abort construction.
            ////

            //if (connectionString.Length == 0)
            //    throw new System.ApplicationException("Connection string is missing for the SQL error log.");

            //_connectionString = connectionString;

            ////
            //// Set the application name as this implementation provides
            //// per-application isolation over a single store.
            ////

            //string appName = "YourApplicationName"; // e.Mask.NullString((string)config["applicationName"]);

            //if (appName.Length > _maxAppNameLength)
            //{
            //    throw new System.ApplicationException(string.Format(
            //        "Application name is too long. Maximum length allowed is {0} characters.",
            //        _maxAppNameLength.ToString("N0")));
            //}

            //ApplicationName = appName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorLog"/> class
        /// to use a specific connection string for connecting to the database.
        /// </summary>
        public NLogConnectorForElmah(string connectionString)
        {
            //if (connectionString == null)
            //    throw new ArgumentNullException("connectionString");

            //if (connectionString.Length == 0)
            //    throw new ArgumentException(null, "connectionString");

            //_connectionString = connectionString;
        }

        public override ErrorLogEntry GetError(string id)
        {
            return new ErrorLogEntry(this, id, null);
        }

        public override int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList)
        {
            return 0;
           // throw new NotImplementedException();
        }

        public override string Log(Error error)
        {
            var newId = Guid.NewGuid();
            LogEventInfo le = new LogEventInfo(LogLevel.Error, _log.Name, error.Message);
            le.Properties.Add("AppName", "tbd: elmah-app");
            le.Properties.Add("EnvName", "tbd:elmah-env");
            le.TimeStamp = error.Time;
            le.Properties.Add("Id", newId);            
            le.Properties.Add("Elmah", error);           
            _log.Log(le);
            
            return newId.ToString();
            
           
        }

        
    }
}
