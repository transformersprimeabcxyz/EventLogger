using System;
using System.Collections.Generic;
using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Defines pluggable behavior used by EventBuilder for submitting newly created events to some kind of store.  
    /// Usually only one IEventStore connector is used in an application and often within a group or enterprise
    /// </summary>
    /// <remarks>Replace concrete implementations of this interface to persist to other providers (e.g. log4Net, TraceSource, EnterpriseLibrary)</remarks>
    public interface IEventStoreConnector
    {
        /// <summary>
        /// Send a <see cref="HashTag.Diagnostics.Models.LogEvent">LogEvent</see> to persistant storage
        /// </summary>
        /// <param name="evt">Fully hyrated event to persist</param>
        /// <returns>UUID of submitted event. Might be used in UI scenarios or other tracing</returns>
        Guid Submit(LogEvent evt, EventOptions options = null);

        /// <summary>
        /// Submit a block of <see cref="HashTag.Diagnostics.Models.LogEvent">LogEvents</see> to peristant storage
        /// </summary>
        /// <param name="events"></param>
        void Submit(List<LogEvent> events, EventOptions options = null);

        /// <summary>
        /// Tells underlying provider to force a write-all operation of any pending writes.  Provider should implement as a non-op method
        /// if not implementing functionality.
        /// </summary>
        void Flush();

        /// <summary>
        /// Tells underlying provider to immediately cease writing messages (though it may continue to collect them). Provider should implement as a non-op method
        /// if not implementing functionality.
        /// </summary>
        void Stop();

        /// <summary>
        /// Tells underlying provider to immediately being writing messages (and any cached messages). Provider should implement as a non-op method
        /// if not implementing functionality.
        /// </summary>
        void Start();

        /// <summary>
        /// Provide boostrapping hook for starting up a connector.  Provider should implement as a non-op method
        /// if not implementing functionality.
        /// </summary>
        /// <param name="config">List of KVP containing connector configuration information</param>
        void Initialize(IDictionary<string, string> config);

    }
}
