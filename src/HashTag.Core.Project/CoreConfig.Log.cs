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
using HashTag.Diagnostics;
using System.IO;
using HashTag.Reflection;
using HashTag.Text;
using DotNetConfig = System.Configuration;
using System.Configuration;
using System.Web;
using System.Diagnostics;
using System.Resources;
using System.Globalization;
using HashTag.Properties;
namespace HashTag
{
    /// <summary>
    /// Configuration values for HashTag.Core classes
    /// </summary>
    public partial class CoreConfig
    {
      
        /// <summary>
        /// Logging configuration for LastChance and global logging boot strapping values
        /// </summary>
        public static class Log
        {
            private static ILogConnector _activeConnector;

            /// <summary>
            /// Load the configured persister for logging sub-system.  Default: Internal rolling log file persister
            /// </summary>
            /// <remarks>
            /// If not configured, probe the configuration file for known types
            /// </remarks>
            public static ILogConnector ActiveConnector
            {
                get
                {
                    if (_activeConnector != null)
                    {
                        return _activeConnector;
                    }

                    string persisterClassName = _resolveAppSettingsValueFromConfigOrResourceFile<string>(KeyIds.Diagnostics.ConnectorType);

                    try
                    {
                        _activeConnector = ProviderFactory<ILogConnector>.GetInstance(persisterClassName);
                        if (_activeConnector == null)
                        {
                            HashTag.Diagnostics.Log.Internal.Write(TraceEventType.Error, CoreResources.MSG_Core_Unable_CreateInstance, persisterClassName, typeof(ILogConnector).FullName);
                            throw new DotNetConfig.ConfigurationErrorsException(TextUtils.StringFormat(CoreResources.MSG_Core_Unable_CreateInstance, persisterClassName, typeof(ILogConnector).FullName));
                        }
                        _activeConnector.Initialize();
                    }
                    catch (Exception ex)
                    {
                        HashTag.Diagnostics.Log.Internal.Write(ex, CoreResources.MSG_Core_Unable_CreateInstance, persisterClassName,typeof(ILogConnector).FullName);
                        throw new DotNetConfig.ConfigurationErrorsException(TextUtils.StringFormat(CoreResources.MSG_Core_Unable_CreateInstance, persisterClassName, typeof(ILogConnector).FullName), ex);
                    }
                    HashTag.Diagnostics.Log.Internal.Write(CoreResources.MSG_Diagnostics_Using_LogConnector, persisterClassName);
                    return _activeConnector;
                }
                set
                {
                    _activeConnector = value;
                }
            }

      
            static int? _messageBufferCacheSize;
            public static int MessageBufferCacheSize
            {
                get
                {
                    if (_messageBufferCacheSize.HasValue) return _messageBufferCacheSize.Value;

                    _messageBufferCacheSize = _resolveAppSettingsValueFromConfigOrResourceFile<int>(KeyIds.Diagnostics.MessageBufferBlockSize);
                    return _messageBufferCacheSize.Value;
                }
            }

            static int? _messageBufferIntervalMs;
            public static int MessageBufferIntervalMs
            {
                get
                {
                    if (_messageBufferIntervalMs.HasValue) return _messageBufferIntervalMs.Value;
                    _messageBufferIntervalMs = _resolveAppSettingsValueFromConfigOrResourceFile<int>(KeyIds.Diagnostics.MessageBufferIntervalMs);
                    return _messageBufferIntervalMs.Value;
                }
            }

            static int? _messageBufferWriteTimeoutMs = 10000;
            public static int MessageBufferWriteTimeoutMs
            {
                get
                {
                    if (_messageBufferWriteTimeoutMs.HasValue) return _messageBufferWriteTimeoutMs.Value;
                    _messageBufferWriteTimeoutMs = _resolveAppSettingsValueFromConfigOrResourceFile<int>(KeyIds.Diagnostics.MessageBufferWriteTimeoutMs);
                    return _messageBufferWriteTimeoutMs.Value;
                }
            }

            static SourceLevels? _applicationLogLevels;
            public static SourceLevels ApplicationLogLevels
            {
                get
                {
                    if (_applicationLogLevels.HasValue) return _applicationLogLevels.Value;
                    _applicationLogLevels = _resolveAppSettingsValueFromConfigOrResourceFile<SourceLevels>(KeyIds.Diagnostics.ApplicationLogLevels);
                    return _applicationLogLevels.Value;
                }
            }

            static SourceLevels? _lastChanceLogLevels = SourceLevels.Warning;
            public static SourceLevels LastChanceLogLevels
            {
                get
                {
                    if (_lastChanceLogLevels.HasValue) return _lastChanceLogLevels.Value;
                    _lastChanceLogLevels = _resolveAppSettingsValueFromConfigOrResourceFile<SourceLevels>(KeyIds.Diagnostics.LastChanceLogLevels);
                    return _lastChanceLogLevels.Value;
                }
            }

            static SourceLevels? _internalLogLogLevel;
            public static SourceLevels InternalLogLogLevels
            {
                get
                {
                    if (_internalLogLogLevel.HasValue) return _internalLogLogLevel.Value;
                    _internalLogLogLevel = _resolveAppSettingsValueFromConfigOrResourceFile<SourceLevels>(KeyIds.Diagnostics.InternalLogLevels);
                    return _internalLogLogLevel.Value;
                }
            }

            static int? _internalHandlerTimeOutMs;
            public static int InternalHandlerTimeOutMs
            {
                get
                {
                    if (_internalHandlerTimeOutMs.HasValue) return _internalHandlerTimeOutMs.Value;
                    _internalHandlerTimeOutMs = _resolveAppSettingsValueFromConfigOrResourceFile<int>(KeyIds.Diagnostics.InternalHandlerTimeOutMs);
                    return _internalHandlerTimeOutMs.Value;
                }
            }

            static string _logfilepath;
            public static string LogfilePath
            {
                get
                {
                    if (!string.IsNullOrEmpty(_logfilepath)) return _logfilepath;

                    var configuredPath = _resolveAppSettingsValueFromConfigOrResourceFile<string>(KeyIds.Diagnostics.LogFilePath);

                    if (!string.IsNullOrWhiteSpace(configuredPath))
                    {

                        if (Directory.Exists(configuredPath) == false)
                        {
                            try
                            {
                                Directory.CreateDirectory(configuredPath);
                                _logfilepath = configuredPath;
                                return _logfilepath;
                            }
                            catch
                            {

                            }
                        }
                    }

                    if (HttpContext.Current != null)
                    {
                        var appDataPath = HttpContext.Current.Server.MapPath("~/App_Data");
                        if (Directory.Exists(appDataPath))
                        {
                            var webLogPath = Path.Combine(appDataPath, "Logs");
                            if (Directory.Exists(webLogPath))
                            {
                                _logfilepath = webLogPath;
                                return _logfilepath;
                            }
                            else
                            {
                                try
                                {
                                    Directory.CreateDirectory(webLogPath);
                                }
                                catch
                                {
                                    // ignore error and continue probing
                                }
                            }
                        }
                    }

                    string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    string tmpPath = Path.Combine(path, "Logs");
                    if (Directory.Exists(tmpPath))
                    {
                        _logfilepath = tmpPath;
                        return _logfilepath;
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(tmpPath);
                            _logfilepath = tmpPath;
                            return _logfilepath;
                        }
                        catch
                        {
                            // ignore error and continue probing
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_logfilepath)) // dump files in default folder containing .dlls as last resort
                    {
                        _logfilepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    }
                    return _logfilepath;
                }
            }
        }
    }
}