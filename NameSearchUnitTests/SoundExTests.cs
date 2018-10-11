using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NameSearch;

namespace ParadigmUnitTests
{
    [TestClass]
    public class SoundExTests
    {
        [TestMethod]
        public void TestSoundEx()
        {
            string result = SoundExDb.GetSoundEx("Microsoft");
            Assert.AreEqual("M262", result);
        }

        [TestMethod]
        public void TestSoundExAlgo()
        {
            string result = SoundExHash.SoundexWord("Microsoft");
            Assert.AreEqual("M262", result);
            result = SoundExHash.SoundexWord("Apple");
            Assert.AreEqual("A140", result);
            result = SoundExHash.SoundexWord("Super");
            Assert.AreEqual("S160", result);
            result = SoundExHash.SoundexWord("Souper");
            Assert.AreEqual("S160", result);
            List<string> results = SoundExHash.SoundexList("Souper Duper Trooper");
            result = results[0];
            Assert.AreEqual("S160", result);
            result = results[1];
            Assert.AreEqual("D160", result);
            result = results[2];
            Assert.AreEqual("T616", result);

            return;
        }

        [TestMethod]
        public void TestGetCompanyRecords()
        {
            SoundExHash soundExDb = new SoundExHash();

            DateTime start = DateTime.Now;
            
            Dictionary<string, CompanyRecord> companyRecordsList = soundExDb.GetCompanyRecords();

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;

            return;
        }

        [TestMethod]
        public void TestFindSimilarCompanyRecords()
        {
            SoundExHash soundExDb = new SoundExHash();

            DateTime start = DateTime.Now;

            Dictionary<string, CompanyRecord> companyRecordsList = soundExDb.GetCompanyRecords();

            DateTime start2 = DateTime.Now;
            TimeSpan span1 = start2 - start;


            DateTime start3 = DateTime.Now;
            TimeSpan span2 = start3 - start2;

            List<CompanyRecord> companySoundMatchList = soundExDb.FindSimilarCompanyRecords("Walt Disney");

            Assert.AreEqual("Disney Walt", companySoundMatchList[0].CompanyName);
            Assert.AreEqual("Dizney Walt", companySoundMatchList[1].CompanyName);

            DateTime start4 = DateTime.Now;
            TimeSpan span3 = start4 - start3;

            TimeSpan span4 = start4 - start;

            Trace.WriteLine("{0}", span4.ToString());

            return;
        }
    }
}
