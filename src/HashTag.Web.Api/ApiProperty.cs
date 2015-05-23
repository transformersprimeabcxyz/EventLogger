#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HashTag.Web.Api
{
    /// <summary>
    /// Represents a serializable key-value-type tuple
    /// </summary>
	[Serializable]
	public class ApiProperty : Comparer<ApiProperty>, IComparable<ApiProperty>, IComparable, IEqualityComparer<ApiProperty>, IEquatable<ApiProperty>, IComparer<ApiProperty>, ICloneable
	{
        private const bool IGNORECASE_FLAG = true;

        /// <summary>
        /// Default constructor
        /// </summary>
		public ApiProperty()
		{
            
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public ApiProperty(string key, object value)
		{
            Key = key;
            SetValue(value);
		}

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="property"></param>
		public ApiProperty(ApiProperty property)
		{
			this.Value = property.Value;
			this.ValueType = property.ValueType;
			this.Key = property.Key;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public ApiProperty(string key, string value)
		{
			Key = key;
			Value = value;
			ValueType = "System.String";
		}

		/// <summary>
		/// .Net type of value
		/// </summary>
		public string ValueType { get; set; }
		
        /// <summary>
        /// Key of property
        /// </summary>
		public string Key { get; set; }
		
        /// <summary>
        /// Value of property
        /// </summary>
		public string Value { get; set; }

        /// <summary>
        /// Sets the value and the type of this property
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public ApiProperty SetValue(object value)
		{
			if (value == null)
			{
				Value = "(null)";
				ValueType = "(unknown)";
				return this;
			}
			else
			{
                if (value == null)
                {
                    Value = "(null)";
                    ValueType = "(undefined)";
                    return this;
                }

				Type t = value.GetType();
				ValueType = t.FullName;
                if (t.IsPrimitive)
                {
                    Value = value.ToString();
                }
                else
                {
                    switch (ValueType)
                    {
                        case "System.String":
                            Value = (string)value;
                            break;
                        case "System.DateTime":
                            Value = string.Format("{0:yyyy-DD-MMTHH:mm:ss.fff}", value);
                            break;
                        case "System.Guid":
                            Value = ((Guid)value).ToString();
                            break;
                        default:
                            ValueType = string.Format("{0}::json", t.FullName);
                            Value = JsonConvert.SerializeObject(value);
                            break;
                    }
                }
                return this;
			}
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			ApiProperty source = obj as ApiProperty;
			if (source == null)
			{
				return false;
			}
			return (string.Compare(source.Key, this.Key, IGNORECASE_FLAG) == 0 && string.Compare(source.Value, this.Value, IGNORECASE_FLAG) == 0);
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			return GetHashCode(this);
		}

        /// <summary>
        /// Displays standard property syntax
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return string.Format("[{0}]={1} ({2})",
				Key,
				Value,
				ValueType);
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
		public override int Compare(ApiProperty left, ApiProperty right)
		{
			if (left != null && right == null) return 1;
			if (left == null && right == null) return 0;
			if (left == null && right != null) return -1;

			int result = string.Compare(left.Key, right.Key, IGNORECASE_FLAG);
			if (result != 0) return result;
			return string.Compare(left.Value, right.Value, IGNORECASE_FLAG);
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
		public int CompareTo(ApiProperty other)
		{
			return Compare(this, other);
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public int CompareTo(object obj)
		{
			return Compare(this, (ApiProperty)obj);
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
		public bool Equals(ApiProperty x, ApiProperty y)
		{
			return Compare(x, y) == 0;
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public int GetHashCode(ApiProperty obj)
		{
			return Key.GetHashCode() ^ Value.GetHashCode();
		}

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
		public bool Equals(ApiProperty other)
		{
			return Equals(this, other);
		}

        /// <summary>
        /// Creates a copy of this property
        /// </summary>
        /// <returns></returns>
		public object Clone()
		{
			return (object)new ApiProperty()
			{
				Key = this.Key,
				Value = this.Value,
				ValueType = this.ValueType
			};
		}
	}
}