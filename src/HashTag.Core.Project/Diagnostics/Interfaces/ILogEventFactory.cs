using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogFactory
    {
        ILog NewLog(string logName, SourceLevels allowedSourceLevels = SourceLevels.All);
        ILog NewLog(Type logNameFromType, SourceLevels allowedSourceLevels = SourceLevels.All);
        ILog NewLog(object logNameFromObjectsType, SourceLevels allowedSourceLevels = SourceLevels.All);
    }
    public interface ILogFactory<TEventLogger>:ILogFactory
    {
        ILog NewLog(SourceLevels allowedSourceLevels=SourceLevels.All);        
    }


}
