using HashTag.Configuration;
using HashTag.Properties;
using HashTag.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HashTag.Diagnostics
{
    public class InternalLogger
    {
        private static ReaderWriterLockSlim _logLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private TraceSource _traceSource { get; set; }

        public void Write(string message, params object[] args)
        {
            Write(TraceEventType.Information, message, args);
        }

        public void Write(Exception ex)
        {
            if (ex == null) return;
            try
            {
                Write(TraceEventType.Error, ex.Expand());
            }
            catch (Exception ex2)
            {
                Write(TraceEventType.Error, ex2.ToString());
            }
        }

        public void Write(TraceEventType logLevel, string message, params object[] args)
        {
            var timeOutMs = CoreConfig.Log.InternalHandlerTimeOutMs;
            if (_logLock.TryEnterWriteLock(CoreConfig.Log.InternalHandlerTimeOutMs))
            {
                try
                {
                    if (_traceSource.Switch.ShouldTrace(logLevel))
                    {
                        _traceSource.TraceEvent(logLevel, (int)logLevel * -1, message,args);
                    }
                }
                catch (Exception ex)
                {
                    doFinalChanceMessage(ex, message, args);
                }
                finally
                {
                    _logLock.ExitWriteLock();
                }
            }
            else // cannot access trace source because it's too busy, blocked, etc.  
            {
                doFinalChanceMessage(null, CoreResources.MSG_Diagnostics_InternalLog_WriteLock_Error, CoreConfig.Log.InternalHandlerTimeOutMs);
                doFinalChanceMessage(null, message, args);
            }
        }

        public void Write(Exception ex, string messageFormat, params object[] args)
        {
            Write(ex);
            Write(messageFormat, args);
        }

        public void Write(LogMessage message)
        {
            if (_logLock.TryEnterWriteLock(CoreConfig.Log.InternalHandlerTimeOutMs))
            {
                try
                {
                    if (_traceSource.Switch.ShouldTrace(message.Severity))
                    {
                        _traceSource.TraceData(message.Severity, message.EventId, message);
                    }
                }
                catch (Exception ex)
                {
                    doFinalChanceMessage(ex, message.ToString());
                }
                finally
                {
                    _logLock.ExitWriteLock();
                }
            }
            else // cannot access trace source because it's too busy, blocked, etc.  
            {
                doFinalChanceMessage(null, CoreResources.MSG_Diagnostics_InternalLog_WriteLock_Error, CoreConfig.Log.InternalHandlerTimeOutMs);
                doFinalChanceMessage(null, message.ToString());
            }
        }

        private void doFinalChanceMessage(Exception ex, string message, params object[] args)
        {
            try // write to event log if unable to log to configured trace source.  This is really the last chance of the last chance!
            {
                string assmName = this.GetType().Assembly.GetName().Name;
                if (message != null)
                {
                    EventLog.WriteEntry(assmName, TextUtils.StringFormat(message, args), EventLogEntryType.Error);
                }
                if (ex != null)
                {
                    EventLog.WriteEntry(assmName, string.Format(ex.ToString()), EventLogEntryType.Error);
                }
            }
            catch (Exception ex2) //couldn't write log to last chance log file or event viewer so swallow exception
            {
                try
                {
                    System.Diagnostics.Trace.TraceError(string.Format(CoreResources.MSG_Diagnostics_InternalLog_Error, ex2.ToString(), Environment.NewLine, message));
                }
                catch (Exception ex3) // swallow final exception.  This is the third level cascade exception so we don't do anything with it
                {
                    Console.WriteLine(ex3.ToString());
                }
            }
        }

        private static Tuple<SourceLevels,string> getDefaultInternalLogLevel()
        {
           
            SourceLevels level = CoreConfig.Log.InternalLogLogLevels;
            var msg = string.Format(CultureInfo.InvariantCulture, "Using HashTag.Diagnostics.InternalLogLevel of SourceLevels.{0}", level.ToString());
           
            var retVal = new Tuple<SourceLevels, string>(level,msg);
            return retVal;
        }
        internal static InternalLogger configureInternalLog()
        {
            var xmlConfig = TraceSourceCollection.GetConfigXml();
            var log = new InternalLogger();
            var internalNode = xmlConfig.SelectNodes("source[contains(@name,'InternalLog}')]");
            List<string> messages = new List<string>();

            var defaultLevel = getDefaultInternalLogLevel();
            messages.Add(defaultLevel.Item2);
            if (internalNode.Count == 0) // nothing configured in .config so create a default source
            {
               
                var intSource = new TraceSource(CoreResources.HashTag_Core_Setting_InternalLog_TraceSourceName);
                messages.Add(CoreResources.MSG_Diagnostics_InternalLogger_TraceNotFound,intSource.Name);

                intSource.Switch.Level = defaultLevel.Item1;
                messages.Add(CoreResources.MSG_Diagnostics_InternalLogger_FilteringMessagesTo, intSource.Switch.Level.ToString());
                log._traceSource = intSource;
                intSource.Listeners.Clear();

                var cListener = new InternalTraceListener();
                cListener.Name = "consoleInternalListener";
                intSource.Listeners.Add(cListener);

                messages.Add(string.Format(CoreResources.MSG_Diagnostics_LocalLogger_BoundToListener, cListener.GetType().FullName));
            }
            else  //merge existing attributes
            {
                var sourceName = internalNode[0].Attributes["name"].Value;
                messages.Add("System.diagnostics/sources {0} found in .config file",sourceName);
                
                var ts = new TraceSource(sourceName);
                log._traceSource = ts;
                if (internalNode[0].Attributes["switchValue"] == null)
                {
                    ts.Switch.Level = defaultLevel.Item1;
                    messages.Add("Source switchValue attribute not found.  Defaulting to: TraceEventType.{0}", ts.Switch.Level.ToString());
                }
                else
                {
                    messages.Add("Internal logger filtering messages: {0}", ts.Switch.Level.ToString());
                }
                var xmlListeners = internalNode[0].SelectNodes("listeners/add");
                if (xmlListeners.Count == 0) //no listeners configured so add default listener
                {
                    try
                    {
                        ts.Listeners.Clear();
                        messages.Add("No listeners configured in .config");
                        var cListener = new InternalTraceListener();
                        cListener.Name = "consoleInternalListener";
                        ts.Listeners.Add(cListener);

                        messages.Add(CoreResources.MSG_Diagnostics_LocalLogger_BoundToListener, cListener.GetType().FullName);

                    }
                    catch (Exception ex)
                    {

                        messages.Add(ex.ToString());
                        throw;
                    }
                }
                else
                {
                    foreach (TraceListener listener in ts.Listeners)
                    {
                        messages.Add(CoreResources.MSG_Diagnostics_LocalLogger_BoundToListener, listener.GetType().FullName);
                    }
                }
            }
            messages.Add(CoreResources.MSG_Diagnostics_Internal_Logger_Configured);
            
            if (log._traceSource.Switch.ShouldTrace(TraceEventType.Information))
            {
                foreach (var msg in messages)
                {
                    log.Write(TraceEventType.Information, msg);
                }
            }

            return log;
        }

        internal static InternalLogger configureLocalLog()
        {
            var dir = CoreConfig.Log.LogfilePath;
            var configFileName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var logFilePath = Path.Combine(dir, configFileName) + ".local.Log";

            var xmlConfig = TraceSourceCollection.GetConfigXml();
            var log = new InternalLogger();
            var internalNode = xmlConfig.SelectNodes("source[contains(@name,'LocalLog}')]");

            if (internalNode.Count == 0) // nothing configured in .config so create a default source
            {
                var intSource = new TraceSource("{HashTag.Diagnostics.LocalLog}");
                Log.Internal.Write("{0} not found in .config source.  Building default",intSource.Name);

                intSource.Switch.Level = CoreConfig.Log.LastChanceLogLevels;
                Log.Internal.Write("Local Log filtering messages: {0}", intSource.Switch.Level.ToString());
                log._traceSource = intSource;
                intSource.Listeners.Clear();

                var intListener = new InternalFileTraceListener();
                intListener.Name = "defaultLastChanceListener";
                intListener.LogFileName = logFilePath;
                intSource.Listeners.Add(intListener);
                Log.Internal.Write("{0} bound to source {1}", intListener.GetType().FullName, intSource.Name);
                Log.Internal.Write("Log file: {0}", logFilePath);

                var evtListener = new EventLogTraceListener();
                evtListener.Name = "eventLogLastChanceListener";
                evtListener.EventLog = new EventLog();
                evtListener.EventLog.Source = CoreConfig.ApplicationName;
                evtListener.EventLog.Log = "Application";
                intSource.Listeners.Add(evtListener);
                Log.Internal.Write("{0} bound to source {1}", evtListener.GetType().FullName, intSource.Name);

                var cListener = new InternalTraceListener();
                cListener.Name = "consoleLastChanceListener";
                intSource.Listeners.Add(cListener);
                Log.Internal.Write("{0} bound to source {1}", cListener.GetType().FullName, intSource.Name);

            }
            else  //merge existing attributes to first node containing 'LocalLog'
            {
                var sourceName = internalNode[0].Attributes["name"].Value;
                var ts = new TraceSource(sourceName);
                log._traceSource = ts;
                if (internalNode[0].Attributes["switchValue"] == null)
                {
                    ts.Switch.Level = CoreConfig.Log.LastChanceLogLevels;
                    Log.Internal.Write(CoreResources.MSG_Diagnostics_SourceLevelNotFound, ts.Switch.Level.ToString());
                }
                Log.Internal.Write(CoreResources.MSG_Diagnostics_LocalLog_Filtering_Messages_ToSwitch, ts.Switch.Level.ToString());
                
                var xmlListeners = internalNode[0].SelectNodes("listeners/add");
                if (xmlListeners.Count == 0) //no listeners configured so add default listener
                {
                    try
                    {
                        ts.Listeners.Clear();
                        Log.Internal.Write(CoreResources.MSG_Diagnostics_LocalLog_No_Listeners_Specified);

                        var intListener = new InternalFileTraceListener();
                        intListener.Name = "defaultLastChanceListener";
                        intListener.LogFileName = logFilePath;
                        ts.Listeners.Add(intListener);
                        Log.Internal.Write("{0} bound to source {1}", intListener.GetType().FullName, ts.Name);
                        Log.Internal.Write("Log file: {0}", logFilePath);

                        var evtListener = new EventLogTraceListener();
                        evtListener.Name = "eventLogLastChanceListener";
                        evtListener.EventLog = new EventLog();
                        evtListener.EventLog.Source = CoreConfig.ApplicationName;
                        evtListener.EventLog.Log = "Application";
                        ts.Listeners.Add(evtListener);
                        Log.Internal.Write("{0} bound to source {1}", evtListener.GetType().FullName, ts.Name);

                        var cListener = new InternalTraceListener();
                        cListener.Name = "consoleLastChanceListener";
                        ts.Listeners.Add(cListener);
                        Log.Internal.Write("{0} bound to source {1}", cListener.GetType().FullName, ts.Name);

                    }
                    catch (Exception ex)
                    {
                        Log.Internal.Write(ex);
                        throw;
                    }
                }
            }
            Log.Internal.Write(CoreResources.MSG_Diagnostics_Local_Logger_Configured);
            return log;
        }

    }
    public static class LoggerExtensions
    {
        public static void Add(this List<string> messageList, string message, params object[] args)
        {
            messageList.Add(string.Format(message, args));
        }
    }
}
