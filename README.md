# Event Logger

## Quick Start
1. Install Nuget package (or download source and add a reference in your project)
1. In your main application, set desired configuration settings
1. Begin capturing log events in your application

### Logging in your code
(1) Create reference to a logger inside each class you want to log from.  There are several ways to do this including dependency injection though private instances is probably one of the most common.

```csharp
public class HomeController : Controller
{
    IEventLogger _log = EventLogger.GetLogger(typeof(HomeController));
}
````
(2) Capture log events

**Reminder:** - always end your `_log` statements with a `.Write()` which sends your event to the logging store (by default NLog)

```csharp
 public void SaveRecords(List<int> records)
        {
            _log.Start.Write("Saving records");
            var currentRecordId = -1;
            try
            {
                for(var x =0;x<records.Count;x++)
                {
                    _log.Info.Write("Processing and saving record: id: {0}, index: {1}", records[x], x);
                    currentRecordId = records[x];
                }
            }
            catch(Exception ex)
            {
                _log.Error.Reference(currentRecordId).Catch(ex).Write();
                _log.Error.Reference(currentRecordId).Write(ex);
                _log.Error.Reference(currentRecordId).Write();
                throw;
            }
            _log.Stop.Write("Finished saving records");
        }
```
## Recommended Configuration

### AppSettings
```csharp
 <appSettings>

    <add key="HashTag.Application.Name" value="[put something here]" />
    <add key="HashTag.Application.Environment" value="[something like local, DEV, PROD]" />
        
    <!-- 
    DO NOT REMOVE THESE COMMENTS
    
    A comma delmited list of:
      Off, Critical, Error, Warning, Information, ActivityTracing, All
      
    Default:  Information, ActivityTracing
    -->
    <add key="HashTag.Logging.SourceLevels" value="Information,ActivityTracing" />
</appSettings>
```
### Elmah

Route Elmah to NLog.

This is optional.  This section can be omitted if your application doesn't use Elmah or your application is not a web based application.

```csharp
  <elmah>
    <security allowRemoteAccess="false" />
    <errorLog type="HashTag.Logging.Client.Elmah.ElmahErrorLog, HashTag.Logging.Client" />
  </elmah>
```
### NLog Configuration

All log events are sent to NLog.  You can use any NLog settings to persist your application's events.  

This example is from the demo which uses some nice NLog features:
* Asynchronous writing so the primary application doesn't block
* Fall back on write failure
* Custom targets
* Layouts to format output
* File writing that is compatible for Splunk indexing
* Rules with split outputs

```csharp
  <nlog throwExceptions="true" xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <extensions>
      <add assembly="HashTag.Logging.Client" />
    </extensions>
    <targets async="true">
      <target xsi:type="FallbackGroup" name="output" returnToFirstOnSuccess="true">
        <target xsi:type="Debugger" name="T1" layout="${longdate}|${level}|${logger}|${message}" />
        <target xsi:type="Splunk" name="t2" dropFolder="c:\splunkfiles" />
      </target>    
      <target xsi:type="Splunk" name="demoTarget" dropFolder="c:\splunkfiles" />
    </targets>
    <rules>
      <logger name="*" minlevel="Trace" writeTo="output,demoTarget" />
    </rules>
  </nlog>
```
