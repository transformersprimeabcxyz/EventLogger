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

        IEventBuilder Critical { get; }
        IEventBuilder Error { get; }
        IEventBuilder Info { get; }
        IEventBuilder Start { get; }
        IEventBuilder Stop { get; }
        IEventBuilder Verbose { get; }
        IEventBuilder Warning { get; }
    }
}
