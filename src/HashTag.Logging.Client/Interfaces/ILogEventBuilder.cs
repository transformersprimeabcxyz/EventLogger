using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;
using System;
namespace HashTag.Logging.Client.Interfaces
{
    public interface ILogEventBuilder
    {
       ILogEventBuilder CaptureHttp();
       ILogEventBuilder CaptureHttp(HashTag.Diagnostics.HttpCaptureFlags flags);
       ILogEventBuilder CaptureIdentity();
       ILogEventBuilder CaptureMachineContext();
       ILogEventBuilder Catch(Exception ex);
       ILogEventBuilder Collect(string key, object value);
       LoggingOptions Config { get; set; }
       ILogEventBuilder ForModule(string moduleName);
       int GetHashCode();
       ILogEventBuilder Reference(object reference);
       ILogEventBuilder Reference(string reference, params object[] args);
       string ToString();
       ILogEventBuilder WithCode(string code, params object[] args);
       ILogEventBuilder WithId(int eventId);
       LogEvent Write(Exception ex, string message = null, params object[] args);
       LogEvent Write(object messageData = null);
       LogEvent Write(string message, params object[] args);
    }
}
