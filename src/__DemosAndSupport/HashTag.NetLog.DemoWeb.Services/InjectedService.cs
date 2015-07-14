using HashTag.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HashTag.NetLog.DemoWeb.Services
{
    public class InjectedService:IInjectedService
    {
        IEventLogger _log;
        public InjectedService(IEventLogger logger)
        {
            _log = logger;
        }

        public void SaveRecords(List<int> records)
        {
            _log.Info.Write("API Service");
        }


        public void SaveRecordsWithRandomError(List<int> records)
        {
            var rdm = new System.Random();

            var errorIndex = rdm.Next(0, records.Count);
            _log.Start.Write("Processing {0} records", records.Count);

            var currentRecord = 0;
            try
            {
                for(int x=0;x<records.Count;x++)
                {
                    currentRecord = records[x];
                    _log.Info.Reference(currentRecord).Write("Processing record: {0}", currentRecord);
                    Thread.Sleep(((int)rdm.NextDouble()) * 1000);
                    _log.Info.Write("Processed record: {0}", currentRecord);

                    //simulate an error
                    if (x ==errorIndex)
                    {
                        throw new InvalidOperationException("SaveRecordsWithRandomError::Something really really bad happened! Running away to become animated cartoon");
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Error.Reference(currentRecord).Write(ex);
                throw;
            }
        }
    }
}
