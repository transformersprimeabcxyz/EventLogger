# EventLogger
.Net Fluent logging wrapper for both NLog and native .Net TraceSource.  DI compatible with built in integrations for Ninject and Elmah.

## Quick Start
1. Install Nuget package (or download source and add a reference in your project
1. In your main application, set desired configuration settings
1. Begin capturing log events in your application

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

