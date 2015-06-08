using System;
namespace HashTag.Diagnostics
{
    public interface IEventLogger:IDisposable
    {
        LogEventBuilder Critical { get; }
        LogEventBuilder Error { get; }
        LogEventBuilder Info { get; }
        string LogName { get; set; }
        LogEventBuilder Start { get; }
        LogEventBuilder Stop { get; }
        LogEventBuilder Verbose { get; }
        LogEventBuilder Warning { get; }
        Func<LogMessage, Guid> Write { get; set; }
    }
}
