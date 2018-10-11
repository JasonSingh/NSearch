using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STFO.Logic.Custom
{
    public class NameSearchRatios
    {

        public NameSearchRatios
        (
            int exactMatchNoNormalization,
            int exactMatchWithNormalization,
            int exactMatchWithNormalizationOutOfOrder,
            int matchWithoutPlurals,
            int matchSpellings,
            int matchSoundsLike
    )
        {
            this.ExactMatchNoNormalization = exactMatchNoNormalization;
            this.ExactMatchWithNormalization = exactMatchWithNormalization;
            this.ExactMatchWithNormalizationOutOfOrder = exactMatchWithNormalizationOutOfOrder;
            this.MatchWithoutPlurals = matchWithoutPlurals;
            this.MatchSpellings = matchSpellings;
            this.MatchSoundsLike = matchSoundsLike;
        }

        public int ExactMatchNoNormalization { get; set; } = 1;
        public int ExactMatchWithNormalization { get; set; } = 0;
        public int ExactMatchWithNormalizationOutOfOrder { get; set; } = 0;
        public int MatchWithoutPlurals { get; set; } = 0;
        public int MatchSpellings { get; set; } = 0;
        public int MatchSoundsLike { get; set; } = 0;
    }

    public class NameSearchPercentage
    {
        public float ExactMatchNoNormalizationPercentage { get; set; } = 0;
        public float ExactMatchWithNormalizationPercentage { get; set; } = 0;
        public float ExactMatchWithNormalizationOutOfOrderPercentage { get; set; } = 0;
        public float MatchWithoutPluralsPercentage { get; set; } = 0;
        public float MatchSpellingsPercentage { get; set; } = 0;
        public float MatchSoundsLikePercentage { get; set; } = 0;
    }
}
