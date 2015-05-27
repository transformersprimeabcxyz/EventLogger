using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogWorker
    {
        void Initialize(object config);
    }
}
