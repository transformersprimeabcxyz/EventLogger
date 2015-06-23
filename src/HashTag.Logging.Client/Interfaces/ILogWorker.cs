using System.Collections.Generic;

namespace HashTag.Diagnostics
{
    public interface ILogWorker
    {
        void Initialize(IDictionary<string,string> config);
    }
}
