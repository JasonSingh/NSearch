using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using NameSearch;

namespace NameSearchUnitTests
{
    [TestClass]
    public class NameSearchTests
    {
        [TestMethod]
        public void TestUpperCase()
        {
            // The name is converted to upper case
            
            string result = NormalizeNameBase.ToUpperCase("abc");
            Assert.AreEqual<string>("ABC", result);
            result = NormalizeNameBase.ToUpperCase("AbCdEfGH");
            Assert.AreEqual<string>("ABCDEFGH", result);

            return;
        }

        [TestMethod]
        public void TestConvertAmpersands()
        {
            // Ampersands(&) are converted to "and".
            string result = NormalizeNameBase.ConvertAmpersands("This & That");
            Assert.AreEqual<string>("This AND That", result);
            result = NormalizeNameBase.ConvertAmpersands("& This & That &");
            Assert.AreEqual<string>("AND This AND That AND", result);
            result = NormalizeNameBase.ConvertAmpersands("&&&&&");
            Assert.AreEqual<string>("ANDANDANDANDAND", result);

            return;
        }

        [TestMethod]
        public void TestRemoveNonAlphaNums()
        {
            // Characters that are not 0 - 9 or A-Z, including punctuation, are replaced by a space
            string result = NormalizeNameBase.RemoveNonAlphaNums("abcde");
            Assert.AreEqual("abcde", result);
            result = NormalizeNameBase.RemoveNonAlphaNums("a,b_c*`^d()\\");
            Assert.AreEqual("a b c   d   ", result);

            return;
        }

        [TestMethod]
        public void TestRemoveBeginAndEndSpaces()
        {
            // Spaces at the beginning or end of the name are removed
            string result = NormalizeNameBase.RemoveBeginAndEndSpaces("abcde");
            Assert.AreEqual("abcde", result);
            result = NormalizeNameBase.RemoveBeginAndEndSpaces("    abcde ");
            Assert.AreEqual("abcde", result);
            result = NormalizeNameBase.RemoveBeginAndEndSpaces("  ab c  de   \t");
            Assert.AreEqual("ab c  de", result);

            return;
        }

        [TestMethod]
        public void TestRemoveNeighboringSpaces()
        {
            // If there are two (or more) spaces in a row, extra spaces are removed and only one will remain
            string result = NormalizeNameBase.RemoveNeighboringSpaces("abcde");
            Assert.AreEqual("abcde", result);
            result = NormalizeNameBase.RemoveNeighboringSpaces("ab  cde");
            Assert.AreEqual("ab cde", result);
            result = NormalizeNameBase.RemoveNeighboringSpaces("a    b   c  d          e");
            Assert.AreEqual("a b c d e", result);

            return;
        }

        private static string PreProcessTestString(string input)
        {
            input = NormalizeNameBase.RemoveBeginAndEndSpaces(input);
            input = NormalizeNameBase.ToUpperCase(input);
            input = NormalizeNameBase.RemoveNeighboringSpaces(input);

            return input;
        }

        [TestMethod]
        public void TestRemoveAAndAnAndTheFromBeginning()
        {
            // "A", "an" and "the" at the beginning or end of a name are removed

            string input = "the abcde";
            input = PreProcessTestString(input);
            string result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "an abcde";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "a abcde";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "abcde the";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "abcde an";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "abcde a";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            input = "the abcde a";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("ABCDE", result);

            // TODO. Consider removing multiple instances of these tokens in the front and back.
            // e.g. result = Normalize.RemoveAAndAnAndTheFromBeginning("the the abcde a");
            // Assert.AreEqual(result, "abcde");

            return;
        }

        [TestMethod]
        public void CheckANotRemovedIfFollowedByAndOrSpace()
        {
            // A" is not removed if it is followed by "and" and a space (such as "A and M, Inc." or "A & M, Inc.")

            // Implemented by previous...

            string input = "A and M";
            input = PreProcessTestString(input);
            string result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("A AND M", result);

            input = "A book";
            input = PreProcessTestString(input);
            result = NormalizeNameBase.RemoveAAndAnAndTheFromBeginning(input);
            Assert.AreEqual<string>("BOOK", result);
        }

        [TestMethod]
        public void TestRemoveNoiseWords()
        {
            string result = NormalizeName.RemoveCompanyTypeWords("ABC COM", out bool foundCompany);
            Assert.AreEqual("ABC", result);

            result = NormalizeName.RemoveCompanyTypeWords("ABC MM", out foundCompany);
            Assert.AreEqual("ABC", result);

            return;
        }

        [TestMethod]
        public void TestRemoveAllSpaces()
        {
            string result = NormalizeName.RemoveAllSpaces("LOTS    OF SPACE IS     NOT GOOD!");
            Assert.AreEqual("LOTSOFSPACEISNOTGOOD!", result);

            return;
        }

        [TestMethod]
        public void TestProcessName()
        {
            /*
             For example, if you searched for "The ABC Company", your results would include:
            ABC Co
            ABC Company
            ABC Inc
            ABC LLC
            ABC Partnership
            The ABC Company
            */
            NormalizeName normalizeName = new NormalizeName("The ABC Company");
            List<string> results = normalizeName.ProcessName();

            Assert.AreEqual(11, results.Count);
            Assert.IsTrue(results.Contains("ABC INC"));
            Assert.IsTrue(results.Contains("ABC LLC"));
            Assert.IsTrue(results.Contains("THE ABC COMPANY"));
            Assert.IsTrue(results.Contains("ABC COMPANY"));

            return;
        }

        [TestMethod]
        public void TestConcatAbbreviations()
        {
            string result = PreProcessTestString("A B C");
            result = NormalizeName.ConcatAbbreviations(result);
            Assert.AreEqual("ABC", result);

            result = PreProcessTestString("Walt A B C Disney");
            result = NormalizeName.ConcatAbbreviations(result);
            Assert.AreEqual("WALT ABC DISNEY", result);

            result = PreProcessTestString("Walt A B C");
            result = NormalizeName.ConcatAbbreviations(result);
            Assert.AreEqual("WALT ABC", result);

            result = PreProcessTestString("A B C Disney");
            result = NormalizeName.ConcatAbbreviations(result);
            Assert.AreEqual("ABC DISNEY", result);

            result = PreProcessTestString("Walt A B C Disney A B C");
            result = NormalizeName.ConcatAbbreviations(result);
            Assert.AreEqual("WALT ABC DISNEY ABC", result);

            return;
        }

        [TestMethod]
        public void TestGetNonPlural()
        {
            string testString = "stories";

            bool result = NormalizeNameBase.GetNonPlural(ref testString);
            Assert.AreEqual<bool>(true, result);
            Assert.AreEqual<string>("story", testString);

            testString = "cats";
            result = NormalizeNameBase.GetNonPlural(ref testString);
            Assert.AreEqual<bool>(true, result);
            Assert.AreEqual<string>("cat", testString);

            testString = "waste";
            result = NormalizeNameBase.GetNonPlural(ref testString);
            Assert.AreEqual<bool>(false, result);
            Assert.AreEqual<string>("waste", testString);
        }

    }
}
