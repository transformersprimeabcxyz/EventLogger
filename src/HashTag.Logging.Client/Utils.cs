using System;
using System.Data.SqlTypes;

namespace HashTag
{
    public partial class Utils
    {
        public class Date
        {


            /// <summary>
            /// Maximum date value that can safely be stored in SQL2005,2008
            /// </summary
            public static readonly DateTime SqlMaxDate = SqlDateTime.MaxValue.Value;

            /// <summary>
            /// Minimum date value that can safely be stored in SQL2005,2008
            /// </summary
            public static readonly DateTime SqlMinDate = SqlDateTime.MinValue.Value;

            /// <summary>
            /// Maximum date that can be serialized by JSON.  JSON serializes dates by converting all dates to UTC and thus any Dates.MaxValue will exceed boundry for timezones 12 hrs from UTC (UTC-12)
            /// </summary>
            public static readonly DateTime JsonMaxDate = DateTime.MaxValue.AddDays(-2);

            /// <summary>
            /// Maximum date that can be stored in SQL2005,2008 and serialized to JSON
            /// </summary>
            public static readonly DateTime SafeMaxDate = new DateTime(Math.Min(SqlDateTime.MaxValue.Value.Ticks, JsonMaxDate.Ticks));

            /// <summary>
            /// Minimum data that can be stored in SQL2005,2008 and serialized to JSON
            /// </summary>
            public static readonly DateTime SafeMinDate = SqlMinDate.AddDays(2);

            /// <summary>
            /// Returns a date that is both JSON serializable and can be stored in SQL2005,2008
            /// </summary>
            /// <param name="dt">Date that will be adjusted to a safe date</param>
            /// <returns>A data that is safe for persisting</returns>
            public static DateTime ToSafeDate(DateTime dt)
            {
                DateTime dtMin = SqlDateTime.MinValue.Value.AddDays(2);
                if (dt < SafeMinDate)
                    return SafeMinDate;
                DateTime dtMax = new DateTime(Math.Min(SqlDateTime.MaxValue.Value.Ticks, JsonMaxDate.Ticks));
                if (dt > SafeMaxDate)
                    return SafeMaxDate;
                return dt;
            }
        }
    }
}
