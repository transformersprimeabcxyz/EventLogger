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
using System.Reflection;
using System.Collections;
using HashTag.Collections;

namespace HashTag.Reflection
{
	public class Reflector
	{
		public static T GetProtectedProperty<T>(string propertyName, object obj, T defaultValue)
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
			return Transform.ConvertValue<T>(value, defaultValue);
		}

		public static string[] GetPublicPropertyNames(Type reflectedType)
		{
			var propList = reflectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			List<string> propNames = new List<string>();
			foreach (var prop in propList)
			{
				propNames.Add(prop.Name);
			}
			return propNames.ToArray();
		}

		public static List<Property> GetPublicProperties(object objectToScan, string[] excludedPropertyNames)
		{
			Type exType = objectToScan.GetType();
			List<Property> properties = new List<Property>();
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
						properties.Add(new Property()
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
						properties.Add(new Property()
						{
							Key = key,
							ValueType = (c == null) ? "(null)" : c.GetType().Name,
							Value = value
						});
					}
				}
				else
				{
					properties.Add(new Property()
					{
						Key = prop.Name,
						ValueType = (propValue == null) ? "(null)" : propValue.GetType().Name,
						Value = (propValue == null) ? "(null)" : string.Format("{0}", propValue)
					});
				}
			}
			return properties;
		}
	}
}