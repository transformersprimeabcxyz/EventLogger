using Elmah;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Web.Library.Elmah
{
    public class OracleElmahConnector : OracleErrorLog
    {
        public OracleElmahConnector()
            : base(new Dictionary<string, string>())
        {

        }
        public OracleElmahConnector(IDictionary config)
            : base(config)
        {

        }
        public OracleElmahConnector(string connectionString)
            : base(connectionString)
        {

        }

    }
}
