using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Web.Library.Interfaces
{
    public interface IEventRepository
    {
        void StoreEvent(Error error);
    }
}
