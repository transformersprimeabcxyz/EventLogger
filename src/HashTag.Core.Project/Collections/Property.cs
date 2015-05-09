/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HashTag.Collections
{
	[DataContract(Namespace = CoreConfig.WcfNamespace)]
	[Serializable]
	public class Property : Comparer<Property>, IComparable<Property>, IComparable, IEqualityComparer<Property>, IEquatable<Property>, IComparer<Property>, ICloneable
	{

		public Property()
		{
		}
		public Property(string key, object value)
		{
            Key = key;
            SetValue(value);
		}

		public Property(Property property)
		{
			this.Value = property.Value;
			this.ValueType = property.ValueType;
			this.Key = property.Key;
		}

		public Property(string key, string value)
		{
			Key = key;
			Value = value;
			ValueType = "System.String";
		}

		[DataMember]
		public string ValueType { get; set; }
		[DataMember]
		public string Key { get; set; }
		[DataMember]
		public string Value { get; set; }

		public Property SetValue(object value)
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

		public override bool Equals(object obj)
		{
			Property source = obj as Property;
			if (source == null)
			{
				return false;
			}
			return (string.Compare(source.Key, this.Key, CoreConfig.IGNORECASE_FLAG) == 0 && string.Compare(source.Value, this.Value, CoreConfig.IGNORECASE_FLAG) == 0);
		}
		public override int GetHashCode()
		{
			return GetHashCode(this);
		}
		public override string ToString()
		{
			return string.Format("[{0}]={1} ({2})",
				Key,
				Value,
				ValueType);
		}

		public override int Compare(Property left, Property right)
		{
			if (left != null && right == null) return 1;
			if (left == null && right == null) return 0;
			if (left == null && right != null) return -1;

			int result = string.Compare(left.Key, right.Key, CoreConfig.IGNORECASE_FLAG);
			if (result != 0) return result;
			return string.Compare(left.Value, right.Value, CoreConfig.IGNORECASE_FLAG);
		}

		public int CompareTo(Property other)
		{
			return Compare(this, other);
		}

		public int CompareTo(object obj)
		{
			return Compare(this, (Property)obj);
		}

		public bool Equals(Property x, Property y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(Property obj)
		{
			return Key.GetHashCode() ^ Value.GetHashCode();
		}

		public bool Equals(Property other)
		{
			return Equals(this, other);
		}

		public object Clone()
		{
			return (object)new Property()
			{
				Key = this.Key,
				Value = this.Value,
				ValueType = this.ValueType
			};
		}
	}
}