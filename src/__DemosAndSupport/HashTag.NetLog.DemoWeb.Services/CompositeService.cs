using HashTag.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.NetLog.DemoWeb.Services
{
    public class CompositeService
    {
        IEventLogger _log = EventLogger.GetLogger<CompositeService>();

        public void SaveRecords(List<int> records)
        {
            _log.Start.Write("Saving records");
            var currentRecordId = -1;
            try
            {
                for(var x =0;x<records.Count;x++)
                {
                    _log.Info.Write("Processing and saving record: id: {0}, index: {1}", records[x], x);
                    currentRecordId = records[x];
                }
            }
            catch(Exception ex)
            {
                _log.Error.Reference(currentRecordId).Catch(ex).Write();
                _log.Error.Reference(currentRecordId).Write(ex);
                _log.Error.Reference(currentRecordId).Write();
                throw;
            }
            _log.Stop.Write("Finished saving records");

        }
    }
}
