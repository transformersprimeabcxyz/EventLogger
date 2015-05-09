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
using System.Configuration;
using System.Reflection;

namespace HashTag.Configuration
{
    public partial class ConfigManager
    {
        /// <summary>
        /// Modify underlaying ConnectionStrings parameters at runtime, bypassing inherent ReadOnly status
        /// </summary>
        public class ConnectionStrings
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="connectionString"></param>
            /// <param name="providerName"></param>
            public static void Add(string name, string connectionString, string providerName)
            {
                Add(new ConnectionStringSettings(name,connectionString,providerName));
            }
            /// <summary>
            /// Add a new string to collection 
            /// </summary>
            /// <param name="setting"></param>
            public static void Add(ConnectionStringSettings setting)
            {
                var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);
                ConfigurationManager.ConnectionStrings.Add(setting);
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
            }

            ///// <summary>
            ///// Creates or updates a connection string setting
            ///// </summary>
            ///// <param name="connectionStringName">Case sensitive connection string name</param>
            ///// <param name="connectionString">Valid connection string for <paramref name="providerName"/></param>
            ///// <param name="providerName">(e.g. "System.Data.SqlClient")</param>
            //public static void SetConnectionString(string connectionStringName, string connectionString, string providerName)
            //{
            //    var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            //    readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);

            //    for (int x = ConfigurationManager.ConnectionStrings.Count - 1; x > 0; x--)
            //    {
            //        if (string.Compare(ConfigurationManager.ConnectionStrings[x].Name, connectionStringName, false) == 0)
            //        {
            //            ConfigurationManager.ConnectionStrings.Remove(connectionStringName);
            //            break;
            //        }
            //    }

            //    var csSetting = new ConnectionStringSettings(connectionStringName, connectionString, providerName);
            //    ConfigurationManager.ConnectionStrings.Add(csSetting);

            //    //var baseAddMethod = typeof(ConfigurationElementCollection).GetMethod("BaseAdd", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(ConfigurationElement) }, null);
            //    //baseAddMethod.Invoke(ConfigurationManager.ConnectionStrings, new object[] { csSetting });

            //    readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
            //}

            /// <summary>
            /// Clear all exsist keys from collection
            /// </summary>
            public static void Clear()
            {
                var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);
                ConfigurationManager.ConnectionStrings.Clear();
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
            }

            public static void Set(string name, string connectionString, string providerName)
            {
                Set(new ConnectionStringSettings(name, connectionString,providerName));
            }

            /// <summary>
            /// Update an existing key in the collection
            /// </summary>
            /// <param name="setting"></param>
            public static void Set(ConnectionStringSettings setting)
            {
                Remove(setting.Name);
                Add(setting);   
            }

            /// <summary>
            /// Remove an existing key from collection
            /// </summary>
            /// <param name="name"></param>
            public static void Remove(string name)
            {
                var readonlyField = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, false);
                ConfigurationManager.ConnectionStrings.Remove(name);
                readonlyField.SetValue(ConfigurationManager.ConnectionStrings, true);
            }
        }
    }
}