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
using HashTag.Configuration;

namespace HashTag
{
    public static partial class CoreConfig
    {
        /// <summary>
        /// Values to bootstrap configuration providers
        /// </summary>
        public class Configuration
        {
            ///// <summary>
            ///// Fully qualified name of assembly ('type','assembly') that will provide external configuration data to application. Assembly must implement IInternalConfigSystem. (Key: HashTag.Configuration.ExternalSource)
            ///// </summary>
            ///// <remarks>
            ///// Used ConfigurationManager reference instead of HashTag.ConfigManager since this value is read at library bootstrapping time and ConfigManager isn't fully initialized
            ///// </remarks>
            //public static string ExternalSource
            //{
            //    get
            //    {
            //        string configSource = ConfigurationManager.AppSettings["HashTag.Configuration.ExternalSource"];
            //        throw new ConfigurationErrorsException();
            //        //if (configSource == null || string.IsNullOrEmpty(configSource) == true)
            //        //{
            //        //    HashTag.Diagnostics.Log.InternalLog(Diagnostics.MessageLevel.Warning, "Unable to find <appSettings> key 'HashTag.Configuration.ExternalSource'.  External configuration source might be disabled");
            //        //}
            //        return configSource;
            //    }
            //}

            private static bool? _outputResolvedConfiguration;
            /// <summary>
            /// Config: HashTag.Configuration.OutputResolvedConfiguration
            /// </summary>
            public static bool OutputResolvedConfiguration
            {
                get
                {
                    if (_outputResolvedConfiguration.HasValue) return _outputResolvedConfiguration.Value;
                    _outputResolvedConfiguration = _resolveAppSettingsValueFromConfigOrResourceFile<bool>(KeyIds.OutputResolvedConfiguration);
                    return _outputResolvedConfiguration.Value;
                }
            }

            private static ConfigMergeOptions? _mergeOptions;
            public static ConfigMergeOptions MergeOptions
            {
                get
                {
                    if (_mergeOptions.HasValue == true) return _mergeOptions.Value;
                    _mergeOptions = _resolveAppSettingsValueFromConfigOrResourceFile<ConfigMergeOptions>(KeyIds.RemoteConfig.DefaultMergeOptions);
                    return _mergeOptions.Value;
                }
            }

            private static ConnectionStringSettings _cnSettings;
            public static ConnectionStringSettings RemoteConfigurationProvider
            {
                get
                {
                    if (_cnSettings != null) return _cnSettings;
                    var settingFromConfigFile= ConfigurationManager.ConnectionStrings[ConfigurationServiceConnectionStringsKey];
                    if (settingFromConfigFile == null) return null;

                    var name = settingFromConfigFile.Name;
                    var cnString = settingFromConfigFile.ConnectionString;
                    var providerName = settingFromConfigFile.ProviderName;

                    if (string.IsNullOrWhiteSpace(providerName))
                    {
                        providerName = _findDefaultValueFromResourceFile(KeyIds.RemoteConfig.DefaultServiceProvider);
                    }

                    cnString = _valueReplacer.Execute(cnString);
                    providerName = _valueReplacer.Execute(providerName);

                    _cnSettings = new ConnectionStringSettings(name, cnString, providerName);
                    return _cnSettings;
                }
            }
            public static string ConfigurationServiceConnectionStringsKey
            {
                get
                { 
                    return _findConfigFileActiveSettingKey(KeyIds.RemoteConfig.ServiceUriConnectionString);
                }
            }
        }
    }
}