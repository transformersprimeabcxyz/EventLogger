using Elmah;
using HashTag.Logging.Web.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Web.Library
{
    public class EventRepository:IEventRepository
    {

        public void StoreEvent(Error error)
        {
            throw new NotImplementedException();
        }
    }
}
