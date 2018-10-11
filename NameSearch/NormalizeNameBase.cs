using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSearch
{
    class PluralReplacement
    {
        public PluralReplacement(string replaceThis, string replaceWith)
        {
            this.ReplaceThis = replaceThis;
            this.ReplaceWith = replaceWith;
        }
        public string ReplaceThis { get; set; }
        public string ReplaceWith { get; set; }
    }


    public class NormalizeNameBase
    {
        private static Dictionary<string, List<string>> NoiseWords = new Dictionary<string, List<string>>();
        private static List<PluralReplacement> pluralReplacements = new List<PluralReplacement>();

        static NormalizeNameBase()
        {
            List<string> preList = new List<string>();
            preList.Add("THE");
            NoiseWords.Add("CO", null);
            NoiseWords.Add("COMPANY", preList);
            NoiseWords.Add("INC", null);
            NoiseWords.Add("LLC", null);
            NoiseWords.Add("PARTNERSHIP", null);

            NoiseWords.Add("CORP", null);
            NoiseWords.Add("CORPORATION", null);
            NoiseWords.Add("INCORPORATED", null);

            NoiseWords.Add("MM", null);
            NoiseWords.Add("COM", null);

            pluralReplacements.Add(new PluralReplacement("ies", "y"));
            pluralReplacements.Add(new PluralReplacement("s", ""));
        }

        // The name is converted to upper case.
        public static string ToUpperCase(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            return input.ToUpper();
        }

        // Ampersands(&) are converted to "and".
        public static string ConvertAmpersands(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            return input.Replace("&", "AND");
        }

        // Characters that are not 0-9 or A-Z, including punctuation, are replaced by a space.
        public static string RemoveNonAlphaNums(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (char ch in input)
            {
                if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == ' ')
                    stringBuilder.Append(ch);
                else
                    stringBuilder.Append(' ');
            }

            return stringBuilder.ToString();
        }

        // Spaces at the beginning or end of the name are removed.
        public static string RemoveBeginAndEndSpaces(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            return input.Trim();
        }

        // If there are two(or more) spaces in a row, extra spaces are removed and only one will remain.
        public static string RemoveNeighboringSpaces(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            string old = string.Empty;

            do
            {
                old = input;
                input = input.Replace("  ", " ");
            }
            while (old.CompareTo(input) != 0);

            return old;
        }

        public static string RemoveTabs(string input)
        {
            return input.Replace("\t", " ");
        }

        // "A", "an" and "the" at the beginning or end of a name are removed.
        // "A" is not removed if it is followed by "and" and a space (such as "A and M, Inc." or "A & M, Inc.")
        public static string RemoveAAndAnAndTheFromBeginning(string input)
        {
            if (input == string.Empty || input == null)
                return input;


            string[] words = input.Split(' ');

            bool RemoveFirst = false;

            if ((words.GetLength(0) > 0) && string.Compare(words[0], "AN") == 0 || string.Compare(words[0], "THE") == 0 || string.Compare(words[0], "A") == 0)
                RemoveFirst = true;

            if ((words.GetLength(0) > 2) && (string.Compare(words[0], "A") == 0))
                if (string.Compare(words[1], "AND") == 0 || string.Compare(words[1], "&") == 0)
                    RemoveFirst = false;

            if (RemoveFirst)
                words[0] = string.Empty;

            if (words.GetLength(0) > 1)
                if (string.Compare(words[words.GetLength(0) - 1], "A") == 0 || string.Compare(words[words.GetLength(0) - 1], "AN") == 0 || string.Compare(words[words.GetLength(0) - 1], "THE") == 0)
                    words[words.GetLength(0) - 1] = string.Empty;

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < words.GetLength(0); i++)
                if (words[i] != string.Empty)
                    result.Append(words[i] + ' ');

            // TODO. Consider removing multiple instances of these tokens in the front and back.
            result = new StringBuilder(RemoveBeginAndEndSpaces(result.ToString()));

            return result.ToString();
        }

        private static string ReverseString(string input)
        {
            if (input == string.Empty || input == null)
                return input;

            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);

            return new string(charArray);
        }

        // Implemented in RemoveAAndAnAndTheFromBeginning()
        public static string CheckANotRemovedIfFollowedByAndOrSpace(string input)
        {
            throw new NotImplementedException();
        }


        // "Ending noise words" are ignored(e.g., "company", "Corp", "Inc", etc.)
        public static string RemoveCompanyTypeWords(string input, out bool foundCompany)
        {
            foundCompany = false;

            if (input == string.Empty || input == null)
                return input;

            string[] words = input.Split(' ');

            StringBuilder result = new StringBuilder();

            if (NoiseWords.ContainsKey(words[words.GetLength(0) - 1]))
            {
                foundCompany = true;
                words[words.GetLength(0) - 1] = string.Empty;
            }

            for (int i = 0; i < words.GetLength(0); i++)
                if (words[i] != string.Empty)
                    result.Append(words[i] + ' ');

            result = new StringBuilder(RemoveBeginAndEndSpaces(result.ToString()));

            return result.ToString();
        }

        public static string RemoveAllSpaces(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char ch in input)
                if (ch != ' ')
                    result.Append(ch);

            return result.ToString();
        }

        public static bool IsCompany(string input)
        {
            throw new NotImplementedException();
        }

        public static List<string> GenerateCompanyNames(string input)
        {
            List<string> companyCombinations = new List<string>();

            foreach (KeyValuePair<string, List<string>> entry in NoiseWords)
            {
                bool preFix = false;

                string companyName = string.Empty;
                if (entry.Value != null)
                {
                    preFix = true;
                    foreach (string str in entry.Value)
                        companyName += str + " ";
                }
                companyName += input + " " + entry.Key;

                companyCombinations.Add(companyName);

                if (preFix)
                {
                    companyName = input + " " + entry.Key;
                    companyCombinations.Add(companyName);
                }
            }

            return companyCombinations;
        }

        public static string ConcatAbbreviations(string input)
        {
            StringBuilder result = new StringBuilder();

            List<string> words = input.Split(' ').ToList();
            string abbreviateBuffer = string.Empty;

            foreach (string word in words)
            {
                if (word.Length == 1)
                    abbreviateBuffer += word;
                else
                {
                    if (abbreviateBuffer.Length > 0)
                    {
                        result.Append(abbreviateBuffer + ' ');
                        abbreviateBuffer = string.Empty;
                    }
                    result.Append(word + ' ');
                }
            }
            if (abbreviateBuffer.Length > 0)
                result.Append(abbreviateBuffer);

            return result.ToString().Trim();
        }

        public static string ReplaceDigits(string input)
        {
            StringBuilder result = new StringBuilder();


            return result.ToString().Trim();
        }

        public static bool GetNonPlural(ref string input)
        {
            if (input == string.Empty || input == string.Empty || input == "")
                return false;

            bool found = false;
            StringBuilder tmpStr = new StringBuilder(input);

            foreach (PluralReplacement pluralReplacement in pluralReplacements)
            {
                if (pluralReplacement.ReplaceThis.Length >= tmpStr.Length)
                    continue;

                if (tmpStr.ToString().EndsWith(pluralReplacement.ReplaceThis))
                {
                    tmpStr.Remove(tmpStr.Length - pluralReplacement.ReplaceThis.Length, pluralReplacement.ReplaceThis.Length);
                    tmpStr.Append(pluralReplacement.ReplaceWith);

                    if (Spelling.IsWordInDictionary(tmpStr.ToString()))
                    {
                        input = tmpStr.ToString();
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

    }
}
