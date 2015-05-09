using System;
namespace HashTag.Diagnostics
{
    public interface ILog:IDisposable
    {
        LogMessageBuilder Critical { get; }
        LogMessageBuilder Error { get; }
        LogMessageBuilder Info { get; }
        string LogName { get; set; }
        LogMessageBuilder Start { get; }
        LogMessageBuilder Stop { get; }
        LogMessageBuilder Verbose { get; }
        LogMessageBuilder Warning { get; }
        void Write(LogMessage message);
    }
}
