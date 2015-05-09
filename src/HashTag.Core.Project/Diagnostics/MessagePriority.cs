/**
/// FirstEnergy.Common.Core Library
/// Copyright © 2005-2012
///



**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HashTag.Diagnostics
{
    /// <summary>
    ///  Specifies how urgent the sender of this message considers this message. (Lowest=80000,...Highest=10000).  Often used in
    ///  distributed logging scenarious to order which messages are sent to the logging handlers first
    /// </summary>
    /// <remarks>
    /// Most urgent priority is lowest value to match TraceEventType ordering.  High enumeration values
    /// are used so MessagePriority and TraceEventTypes can be combined to
    /// create a default EventId in LogMessage
    /// </remarks>
    [DataContract(Namespace = CoreConfig.WcfNamespace)]
    public enum MessagePriority
    {
        
        /// <summary>
        ///  Lowest message priority. (80000)
        /// </summary>
        [EnumMember]
        Lowest = 80000,

        /// <summary>
        /// Between Low and Lowest message priority. (70000)
        /// </summary>
        [EnumMember]
        VeryLow = 70000,

        /// <summary>
        /// Low message priority. (60000)
        /// </summary>
        [EnumMember]
        Low = 60000,

        /// <summary>
        /// Normal message priority. (50000)
        /// </summary>    
        [EnumMember]
        Normal = 50000,

        /// <summary>
        /// Between High and Normal message priority. (40000)
        /// </summary>
        [EnumMember]
        AboveNormal = 40000,

        /// <summary>
        /// High message priority. (30000)
        /// </summary>
        [EnumMember]
        High = 30000,

        /// <summary>
        /// Between Highest and High message priority. (20000)
        /// </summary>
        [EnumMember]
        VeryHigh = 20000,

        /// <summary>
        /// Highest message priority. (10000)
        /// </summary>
        [EnumMember]
        Highest = 10000,
    }
}
