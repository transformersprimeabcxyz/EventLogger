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
using HashTag.Configuration;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HashTag.Diagnostics;
using HashTag;
using HashTag.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Configuration.Internal;
using System.Collections.Specialized;
using System.Diagnostics;

namespace HashTag.Configuration
{
    /// <summary>
    /// Methods to retrieve values from connection strings.  Contains FSFH (fail-fast, fail-hard) logic and appropriate logging
    /// </summary>
    public sealed partial class ConfigManager
    {
        //static bool _usingExternalSource = Initialize(); //force loading external configuration system before any other calls to ConfigManager
        ///// <summary>
        ///// Load external configuration sources, if configured
        ///// </summary>
        ///// <returns></returns>
        //public static bool Initialize(IInternalConfigSystem system=null )
        //{
        //    //MergeConfigFiles();  // merge several .config files into separate files

        //    // hack our proxy IInternalConfigSystem into the ConfigurationManager
        //    //try
        //    //{
        //    //    if (string.IsNullOrEmpty(CoreConfig.Configuration.ExternalSource) == true)
        //    //    {
        //    //        //Log.InternalLog(LogLevel.Information, "Unable to determine external configuration setting source.  Using only .Net default .config settings");
        //    //        return true; //
        //    //    }
        //    //    // inject custom config setting source into ConfigurationManager
        //    //    FieldInfo s_configSystem = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static | BindingFlags.NonPublic);
        //    //    IInternalConfigSystem externalSource = Reflection.ProviderFactory<IInternalConfigSystem>.GetInstance(CoreConfig.Configuration.ExternalSource, s_configSystem.GetValue(null));
        //    //    s_configSystem.SetValue(null, externalSource);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Log.InternalLog(ex, "Unable to set external source of configuration data to '{0}'.  Be sure assembly exists and implements 'System.Configuration.IInternalConfigSystem'", CoreConfig.Configuration.ExternalSource);
        //    //}
        //    return true; //doesn't matter what we return.  Value is ignored     
        //}

        internal static readonly TokenReplacer _valueReplacer = new TokenReplacer((token) =>
        {
            switch (token.Delimiter.Begin)
            {
                case "$[":
                    if (ConfigurationManager.AppSettings.AllKeys.Contains(token.OuterToken))
                    {
                        return ConfigurationManager.AppSettings[token.OuterToken];
                    }
                    if (ConfigurationManager.AppSettings.AllKeys.Contains(token.InternalToken))
                    {
                        return ConfigurationManager.AppSettings[token.InternalToken];
                    }
                    return string.Format("$?NOT FOUND: {0})", token.InternalToken);
                case "${":
                    var cnString = ConfigurationManager.ConnectionStrings[token.OuterToken];
                    if (cnString != null)
                    {
                        return cnString.ConnectionString;
                    }
                    cnString = ConfigurationManager.ConnectionStrings[token.InternalToken];
                    if (cnString != null)
                    {
                        return cnString.ConnectionString;
                    }
                    return string.Format("$?NOT FOUND: {0})",token.InternalToken);
            }
            return token.OuterToken;
        }, "${", "}", "$[", "]");

        internal static readonly TokenReplacer _keyReplacer = new TokenReplacer((token) =>
        {
            switch (token.Delimiter.Begin)
            {
                case "$[":
                    if (ConfigurationManager.AppSettings.AllKeys.Contains(token.OuterToken))
                    {
                        return ConfigurationManager.AppSettings[token.OuterToken];
                    }
                    if (ConfigurationManager.AppSettings.AllKeys.Contains(token.InternalToken))
                    {
                        return ConfigurationManager.AppSettings[token.InternalToken];
                    }
                    return string.Format("$?NOT FOUND: {0})", token.InternalToken);
                case "${":
                    var cnString = ConfigurationManager.ConnectionStrings[token.OuterToken];
                    if (cnString != null)
                    {
                        return cnString.ConnectionString;
                    }
                    cnString = ConfigurationManager.ConnectionStrings[token.InternalToken];
                    if (cnString != null)
                    {
                        return cnString.ConnectionString;
                    }
                    return string.Format("$?NOT FOUND: {0})", token.InternalToken);
            }
            return token.OuterToken;
        }, "${", "}", "$[", "]");

