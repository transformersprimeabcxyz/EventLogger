using System;
namespace HashTag.Diagnostics
{
    public interface ILog:IDisposable
    {
        LogEventBuilder Critical { get; }
        LogEventBuilder Error { get; }
        LogEventBuilder Info { get; }
        string LogName { get; set; }
        LogEventBuilder Start { get; }
        LogEventBuilder Stop { get; }
        LogEventBuilder Verbose { get; }
        LogEventBuilder Warning { get; }
        void Write(LogEvent message);
    }
}
