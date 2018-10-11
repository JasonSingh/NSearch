using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSearch
{
    public class NormalizeName : NormalizeNameBase
    {
        private string InitialString { get; set; }
        public string ModifiedString { get; set; }

        public bool FoundCompanyName { get; set; } = false;
        private NormalizeName() { }

        public NormalizeName(string initialString)
        {
            this.InitialString = initialString;
        }

        public List<string> ProcessName(string initialString)
        {
            this.InitialString = initialString;

            return ProcessName();
        }
        
        public List<string> ProcessName()
        {
            List<string> companyNames = new List<string>();

            ModifiedString = ToUpperCase(InitialString);
            ModifiedString = RemoveBeginAndEndSpaces(ModifiedString);
            ModifiedString = ConvertAmpersands(ModifiedString);
            ModifiedString = RemoveNonAlphaNums(ModifiedString);
            ModifiedString = RemoveNeighboringSpaces(ModifiedString);
            ModifiedString = RemoveTabs(ModifiedString);
            ModifiedString = RemoveAAndAnAndTheFromBeginning(ModifiedString);

            ModifiedString = RemoveCompanyTypeWords(ModifiedString, out bool foundCompany);
            if (foundCompany)
                FoundCompanyName = true;

            if (FoundCompanyName)
                companyNames = GenerateCompanyNames(ModifiedString);

            return companyNames;
        }

    }
}