        //static bool _usingExternalSource = Initialize(); //force loading external configuration system before any other calls to ConfigManager
        /// <summary>
        /// Load external configuration sources, if configured
        /// </summary>
        /// <returns></returns>
       
        
        /// <summary>
        /// Get a required value from AppSettings section of the configuration pipeline.  Expands any embedded config metadata tags
        /// </summary>
        /// <typeparam name="T">Type to cast result to</typeparam>
        /// <param name="key">Key in AppSettings section containing value</param>
        /// <returns>Converted type</returns>
        /// <exception cref="ConfigurationErrorsException">Thrown if key cannot be found or converted into requested type</exception>
        public static T AppSetting<T>(string key)
        {
            string configValue = ConfigurationManager.AppSettings[_keyReplacer.Execute(key)];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find required <AppSettings> key = '{0}' (resolved: {1})in .config file or configuration pipeline", key, _valueReplacer.Execute(key));
                //Log.InternalLog( MessageLevel.Error,msg);
                throw new ConfigurationErrorsException(msg);
            }
            try

            {
                configValue = _valueReplacer.Execute(configValue);
                return configValue.ConvertTo<T>();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Unable to convert key '{2}[{0}]' to type of {1}", configValue, typeof(T).FullName, key);
                //Log.InternalLog(ex, msg);
                throw new ConfigurationErrorsException(msg, ex);
            }
        }

