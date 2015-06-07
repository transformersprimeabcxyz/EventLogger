using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface IEventLoggerFactory
    {
        IEventLogger NewLogger(string logName, SourceLevels allowedSourceLevels = SourceLevels.All);
        IEventLogger NewLogger(Type logNameFromType, SourceLevels allowedSourceLevels = SourceLevels.All);
        IEventLogger NewLogger(object logNameFromObjectsType, SourceLevels allowedSourceLevels = SourceLevels.All);
        IEventLogger NewLogger<T>(SourceLevels allowedSourceLevels = SourceLevels.All);
    }
    


}
