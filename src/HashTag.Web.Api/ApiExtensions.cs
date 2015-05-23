#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    public static class ApiExtensions
    {
        /// <summary>
        /// Extension method since StringBuilder.AppendLine() does not support format arguments
        /// </summary>
        /// <param name="builder">String builder that updates string</param>
        /// <param name="format">Format of string to append</param>
        /// <param name="args">Arguments to supply to format string</param>
        /// <returns>Formatted string appended to existing string builder with new line appended</returns>
        public static StringBuilder AppendLine(this StringBuilder builder, string format, params object[] args)
        {
            builder.AppendFormat(format, args).AppendLine();
            return builder;
        }

        /// <summary>
        /// Extension method since StringBuilder.Append() does not support format arguments (but AppendFormat does support arguments)
        /// </summary>
        /// <param name="builder">String builder that updates string</param>
        /// <param name="format">Format of string to append</param>
        /// <param name="args">Arguments to supply to format string</param>
        /// <returns>Formatted string appended to existing string builder with new line appended</returns>
        public static StringBuilder Append(this StringBuilder builder, string format, params object[] args)
        {
            builder.AppendFormat(format, args);
            return builder;
        }
    }
}
