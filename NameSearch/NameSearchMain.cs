using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STFO.Logic.Custom
{
    /// <summary>
    /// This class should be used as the gateway to Name Search. If features are not implemented in this class, then we should have those
    /// here as oppossed to reaching lower layers in the Search.
    /// </summary>
    public class NameSearchMain
    {
        // Initial Normalize Search
        public NormalizeName normalizeName { get; set; }

        // Spell Check Search
        public Spelling spelling { get; set; } = new Spelling();

        // Sounds Like
        public SoundExHash soundExHash { get; set; } = new SoundExHash();

        private NameSearchMain()
        { }

        public NameSearchMain(string initialString)
        {
            normalizeName = new NormalizeName(initialString);
        }

        public List<string> GetCleansedCompanyName(string input)
        {
            normalizeName.Initialize(input);

            List<string> results = normalizeName.ProcessName();

            return results;
        }

        public List<string> GetCleansedCompanyName()
        {
            List<string> results = normalizeName.ProcessName();

            return results;
        }

    }
}
