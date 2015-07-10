using HashTag.Logging.Client.Interfaces;
using System;
namespace HashTag.Diagnostics
{
    /// <summary>
    /// Provides fluent based methods for event builder
    /// </summary>
    public interface IEventLogger:IDisposable
    {
        string LogName { get; set; }

        ILogEventBuilder Critical { get; }
        ILogEventBuilder Error { get; }
        ILogEventBuilder Info { get; }
        ILogEventBuilder Start { get; }
        ILogEventBuilder Stop { get; }
        ILogEventBuilder Verbose { get; }
        ILogEventBuilder Warning { get; }
    }
}
