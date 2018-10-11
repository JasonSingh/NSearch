using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using STFO.Logic.Custom;

namespace ParadigmUnitTests
{
    [TestClass]
    public class NameSearchMainTests
    {
        [TestMethod]
        public void TestNameSearchMain()
        {
            NameSearchMain nameSearchMain = new NameSearchMain("The Walt Disney");

            List<string> results = nameSearchMain.GetCleansedCompanyName();

            string result = NormalizeName.GetCleansedCompanyName("The Walt Disney");

            Assert.AreEqual<string>(result, results[0]);

            return;
        }

        [TestMethod]
        public void TestProcessCompanyRecordScores()
        {
            NameSearchRatios nameSearchRatios = new NameSearchRatios(5, 4, 2, 0, 0, 2);
            NameSearchScores nameSearchScores = new NameSearchScores(nameSearchRatios);

            Dictionary<string, CompanyRecordScore> results = nameSearchScores.ProcessCompanyRecordScores("Walt Disney");

            return;
        }
    }
}
