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
using HashTag.Diagnostics;
using System.Reflection;
using HashTag.Configuration;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Threading;
using System.Windows.Forms;
using HashTag.Text;
using HashTag.Properties;

namespace HashTag
{
    /// <summary>
    /// Configuration settings for HashTag library.  Note:  Some settings use direct calls to ConfigurationManager[] since they return
    /// bootstrapping values.  By default, always use ConfigManager instead.
    /// </summary>
    public partial class CoreConfig
    {
        public class KeyIds
        {
            public class Diagnostics
            {
                public const string LogFilePath = "HashTag.Diagnostics.LogfilePath";
                public const string ApplicationLogLevels = "HashTag.Diagnostics.Default.ApplicationLogLevels";
                public const string LastChanceLogLevels = "HashTag.Diagnostics.LocalLogLevels";
                public const string ConnectorType = "HashTag.Diagnostics.Connector";
                public const string MessageBufferBlockSize = "HashTag.Diagnostics.MessageBufferBlockSize";
                public const string MessageBufferIntervalMs = "HashTag.Diagnostics.MessageBufferIntervalMs";
                public const string MessageBufferWriteTimeoutMs = "HashTag.Diagnostics.MessageBufferWriteTimeoutMs";
                public const string InternalHandlerTimeOutMs = "HashTag.Diagnostics.InternalHandlerTimeOutMs";
                public const string InternalLogLevels = "HashTag.Diagnostics.InternalLogLevels";
            }
            
            public class Core
            {
                public const string IsCaseSenstive = "HashTag.Core.IsCaseSensitive";
                /// <summary>
                /// "HashTag.Application.Environment"
                /// </summary>
                public const string AppEnvironment = "HashTag.Application.Environment";
                /// <summary>
                /// HashTag.Application.Name
                /// </summary>
                public const string AppName = "HashTag.Application.Name";
                public const string DefaultValueSuffix = "HashTag.Core.Setting.DefaultValueSuffix";
            }
            
            public class RemoteConfig
            {
                public const string ServiceUriConnectionString = "HashTag.RemoteConfig.ServiceUrl";
                public const string DefaultMergeOptions = "HasthTag.RemoteConfig.Default.MergeOptions";
                public const string DefaultServiceProvider = "HashTag.RemoteConfig.ServiceProvider";
            }

            public const string OutputResolvedConfiguration = "HashTag.Config.OutputResolvedConfiguration";
        }

        private static T _resolveAppSettingsValueFromConfigOrResourceFile<T>(string keyId)
        {
            //get redirected app settings key from resource file or use keyId as app settings key if not redirected
            var activeKey = _findConfigFileActiveSettingKey(keyId);

            //look up value in appSettings
            var settingValue = ConfigManager.AppSetting<string>(activeKey, "~~~~");

            // if value is not found in .config then get default from resource file
            if (string.Compare(settingValue, "~~~~") == 0)
            {
                settingValue = _findDefaultValueFromResourceFile(activeKey);
            }

            // resolve and replacement parameter tokens
            settingValue = ConfigManager._valueReplacer.Execute(settingValue);

            try
            {
                return Transform.ConvertValue<T>(settingValue);
            }
            catch
            {
                throw new ConfigurationErrorsException(string.Format(CoreResources.MSG_CoreConfig_ConversionError, activeKey, settingValue, typeof(T).FullName));
            }
        }

        private static string _findDefaultValueFromResourceFile(string activeKey)
        {
            var settingValue = CoreResources.ResourceManager.GetString(string.Format("{0}.{1}", activeKey, CoreResources.HashTag_Core_Setting_DefaultValueSuffix));
            settingValue = _resourceReplacer.Execute(settingValue);
            return settingValue;
        }


        //private static T _resolveConnectionStringSettingsFromConfigOrResourceFile<T>(string keyId)
        //{
        //    var activeKey = _findConfigFileActiveSettingKey(keyId);

        //    //look up value in appSettings
        //    var cnStringSettings = ConfigurationManager.ConnectionStrings[activeKey];

        //    // if value is not found in .config then get default from resource file
        //    if (cnStringSettings == null)
        //    {
        //        var cnStringSettingsConnectionString = CoreResources.ResourceManager.GetString(string.Format("{0}.{1}", activeKey, CoreResources.HashTag_Core_Setting_DefaultValueSuffix));
        //        var cnStringSettingsProvider = CoreResources.ResourceManager.
        //        cnStringSettings = _resourceReplacer.Execute(cnStringSettingsConnectionString);
        //    }

        //    // resolve and replacement parameter tokens
        //    cnStringSettings = ConfigManager._valueReplacer.Execute(cnStringSettings);

        //    try
        //    {
        //        return Transform.ConvertValue<T>(cnStringSettings);
        //    }
        //    catch
        //    {
        //        throw new ConfigurationErrorsException(string.Format(CoreResources.MSG_CoreConfig_ConversionError, activeKey, cnStringSettings, typeof(T).FullName));
        //    }
        //}

