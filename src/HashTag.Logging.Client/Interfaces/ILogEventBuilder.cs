using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;
using System;
namespace HashTag.Logging.Client.Interfaces
{
    /// <summary>
    /// Represents building up a single log event for later persistance
    /// </summary>
    public interface ILogEventBuilder
    {

       LoggingOptions Config { get; set; }
       ILogEventBuilder CaptureHttp();
       ILogEventBuilder CaptureHttp(HashTag.Diagnostics.HttpCaptureFlags flags);
       ILogEventBuilder CaptureIdentity();
       ILogEventBuilder CaptureMachineContext();
       ILogEventBuilder Catch(Exception ex);
       ILogEventBuilder Collect(string key, object value);
       ILogEventBuilder ForModule(string moduleName);
       ILogEventBuilder Reference(object reference);
       ILogEventBuilder Reference(string reference, params object[] args);
       ILogEventBuilder WithCode(string code, params object[] args);
       ILogEventBuilder WithId(int eventId);


       LogEvent Write(Exception ex, string message = null, params object[] args);
       LogEvent Write(object messageData = null);
       LogEvent Write(string message, params object[] args);

       

    }
}
