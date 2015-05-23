#pragma warning disable 1591
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace HashTag.Web.Api
{
    public partial class ApiException
    {
        private static string expand(ApiException ex)
        {
            return expand(ex, 0);
        }
        private static string expand(ApiException ex, int offSetIndex)
        {
            StringBuilder sb = new StringBuilder();
            string paddingString = "";
            if (offSetIndex > 0)
                paddingString = new String(' ', offSetIndex);


            sb.AppendLine("{1}===== Begin {0} =====", ex.ExceptionType, paddingString);
            _expandException(ex, 0, sb);
            sb.AppendLine("{1}===== End {0} =====", ex.ExceptionType, paddingString);

            return sb.ToString().Replace(Environment.NewLine, Environment.NewLine + paddingString);

        }

        private static void _expandException(ApiException ex, int offSet, StringBuilder sb)
        {
            string paddingString = "";
            if (offSet > 1)
                paddingString = new String(' ', offSet);

            string replacementString = Environment.NewLine + paddingString + new String(' ', 24);
            makeLine(sb, "Message", ex.Message, replacementString, paddingString);
            makeLine(sb, "Error Code", ex.ErrorCode, replacementString, paddingString);
            makeLine(sb, "Exception Type", ex.ExceptionType, replacementString, paddingString);
            foreach (var prop in ex.Properties)
            {
                makeLine(sb, prop.Key, string.Format("{0} [{1}]", prop.Value, prop.ValueType), replacementString, paddingString);
            }
            makeLine(sb, "Source", ex.Source, replacementString, paddingString);
            makeLine(sb, "Stack Trace", ex.StackTrace, replacementString, paddingString);
            makeLine(sb, "Target Site", ex.TargetSite, replacementString, paddingString);
            makeLine(sb, "HelpLink", ex.HelpLink, replacementString, paddingString);
            if (ex.InnerException != null)
            {
                sb.AppendFormat("{0}    ----- Inner {1} (begin) -----{2}", paddingString, ex.InnerException.ExceptionType, Environment.NewLine);
                _expandException(ex.InnerException, offSet + 4, sb);
                sb.AppendFormat("{0}    ----- Inner {1} (end) ------{2}", paddingString, ex.InnerException.ExceptionType, Environment.NewLine);
            }
            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (var dataItem in ex.Data)
                {
                    var key = string.Format("Data[{0}]", dataItem.Key);
                    makeLine(sb, key, dataItem.Value, replacementString, paddingString);
                }
            }
        }

        private static void makeLine(StringBuilder sb, string param, object value, string replacementString, string paddingString)
        {
            string format = "{2}{0,-20}: {1}";
            if (value == null)
            {
                value = "(null)";
            }
            sb.AppendLine(format, param, string.Format("{0}", value).Replace(Environment.NewLine, replacementString), paddingString);
        }

        private static string[] getPublicPropertyNames(Type reflectedType)
        {
            var propList = reflectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            List<string> propNames = new List<string>();
            foreach (var prop in propList)
            {
                propNames.Add(prop.Name);
            }
            return propNames.ToArray();
        }

        private static List<ApiProperty> getPublicProperties(object objectToScan, string[] excludedPropertyNames)
        {
            Type exType = objectToScan.GetType();
            List<ApiProperty> properties = new List<ApiProperty>();
            var propList = exType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var prop in propList)
            {
                if (excludedPropertyNames.Contains(prop.Name) == true) continue;

                var propValue = prop.GetValue(objectToScan, null);
                if (propValue is IDictionary && propValue != null)
                {
                    var iDictionary = propValue as IDictionary;
                    foreach (DictionaryEntry de in iDictionary)
                    {
                        string propKey = string.Format("{0}[{1}]", prop.Name, de.Key);
                        string propValue2 = (de.Value == null) ? "(null)" : string.Format("{0}", de.Value);
                        string propType = (de.Value == null) ? "(null)" : string.Format("{0}", de.Value.GetType().FullName);
                        properties.Add(new ApiProperty()
                        {
                            Key = propKey,
                            Value = propValue2,
                            ValueType = propType,
                        });
                    }
                }
                else if (propValue is ICollection && propValue != null)
                {
                    var iCollection = propValue as ICollection;
                    int x = 0;
                    foreach (var c in iCollection)
                    {
                        string key = string.Format("{0}[{1}]", prop.Name, x++);
                        string value = (c == null) ? "(null)" : string.Format("{0}", c);
                        properties.Add(new ApiProperty()
                        {
                            Key = key,
                            ValueType = (c == null) ? "(null)" : c.GetType().Name,
                            Value = value
                        });
                    }
                }
                else
                {
                    properties.Add(new ApiProperty()
                    {
                        Key = prop.Name,
                        ValueType = (propValue == null) ? "(null)" : propValue.GetType().Name,
                        Value = (propValue == null) ? "(null)" : string.Format("{0}", propValue)
                    });
                }
            }
            return properties;
        }
        private static T getProtectedProperty<T>(string propertyName, object obj, T defaultValue)
        {
            Type exType = obj.GetType();
            PropertyInfo propInfo = exType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (propInfo == null)
            {
                return defaultValue;
            }
            object value = propInfo.GetValue(obj, null);
            if (value == null)
            {
                return defaultValue;
            }
            return convertValue<T>(value, defaultValue);
        }

        /// <summary>
        /// Convert an object into T
        /// </summary>
        /// <typeparam name="T">Type to convert value into</typeparam>
        /// <param name="value">Value that will be converted into T</param>
        /// <param name="defaultValue">return value if not able to convert value into T</param>
        /// <returns>value converted into T or defaultValue if any errors or NULL</returns>
        private static T convertValue<T>(object value, T defaultValue)
        {
            try
            {
                return convertValue<T>(value);
            }
            catch
            {
                return defaultValue; //always return default value if any error occurs
            }
        }
        /// <summary>
        /// Convert an object into T.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted T</returns>
        /// <exception cref="InvalidCastException">Thrown when value is null</exception>
        /// <exception cref="NotImplementedException">Thrown when value is not able to be converted into T</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static T convertValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                throw new InvalidCastException(string.Format("Unable to cast a NULL value to '{0}'", typeof(T).FullName));
            }


            if (typeof(T).IsEnum == true)
            {
                return (T)(object)Enum.Parse(typeof(T), convertValue<System.String>(value), true);
            }
            
            switch (typeof(T).ToString())
            {
                case "System.Byte": return (T)(object)Convert.ToByte(value);
                case "System.SByte": return (T)(object)Convert.ToSByte(value);
                case "System.Char": return (T)(object)Convert.ToChar(value);
                case "System.Decimal": return (T)(object)Convert.ToDecimal(value); // Decimal.Parse(value.ToString());
                case "System.Double": return (T)(object)Convert.ToDouble(value);
                case "System.Single": return (T)(object)Convert.ToSingle(value);
                case "System.Int32": return (T)(object)Convert.ToInt32(value); // Int32.Parse(value);
                case "System.UInt32": return (T)(object)Convert.ToUInt32(value);
                case "System.Int64": return (T)(object)Convert.ToInt64(value); // Int64.Parse(value.ToString());
                case "System.UInt64": return (T)(object)Convert.ToUInt64(value);
                case "System.Int16": return (T)(object)Convert.ToInt16(value); // Int16.Parse(value.ToString());
                case "System.UInt16": return (T)(object)Convert.ToUInt16(value);
                case "System.Guid": return (T)(object)new System.Guid(value.ToString());
                case "System.DateTime": return (T)(object)DateTime.Parse(value.ToString());
                case "System.String": return (T)(object)value.ToString();
                case "System.Boolean":
                    Boolean boolRetVal;
                    if (Boolean.TryParse(value.ToString(), out boolRetVal) == true)
                        return (T)(object)boolRetVal;
                    switch (value.ToString().ToUpper())
                    {
                        case "ON":
                        case "1":
                        case "YES":
                        case "TRUE": return (T)(object)true;
                        case "OFF":
                        case "0":
                        case "NO":
                        case "FALSE": return (T)(object)false;
                        default:
                            throw new NotImplementedException(string.Format("Conversion Not Implemented for Type: '{0}' or could not convert value '{1}' to System.Boolean", typeof(T).Name, value));
                    }
                case "System.Byte[]":
                    try
                    {
                        return (T)(object)value; //attempt simple cast
                    }
                    catch
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bf.Serialize(ms, value);
                            ms.Seek(0, 0);
                            return (T)(object)ms.ToArray();
                        }
                    }
                default:
                    throw new NotImplementedException(string.Format("Conversion Not Implemented for Type: '{0}'", typeof(T).Name));
            }
        }
    }
}