        private static string _findConfigFileActiveSettingKey(string keyId)
        {
            //get redirected app settings key from resource file or use keyId as app settings key if not redirected
            var redirectedKey = CoreResources.ResourceManager.GetString(string.Format("{0}.{1}", keyId, CoreResources.HashTag_Core_Setting_RedirectToKeyToken)) as string;
            redirectedKey = _resourceReplacer.Execute(redirectedKey);

            var activeKey = redirectedKey ?? keyId;
            activeKey = _keyReplacer.Execute(activeKey);
            return activeKey;
        }
        

        private static TokenReplacer _resourceReplacer = new TokenReplacer(token =>
        {
            return CoreResources.ResourceManager.GetString(token.InternalToken);
        }, "%(", ")");

        /// <summary>
        /// '"HashTag.com/2014/10/'
        /// </summary>
        public const string WcfNamespace = "HashTag.com/2014/10/";

        private static bool? _ignoreCase;
        /// <summary>
        /// true
        /// </summary>
        public static bool IGNORECASE_FLAG
        {
            get
            {
                if (_ignoreCase != null) return _ignoreCase.Value;
                _ignoreCase = _resolveAppSettingsValueFromConfigOrResourceFile<bool>(KeyIds.Core.IsCaseSenstive);
                return _ignoreCase.Value;
            }
        }
        
        public static bool Initialize()
        {

            foreach (ConnectionStringSettings cnString in ConfigurationManager.ConnectionStrings)
            {
                var name = !string.IsNullOrEmpty(cnString.Name) ? _keyReplacer.Execute(cnString.Name) : cnString.Name;
                var value = _valueReplacer.Execute(cnString.ConnectionString);
                var provider = _valueReplacer.Execute(cnString.ProviderName);
                ConfigManager.ConnectionStrings.Set(name, value, provider);
            }

            var keyList = new List<string>();
            keyList.AddRange(ConfigurationManager.AppSettings.AllKeys);
            for (int x = 0; x < keyList.Count; x++)
            {
                var key = _keyReplacer.Execute(keyList[x]);
                ConfigurationManager.AppSettings[key] = _valueReplacer.Execute(ConfigurationManager.AppSettings[key]);
            }
            return true;
        }

        static string _activeEnvironment = string.Empty;
        /// <summary>
        /// Active environment as configured in .config file (HashTag.Application.Environment)
        /// </summary>
        public static string ActiveEnvironment
        {
            get
            {
                if (string.IsNullOrEmpty(_activeEnvironment) == false)
                {
                    return _activeEnvironment;
                }
                _activeEnvironment = ConfigurationManager.AppSettings[KeyIds.Core.AppEnvironment];

                if (string.IsNullOrEmpty(_activeEnvironment) == true)
                {
                    _activeEnvironment = System.Environment.MachineName;
                }
                
                return _activeEnvironment;
            }
        }

        private static string _applicationKey;
        /// <summary>
        /// Unique key for this application.  Often used in event logging and error handling (optional: HashTag.Application.Key)
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationKey) == false)
                {
                    return _applicationKey;
                }

                _applicationKey = ConfigurationManager.AppSettings[KeyIds.Core.AppName];
                if (string.IsNullOrEmpty(_applicationKey) == true)
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                    {
                        asm = Assembly.GetCallingAssembly();
                    }
                    if (asm == null)
                    {
                        asm = Assembly.GetExecutingAssembly();
                    }
                    _applicationKey = asm.GetName().Name;
                }
                return _applicationKey;
            }
        }

        private static Guid _activityId;
        /// <summary>
        /// Get (or creates if not already set) activity id on CorrelationManager
        /// </summary>
        public static Guid ActivityId
        {
            get
            {
                _activityId = Trace.CorrelationManager.ActivityId;
                if (_activityId == Guid.Empty)
                {
                    _activityId = Guid.NewGuid();
                    Trace.CorrelationManager.ActivityId = _activityId;
                }
                return _activityId;
            }
            set
            {
                _activityId = value;
                Trace.CorrelationManager.ActivityId = _activityId;
            }
        }

        /// <summary>
        /// Return the active user probing for: HttpContext.Currentuser, Thread.CurrentPrincipal, Environment.UserName
        /// </summary>
        public static string ActiveUserName
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                {
                    return HttpContext.Current.User.Identity.Name;
                }

                if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && !string.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity.Name))
                {
                    return Thread.CurrentPrincipal.Identity.Name;
                }
                return Environment.UserName;
            }
        }

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
                    return string.Format(CoreResources.MSG_CoreConfig_TokenNotFound, token.InternalToken);
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
                    return string.Format(CoreResources.MSG_CoreConfig_TokenNotFound, token.InternalToken);
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
                    return string.Format(CoreResources.MSG_CoreConfig_TokenNotFound, token.InternalToken);
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
                    return string.Format(CoreResources.MSG_CoreConfig_TokenNotFound, token.InternalToken);
            }
            return token.OuterToken;
        }, "${", "}", "$[", "]");
    }
}