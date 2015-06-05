using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashTag.Diagnostics;
using System.Diagnostics;
namespace HashTag.Core.Tests.Diagnostics.Extensions
{
    [TestClass]
    public class IsEnabledForTests
    {
        [TestMethod]
        public void SourceLevels_IsEnabledFor__Off_Warning__False()
        {
            var sl = SourceLevels.Off;
            var tl = TraceEventType.Warning;

            Assert.IsFalse(sl.IsEnabledFor(tl));
        }
        [TestMethod]
        public void SourceLevels_IsEnabledFor__Warning_Warning__True()
        {
            var sl = SourceLevels.Warning;
            var tl = TraceEventType.Warning;

            Assert.IsTrue(sl.IsEnabledFor(tl));
        }
        [TestMethod]
        public void SourceLevels_IsEnabledFor__Information_Warning__True()
        {
            var sl = SourceLevels.Information;
            var tl = TraceEventType.Warning;

            Assert.IsTrue(sl.IsEnabledFor(tl));
        }
        [TestMethod]
        public void SourceLevels_IsEnabledFor__Information_Verbose__False()
        {
            var sl = SourceLevels.Information;
            var tl = TraceEventType.Verbose;

            Assert.IsFalse(sl.IsEnabledFor(tl));
        }
        [TestMethod]
        public void SourceLevels_IsEnabledFor__All_Start__True()
        {
            var sl = SourceLevels.All;
            var tl = TraceEventType.Start;

            Assert.IsTrue(sl.IsEnabledFor(tl));
        }

    }
}
