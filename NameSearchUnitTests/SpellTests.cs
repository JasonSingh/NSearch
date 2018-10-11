using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NameSearch;

namespace ParadigmUnitTests
{
    /// <summary>
    /// Summary description for SpellTests
    /// </summary>
    [TestClass]
    public class SpellTests
    {
        public SpellTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestNonAlphas()
        {
            Spelling spelling = new Spelling();

            string result = Spelling.FindNonAlphas();
        }

        [TestMethod]
        public void TestSpellBasicWords()
        {
            string result = string.Empty;
            string word = string.Empty;
            List<string> results = null;

            Spelling spelling = new Spelling();

            word = "excelent";
            result = spelling.TopCorrectionWord(word);
            Assert.AreEqual("excellent", result);

            word = "proper";
            results = spelling.CorrectionList(word);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("proper", results[0]);

            word = "neccessary";
            results = spelling.CorrectionList(word);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("necessary", results[0]);

            return;
        }
    }
}
