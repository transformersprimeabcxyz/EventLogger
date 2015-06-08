using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Filters
{
    public class SourceLevelFilter : ILogEventFilter
    {
        public class Consts
        {
            
            /// <summary>
            /// 'SourceLevels
            /// </summary>
            public const string Config_SourceLevel = "SourceLevels";

            /// <summary>
            /// Configuration object required.
            /// </summary>
            public const string Error_ConfigurationRequried = "Configuration object required.";

            /// <summary>
            /// Unable to find required key: 'SourceLevels' in configuration
            /// </summary>
            public const string Error_MissingRequiredKey = "Unable to find required key: 'SourceLevels' in configuration";

            /// <summary>
            /// Key SourceLevels must be a comma delimited list of one or more of: All, Off, Critical, Error, Warning, Information, Verbose, ActivityTracing
            /// </summary>
            public const string Error_BadParse = "Key['"+Config_SourceLevel+"'] must be a comma delimited list of one or more of: All, Off, Critical, Error, Warning, Information, Verbose, ActivityTracing";
        }
        public SourceLevelFilter()
        {
            SourceLevels = System.Diagnostics.SourceLevels.Off;
        }
       
        public void Initialize(IDictionary<string, string> config)
        {
            if (config == null) throw new ConfigurationErrorsException(Consts.Error_ConfigurationRequried);

            if (!config.Keys.Any(key=>string.Compare(Consts.Config_SourceLevel,key)==0))
            {
                throw new ConfigurationErrorsException(Consts.Error_MissingRequiredKey);
            }

            var sourceLevelsValue = config[Consts.Config_SourceLevel];

            SourceLevels levels;

            var parseResult = Enum.TryParse<SourceLevels>(sourceLevelsValue, out levels);
            if (parseResult == false)
            {
                throw new ConfigurationErrorsException(Consts.Error_BadParse);
            }
            SourceLevels = levels;
            
        }

        public SourceLevels SourceLevels { get; set; }

        public bool Matches(LogEvent le)
        {
            return SourceLevels.IsEnabledFor(le.EventType);            
        }

       
    }
}
