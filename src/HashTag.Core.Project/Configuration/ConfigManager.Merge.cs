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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HashTag.Configuration
{
    public sealed partial class ConfigManager
    {
        /// <summary>
        /// Merge all .config files in the executing folder into the runtime configuration.  Automatically activates
        /// new configuration in the .Net framework
        /// </summary>
        /// <returns>An xml configuration of the merged configuration.  Returned value is not needed to use merged settings but may be used for logging, etc.</returns>
        public static XmlDocument MergeConfigFiles()
        {
            //-------------------------------------------------------
            // collect all .config files which will be merged together
            //-------------------------------------------------------
            string targetFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile; // get default .config file
            string baseDir = AppDomain.CurrentDomain.BaseDirectory; // this is the operating system location for executable
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(targetFileName));

            var fileList = Directory.GetFiles(baseDir, "*.config").ToList(); // retrieve all .config files from executing folder
            if (fileList != null && fileList.Count > 0) // now remove *.exe.config type files so we don't get duplicates
            {
                var executingAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
                List<string> killWords = new List<string>() { "vshost", executingAssemblyName, System.AppDomain.CurrentDomain.FriendlyName };
                for (int x = fileList.Count - 1; x >= 0; x--)
                {
                    string fileName = Path.GetFileName(fileList[x]);
                    if (killWords.Any(w => fileName.Contains(w)))
                    {
                        fileList.RemoveAt(x);
                    }
                }
            }

            //-------------------------------------------------------
            //  merge all .config files into a consolidated version
            //-------------------------------------------------------
            var mergedConfigDoc = MergeConfigFiles(targetFileName, fileList.ToArray());
            mergedConfigDoc.Save(targetFileName);

            //-------------------------------------------------------
            // now refresh configuration so framework can access
            //  merged values
            //-------------------------------------------------------
            ConfigurationManager.RefreshSection("configSections");

            System.Configuration.Configuration config =
                 ConfigurationManager.OpenExeConfiguration(
                 ConfigurationUserLevel.None) as System.Configuration.Configuration;

            var rg = config.RootSectionGroup;
            var rs = rg.Sections;

            foreach (var sectionKey in rs.Keys)
            {
                //Console.WriteLine("S: {0}", k);  //uncomment this line to see what sections are being refreshed after the merge
                ConfigurationManager.RefreshSection(sectionKey.ToString());
            }

            return mergedConfigDoc;
        }

        /// <summary>
        /// Create a combined configuration file by combining is limited to elements and attributes
        /// </summary>
        /// <param name="targetConfigFileName">Source file into which other settings will be copied</param>
        /// <param name="sourceFileNames">List of fully qualified file names of configuration fragments to be copied into <paramref name="targetConfigFileName"/></param>
        /// <returns>Document with combined element and attributes</returns>
        public static XmlDocument MergeConfigFiles(string targetConfigFileName, params string[] sourceFileNames)
        {
            XmlDocument targetDoc = new XmlDocument();
            targetDoc.LoadXml(File.ReadAllText(targetConfigFileName));
            if (sourceFileNames != null)
            {
                for (int x = 0; x < sourceFileNames.Length; x++)
                {
                    var sourceDoc = new XmlDocument();
                    sourceDoc.LoadXml(File.ReadAllText(sourceFileNames[x]));
                    targetDoc.Merge(sourceDoc);
                }
            }
            return targetDoc;
        }

    }
}