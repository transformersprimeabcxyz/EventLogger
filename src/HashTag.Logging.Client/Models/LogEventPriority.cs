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
    public enum LogEventPriority
    {
        
        /// <summary>
        ///  Lowest message priority. (80000)
        /// </summary>
        Lowest = 80000,

        /// <summary>
        /// Between Low and Lowest message priority. (70000)
        /// </summary>
        VeryLow = 70000,

        /// <summary>
        /// Low message priority. (60000)
        /// </summary>
        Low = 60000,

        /// <summary>
        /// Normal message priority. (50000)
        /// </summary>    
        Normal = 50000,

        /// <summary>
        /// Between High and Normal message priority. (40000)
        /// </summary>
        AboveNormal = 40000,

        /// <summary>
        /// High message priority. (30000)
        /// </summary>
        High = 30000,

        /// <summary>
        /// Between Highest and High message priority. (20000)
        /// </summary>
        VeryHigh = 20000,

        /// <summary>
        /// Highest message priority. (10000)
        /// </summary>
        Highest = 10000,
    }
}
