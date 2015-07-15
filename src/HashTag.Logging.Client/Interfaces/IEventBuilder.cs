using HashTag.Diagnostics;
using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;
using System;
namespace HashTag.Logging.Client.Interfaces
{
    /// <summary>
    /// Represents building up a single log event for later persistance
    /// </summary>
    public interface IEventBuilder
    {

       LoggingOptions Config { get; set; }
       IEventBuilder CaptureHttp();
       IEventBuilder CaptureHttp(HttpCaptureFlags flags);
       IEventBuilder CaptureIdentity();
       IEventBuilder CaptureMachineContext();
       IEventBuilder Catch(Exception ex);
       IEventBuilder Collect(string key, object value);
       IEventBuilder ForModule(string moduleName);
       IEventBuilder Reference(object reference);
       IEventBuilder Reference(string reference, params object[] args);
       IEventBuilder WithCode(string code, params object[] args);
       IEventBuilder WithId(int eventId);


       LogEvent Write(Exception ex, string message = null, params object[] args);
       LogEvent Write(object messageData = null);
       LogEvent Write(string message, params object[] args);

       

    }
}
