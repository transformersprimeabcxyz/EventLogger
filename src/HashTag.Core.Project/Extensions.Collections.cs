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
using System.Collections.Specialized;

namespace HashTag
{
	public static partial class Extensions
	{
        /// <summary>
        /// (HashTag.Core extension) Get a strongly typed value from a name/value (string/string) collection
        /// </summary>
        /// <typeparam name="T">Type to cast target string into</typeparam>
        /// <param name="collection">Hydrated list of name/value pairs</param>
        /// <param name="key">Key in list to look up</param>
        /// <returns>Value set at [key] if found or default(T) if key doesn't exist</returns>
        public static T Get<T>(this NameValueCollection collection, string key)
        {
            string retVal = collection[key];
            if (retVal == null) 
                throw new InvalidOperationException(string.Format("Required key: '{0}' missing from collection",
                    key));
            return Transform.ConvertValue<T>(retVal);
        }

        /// <summary>
        /// (HashTag.Core extension) Get a strongly typed value from a name/value (string/string) collection
        /// </summary>
        /// <typeparam name="T">Type to cast target string into</typeparam>
        /// <param name="collection">Hydrated list of name/value pairs</param>
        /// <param name="key">Key in list to look up</param>
        /// <param name="defaultValue">Value to use if key cannot be found OR an excetpion occurs when converting found value into T</param>
        /// <returns>Value set at [key] if found or defaultValue if key doesn't exist</returns>
        public static T Get<T>(this NameValueCollection collection, string key, T defaultValue)
        {
            string retVal = collection[key];
            try
            {
                if (retVal == null) return defaultValue;
                return Transform.ConvertValue<T>(retVal);
            }
            catch (Exception)  //all exceptions return default value
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// (HashTag.Core extension) Copy values from one collection to another
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static NameValueCollection Clone(this NameValueCollection collection)
        {
            var retList = new NameValueCollection();
            int length = collection.Count;
            for (int x = 0; x < length; x++)
            {
                retList.Add(collection.Keys[x], collection[x]);
            }
                return retList;
        }

		/// <summary>
		/// (HashTag.Core extension) Add a value to a string dictionary using formatted string
		/// </summary>
		/// <param name="collection">Collection to which item is being added</param>
		/// <param name="key">Key of item to be added.  If key exists, it will be overwritten</param>
		/// <param name="value"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Dictionary<string, string> Add(this Dictionary<string, string> collection, string key, string value, params object[] args)
		{
			collection[key]= string.Format(value, args);
			return collection;
		}

        /// <summary>
        /// Adds a set of records from one collection into another
        /// </summary>
        /// <param name="target">Collection into which items will be collected</param>
        /// <param name="source">Collection which is being copied into collection</param>
        public static void AddRange(this Dictionary<string, object> target, Dictionary<string, object> source)
        {
            if (source == null)
            {
                return;
            }
            foreach (var kvp in source)
            {
                target[kvp.Key] = kvp.Value;
            }
        }
	}
}