        /// <summary>
        /// Get an optional value from AppSettings section of the configuration pipeline.  Expands any embedded config metadata tags
        /// </summary>
        /// <typeparam name="T">Type to cast result to</typeparam>
        /// <param name="key">Key in AppSettings section containing value</param>
        /// <param name="defaultValue">default to use if key is not found in configuration file</param>
        /// <returns>Converted type</returns>
        /// <exception cref="ConfigurationErrorsException">Throw when configuration value is provided but cannot be converted into T</exception>
        public static T AppSetting<T>(string key, T defaultValue)
        {

            var expandedKey = _keyReplacer.Execute(key);
            string configValue = ConfigurationManager.AppSettings[expandedKey];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find <AppSettings> key='{0}' in .config or configuration pipeline.  Using default value '{1}' instead",
                    expandedKey, defaultValue.ToString());
                //Log.InternalLog(MessageLevel.Warning, msg);
                return defaultValue;
            }
            try
            {
                configValue = _valueReplacer.Execute(configValue);
                return configValue.ConvertTo<T>();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Unable to convert key '{2}[{0}]' to type of {1}.", configValue, typeof(T).FullName, key);
                //Log.InternalLog(ex, msg);
                throw new ConfigurationErrorsException(msg, ex);
            }
        }

        /// <summary>
        /// Gets a connection string from .config or configuration pipeline
        /// </summary>
        /// <param name="key">Key in connection string to get</param>
        /// <returns>Connection string as found in .config file</returns>
        /// <exception cref="ConfigurationException">If key cannot be found in .config file</exception>
        public static string ConnectionString(string key)
        {
            string expandedKey = _keyReplacer.Execute(key);
            ConnectionStringSettings configValue = ConfigurationManager.ConnectionStrings[expandedKey];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find <ConnectionStrings> key = '{0}' in .config file or configuration pipeline",
                    expandedKey);
                //Log.InternalLog(msg);
                throw new ConfigurationErrorsException(msg);
            }
            return _valueReplacer.Execute(configValue.ConnectionString);
        }

        /// <summary>
        /// Gets a SQL connection from .config or configuration pipeline
        /// </summary>
        /// <param name="key">Key in connection string to get</param>
        /// <returns>An instance of a provider's connection</returns>
        /// <exception cref="ConfigurationErrorsException">If key cannot be found in .config file</exception>
        public static IDbConnection Connection(string key)
        {
            string expandedKey = _keyReplacer.Execute(key);
            ConnectionStringSettings configValue = ConfigurationManager.ConnectionStrings[expandedKey];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find <ConnectionStrings> key = \"{0}\" in .config file or configuration pipeline",
                    expandedKey);
              //  Log.InternalLog(msg);
                throw new ConfigurationErrorsException(msg);
            }

            // DbProviderFactory DBProvider = DbProviderFactories.GetFactory(configValue.ProviderName);
            return (IDbConnection)new SqlConnection(configValue.ConnectionString);
            //return DBProvider.CreateConnection();
        }


        /// <summary>
        /// Method to expand tokens within a key
        /// </summary>
        /// <param name="tokenizedString">String with one of these tokens {env}, {app}, {machine}, {user} </param>
        /// <returns>A string with tokens expanded</returns>
        /// <remarks>
        /// <para>{env} - active environment</para>
        /// <para>{app} - application key</para>
        /// <para>{machine} - machine name</para>
        /// <para>{user} - user name</para>
        /// </remarks>
        public static string ResolveKeyTokens(string tokenizedString) //EXTEND
        {
            if (tokenizedString == null) return tokenizedString;

            return tokenizedString.Replace("{machine}", Environment.MachineName).Replace("{env}", CoreConfig.ActiveEnvironment).Replace("{app}", CoreConfig.ApplicationName).Replace("{user}", Environment.UserName);
        }

        /// <summary>
        /// Resolve any .config tokens and retrieve (appSettings and connectionString) values from remote source
        /// </summary>
        /// <param name="outputResolvedTokens"></param>
        /// <param name="mergeOptions"></param>
        public static void Initialize(bool? outputResolvedTokens=null, ConfigMergeOptions mergeOptions = ConfigMergeOptions.ConfigFileWins)
        {
            // resolve any .config tokens calling configuration service
            ResolveTokens(false);

            // get settings from remote source (if any configured) and apply to .config file
            LoadSetingsFromService(mergeOptions);
            
            // resolve any .config tokens again because service might have returned tokens
        ResolveTokens(outputResolvedTokens);
        }
     
        /// <summary>
        /// Resolve all tokens in appSettings and connectionStrings. Warning: Output includes unencrypted connection strings so care must not be expose contents in an unsafe environment
        /// </summary>
        /// <param name="outputResolvedValues">True to output appSettings and connectionStrings to output window. If null use FirstEnergy.Common.Configuration.OutputOnResolve 
        /// configuration setting which defaults to false if not found in .config
        /// </param>
        /// <returns>True</returns>
        public static bool ResolveTokens(bool? outputResolvedValues = null)
        {
            foreach (ConnectionStringSettings cnString in ConfigurationManager.ConnectionStrings)
            {
                var name = _keyReplacer.Execute(cnString.Name);
                var value = _valueReplacer.Execute(cnString.ConnectionString);
                var provider = _valueReplacer.Execute(cnString.ProviderName);
                 ConfigManager.ConnectionStrings.Set(name, value, provider);
            }

            var keyList = new List<string>();
            keyList.AddRange(ConfigurationManager.AppSettings.AllKeys);
            for (int x = 0; x < keyList.Count; x++)
            {
                var key = keyList[x];
                key = _keyReplacer.Execute(key);
                var value = _valueReplacer.Execute(ConfigurationManager.AppSettings[key]);
                ConfigManager.SetAppSetting(key, value);
            }

            try
            {
                if (outputResolvedValues.HasValue == false)
                {
                    outputResolvedValues = CoreConfig.Configuration.OutputResolvedConfiguration;
                }
                if (outputResolvedValues.Value)
                {
                    Log.Internal.Write(EffectiveConnectionStrings());
                    Log.Internal.Write(EffectiveAppSettings());
                }
            }
            catch(Exception ex)
            {
                Log.Internal.Write(ex);
                return false; //ignore any output errors
            }
            return true;
        }

        /// <summary>
        /// Adds or updates an appSettings value
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appValue"></param>
        public static void SetAppSetting(string appKey, string appValue)
        {
            var readonlyField = typeof(NameObjectCollectionBase).GetField("_readOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            var settings = ConfigurationManager.AppSettings;

            readonlyField.SetValue(settings, false);
            var x = readonlyField.GetValue(settings);

            if (ConfigurationManager.AppSettings[appKey] != null)
            {
                ConfigurationManager.AppSettings[appKey] = appValue;
            }
            else
            {
                ConfigurationManager.AppSettings.Set(appKey, appValue);
            }

            readonlyField.SetValue(settings, true);
        }

        /// <summary>
        /// Return a formatted string of all application settings.  Each line is delimited by Environment.NewLine
        /// </summary>
        /// <returns>List of all AppSettings</returns>
        public static string EffectiveAppSettings()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                sb.AppendLine("AppSettings['{0}'] = '{1}'", key, ConfigurationManager.AppSettings[key]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Return a formatted string of all connection strings.  Each line is delimited by Environment.NewLine
        /// </summary>
        /// <returns>List of connection strings</returns>
        public static string EffectiveConnectionStrings()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ConnectionStringSettings setting in ConfigurationManager.ConnectionStrings)
            {
                sb.AppendLine("ConnectionString['{0}'] = '{1}'",
                    setting.Name,
                    setting.ConnectionString);
            }
            return sb.ToString();
        }
    }
}