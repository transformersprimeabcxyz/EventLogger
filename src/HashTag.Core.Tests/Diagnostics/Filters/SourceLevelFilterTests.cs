using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashTag.Diagnostics.Filters;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
namespace HashTag.Core.Tests.Diagnostics.Filters
{
    [TestClass]
    public class SourceLevelFilterTests
    {
        [TestMethod]
        public void SourceLevel_Constructor__Default__Off()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();

            Assert.AreEqual<SourceLevels>(SourceLevels.Off, _filter.SourceLevels);
        }

        [TestMethod]
        public void SourceLevel_Constructor_Initialize__NullConfig__Error()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();

            try
            {
                _filter.Initialize(null);
            }
            catch (ConfigurationErrorsException ex)
            {
                Assert.AreEqual<string>(SourceLevelFilter.Consts.Error_ConfigurationRequried, ex.Message);
            }
        }

        [TestMethod]
        public void SourceLevel_Constructor_Initialize__EmptyConfig__Error()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();
            var config = new Dictionary<string, string>();
            try
            {
                _filter.Initialize(config);
            }
            catch (ConfigurationErrorsException ex)
            {
                Assert.AreEqual<string>(SourceLevelFilter.Consts.Error_MissingRequiredKey, ex.Message);
            }
        }

        [TestMethod]
        public void SourceLevel_Constructor_Initialize__BadConfigValue__Error()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();
            var config = new Dictionary<string, string>();
            config.Add(SourceLevelFilter.Consts.Config_SourceLevel, Guid.Empty.ToString());
            try
            {
                _filter.Initialize(config);
            }
            catch (ConfigurationErrorsException ex)
            {
                Assert.AreEqual<string>(SourceLevelFilter.Consts.Error_BadParse, ex.Message);
            }
        }

        [TestMethod]
        public void SourceLevel_Constructor_Initialize__Warning__Success()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();
            var config = new Dictionary<string, string>();
            config.Add(SourceLevelFilter.Consts.Config_SourceLevel, "Warning");

            _filter.Initialize(config);
            Assert.AreEqual<SourceLevels>(SourceLevels.Warning,_filter.SourceLevels);
        }

        [TestMethod]
        public void SourceLevel_Constructor_Initialize__Warning_Information__Success()
        {
            SourceLevelFilter _filter = new SourceLevelFilter();
            var config = new Dictionary<string, string>();
            config.Add(SourceLevelFilter.Consts.Config_SourceLevel, "Warning,Information");

            _filter.Initialize(config);
            Assert.AreEqual<SourceLevels>(SourceLevels.Warning|SourceLevels.Information,_filter.SourceLevels);
        }
    }
}
