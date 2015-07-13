using System.Collections.Generic;

namespace HashTag.Diagnostics
{
    public interface IEventConnectorBase
    {
        void Initialize(IDictionary<string,string> config);
    }
}
