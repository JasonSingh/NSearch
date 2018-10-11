using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NameSearch
{
    public class Spelling
    {
        private static bool IsSpellerInitialized { get; set; } = false;
        private static Dictionary<String, int> SpellDictionary = new Dictionary<String, int>();
        private static Regex SpellRegex = new Regex("[a-z]+'", RegexOptions.Compiled);

        // Should not be used outside of development. For testing purposes only.
        public static string FindNonAlphas()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string slash = string.Empty;

            if (!path.EndsWith(@"\"))
                slash = @"\";

            // Substitute any file name here for testing.
            string fileContent = File.ReadAllText(path + slash + @"AppData\british-english");
            List<string> wordList = fileContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            string result = string.Empty;
            int wordCount = 0;
            int charCount = 0;

            foreach(string str in wordList)
            {
                wordCount++;
                foreach (char ch in str)
                {
                    if ((ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && ch != '\'')
                       result += ch;
                    charCount++;
                }
            }

            return result;
        }

        static Spelling()
        {
            InitializeDictionary();
        }

        private static void InitializeDictionary()
        {
            if (IsSpellerInitialized)
                return;

            string path = AppDomain.CurrentDomain.BaseDirectory;
            // string path2 = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir)); ;
            string slash = string.Empty;

            if (!path.EndsWith(@"\"))
                slash = @"\";

            string fileContent = File.ReadAllText(path + slash + @"AppData\british-english");
            List<string> wordList = fileContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var word in wordList)
            {
                string trimmedWord = word.Trim().ToLower();
                if (SpellDictionary.ContainsKey(trimmedWord))
                    SpellDictionary[trimmedWord]++;
                else
                    SpellDictionary.Add(trimmedWord, 1);
            }

            IsSpellerInitialized = true;
        }

        public string TopCorrectionWord(string word)
        {
            return CorrectionList(word)[0];
            //  return (candidates.Count > 0) ? candidates.OrderByDescending(x => x.Value).First().Key : word
        }

        public static bool IsWordInDictionary(string input)
        {
            bool result = false;

            if (SpellDictionary.ContainsKey(input))
                result = true;

            return result;
        }

        public List<string> CorrectionList(string word)
        {
            if (string.IsNullOrEmpty(word))
                return new List<string>(word.Split(' '));

            word = word.ToLower();

            // known()
            if (SpellDictionary.ContainsKey(word))
                return new List<string>(word.Split(' '));

            List<String> list = Edits(word);
            Dictionary<string, int> candidates = new Dictionary<string, int>();

            foreach (string wordVariation in list)
            {
                if (SpellDictionary.ContainsKey(wordVariation) && !candidates.ContainsKey(wordVariation))
                    candidates.Add(wordVariation, SpellDictionary[wordVariation]);
            }

            if (candidates.Count > 0)
                return candidates.OrderByDescending(x => x.Value).Select(kvp => kvp.Key).ToList();

            // known_edits2()
            foreach (string item in list)
            {
                foreach (string wordVariation in Edits(item))
                {
                    if (SpellDictionary.ContainsKey(wordVariation) && !candidates.ContainsKey(wordVariation))
                        candidates.Add(wordVariation, SpellDictionary[wordVariation]);
                }
            }

            return (candidates.Count > 0) ? candidates.OrderByDescending(x => x.Value).Select(kvp => kvp.Key).ToList() : new List<string>(word.Split(' '));
        }


        private List<string> Edits(string word)
        {
            var splits = new List<Tuple<string, string>>();
            var transposes = new List<string>();
            var deletes = new List<string>();
            var replaces = new List<string>();
            var inserts = new List<string>();

            // Splits
            for (int i = 0; i < word.Length; i++)
            {
                var tuple = new Tuple<string, string>(word.Substring(0, i), word.Substring(i));
                splits.Add(tuple);
            }

            // Deletes
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (!string.IsNullOrEmpty(b))
                {
                    deletes.Add(a + b.Substring(1));
                }
            }

            // Transposes
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (b.Length > 1)
                {
                    transposes.Add(a + b[1] + b[0] + b.Substring(2));
                }
            }

            // Replaces
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (!string.IsNullOrEmpty(b))
                {
                    for (char c = 'a'; c <= 'z'; c++)
                    {
                        replaces.Add(a + c + b.Substring(1));
                    }
                }
            }

            // Inserts
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                for (char c = 'a'; c <= 'z'; c++)
                {
                    inserts.Add(a + c + b);
                }
            }

            return deletes.Union(transposes).Union(replaces).Union(inserts).ToList();
        }
    }
}