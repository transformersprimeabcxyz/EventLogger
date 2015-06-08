using HashTag.Diagnostics;
using HashTag.Properties;
using HashTag.Reflection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

namespace HashTag.Configuration
{
    public  partial class ConfigManager
    {
        public static void LoadSetingsFromService(ConfigMergeOptions mergeOptions = ConfigMergeOptions.ConfigFileWins)
        {
            IConfigProvider provider = getSetingsProvider();
            if (provider == null) //provider not configured so we don't do anything.  This is often the use-case when not using a remote configuration service
            {
                return; 
            }

            var providerParams = new Dictionary<string, string>()
            {
                {"groups","appSettings,connectionStrings"},
                {"assmName",Assembly.GetEntryAssembly().GetName().Name},
                {"assmVersion",Assembly.GetEntryAssembly().GetName().Version.ToString()},
                {"threadIdentity",Thread.CurrentPrincipal.Identity.Name},
                {"userIdentity",CoreConfig.ActiveUserName},
                {"host",Environment.MachineName},
                {"appName",CoreConfig.ApplicationName},
                {"envName",CoreConfig.ActiveEnvironment},
                {"domain",Environment.UserDomainName}
            };
            
            var settingsFromProvider = provider.LoadSettings(providerParams);

            applyProviderSettingsToConfiguaration(settingsFromProvider, mergeOptions);
        }

        private static void applyProviderSettingsToConfiguaration(List<RemoteConfigSetting> settingsFromProvider, ConfigMergeOptions mergeOptions)
        {
            foreach(var setting in settingsFromProvider)
            {
                switch(setting.SettingGroup.ToLowerInvariant())
                {
                    case "appsettings": applySettingAsAppSetting(setting, mergeOptions);
                        break;
                    case "connectionStrings": applySettingAsConnectionString(setting, mergeOptions);
                        break;
                    default:
                        Logger.Internal.Write(CoreResources.MSG_CoreConfig_UnexpectedGroupName, setting.SettingGroup);
                        throw ExceptionFactory.New<ConfigurationErrorsException>(CoreResources.MSG_CoreConfig_UnexpectedGroupName, setting.SettingGroup);
                }
            }
        }

        private static void applySettingAsConnectionString(RemoteConfigSetting remoteSetting, ConfigMergeOptions mergeOptions)
        {
            var remoteSettingName = remoteSetting["name"].Value;
            bool configSettingExists = ConfigurationManager.ConnectionStrings[remoteSettingName] != null;
            if (configSettingExists && mergeOptions == ConfigMergeOptions.ConfigFileWins) return;

            var remoteCnString = remoteSetting["connectionString"].Value;
            var remoteProvider= remoteSetting["providerName"].Value;

            ConfigManager.ConnectionStrings.Set(remoteSettingName, remoteCnString, remoteProvider);
        }

        private static void applySettingAsAppSetting(RemoteConfigSetting remoteSetting, ConfigMergeOptions mergeOptions)
        {
            var remoteSettingName = remoteSetting["name"].Value;
            var remoteSettingValue = remoteSetting["value"].Value;

            bool configSettingExists = ConfigurationManager.AppSettings.Get(remoteSettingName) != (string)null;
            if (configSettingExists == true && mergeOptions == ConfigMergeOptions.ConfigFileWins) return;

            ConfigManager.SetAppSetting(remoteSettingName, remoteSettingValue);

        }

        private static IConfigProvider getSetingsProvider()
        {
            var cnStringSettings = CoreConfig.Configuration.RemoteConfigurationProvider; //NOTE: might need to change this to handle cases where remote configuration is not configured
            if (cnStringSettings == null) return null;

            Logger.Internal.Write(CoreResources.MSG_CoreConfig_UsingRemoteSetting, cnStringSettings.Name, cnStringSettings.ConnectionString, cnStringSettings.ProviderName);

            try
            {
                var provider = ProviderFactory<IConfigProvider>.GetInstance(cnStringSettings.ProviderName, cnStringSettings.ConnectionString);
                return provider;
            }
            catch(Exception ex)
            {
                Logger.Internal.Write(ex);
                return null;
            }
        }
    }
}
