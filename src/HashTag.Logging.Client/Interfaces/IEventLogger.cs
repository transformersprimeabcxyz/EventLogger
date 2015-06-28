using HashTag.Logging.Client.Interfaces;
using System;
namespace HashTag.Diagnostics
{
    public interface IEventLogger:IDisposable
    {
        ILogEventBuilder Critical { get; }
        ILogEventBuilder Error { get; }
        ILogEventBuilder Info { get; }
        string LogName { get; set; }
        ILogEventBuilder Start { get; }
        ILogEventBuilder Stop { get; }
        ILogEventBuilder Verbose { get; }
        ILogEventBuilder Warning { get; }
    }
}
