using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.NetLog.DemoWeb.Services
{
    public interface IInjectedService
    {
        void SaveRecords(List<int> records);
        void SaveRecordsWithRandomError(List<int> records);
    }
}
