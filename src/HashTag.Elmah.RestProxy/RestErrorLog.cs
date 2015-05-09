using Elmah;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Elmah.RestProxy
{
    public class RestErrorLog : ErrorLog
    {

        private readonly string _connectionString;

        private const int _maxAppNameLength = 60;

     
              /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorLog"/> class
        /// using a dictionary of configured settings.
        /// </summary>

        public RestErrorLog(IDictionary config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            string connectionString = "hello"; // ConnectionStringHelper.GetConnectionString(config);

            //
            // If there is no connection string to use then throw an 
            // exception to abort construction.
            //

            if (connectionString.Length == 0)
                throw new System.ApplicationException("Connection string is missing for the SQL error log.");

            _connectionString = connectionString;

            //
            // Set the application name as this implementation provides
            // per-application isolation over a single store.
            //

            string appName = "appname"; // Mask.NullString((string)config["applicationName"]);

            if (appName.Length > _maxAppNameLength)
            {
                throw new System.ApplicationException(string.Format(
                    "Application name is too long. Maximum length allowed is {0} characters.",
                    _maxAppNameLength.ToString("N0")));
            }

            ApplicationName = appName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorLog"/> class
        /// to use a specific connection string for connecting to the database.
        /// </summary>

        public RestErrorLog(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            if (connectionString.Length == 0)
                throw new ArgumentException(null, "connectionString");
            
            _connectionString = connectionString;
        }

        public override string Log(Error error)
        {
            return Guid.NewGuid().ToString();
        }
        public override string Name
        {
            get { return "Microsoft SQL Server Error Log"; }
        }
        public virtual string ConnectionString
        {
            get { return _connectionString; }
        }
        public override ErrorLogEntry GetError(string id)
        {
            return null;
        }

        public override int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList)
        {
            return 0;
            
        }
    }
}
