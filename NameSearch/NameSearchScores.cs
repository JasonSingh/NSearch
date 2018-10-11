using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STFO.Logic.Custom
{
    public class CompanyRecordScore
    {
        public CompanyRecord companyRecord { get; set; } = null;
        public int Score { get; set; } = 0;
        public CompanyScoreType companyScoreType { get; set; } = CompanyScoreType.MatchUndefined;

        public CompanyRecordScore(CompanyRecord companyRecord, int score, CompanyScoreType companyScoreType)
        {
            this.companyRecord = companyRecord;
            this.Score = score;
            this.companyScoreType = companyScoreType;
        }
    }

    public enum CompanyScoreType
    {
        MatchUndefined = 0,
        ExactMatchNoNormalization,
        ExactMatchWithNormalization,
        ExactMatchWithNormalizationOutOfOrder,
        MatchWithoutPlurals,
        MatchSpellings,
        MatchSoundsLike
    }

    public class NameSearchScores
    {
        private NameSearchPercentage nameSearchPercentage = new NameSearchPercentage();

        private NameSearchRatios nameSearchRatios {get; set;} = null;
        private const int PercentageMultiplier = 100;

        private NameSearchScores()
        {
        }

        private NameSearchPercentage CalculatePercentages(NameSearchRatios nameSearchRatios)
        {
            NameSearchPercentage nameSearchPercentage = new NameSearchPercentage();

            int totalPoints = nameSearchRatios.ExactMatchNoNormalization +
                        nameSearchRatios.ExactMatchWithNormalization +
                        nameSearchRatios.ExactMatchWithNormalizationOutOfOrder +
                        nameSearchRatios.MatchWithoutPlurals +
                        nameSearchRatios.MatchSpellings +
                        nameSearchRatios.MatchSoundsLike;

            nameSearchPercentage.ExactMatchNoNormalizationPercentage = (float) (nameSearchRatios.ExactMatchNoNormalization * PercentageMultiplier) / totalPoints;
            nameSearchPercentage.ExactMatchWithNormalizationPercentage = (float) (nameSearchRatios.ExactMatchWithNormalization * PercentageMultiplier) / totalPoints;
            nameSearchPercentage.ExactMatchWithNormalizationOutOfOrderPercentage = (float) (nameSearchRatios.ExactMatchWithNormalizationOutOfOrder * PercentageMultiplier) / totalPoints;
            nameSearchPercentage.MatchWithoutPluralsPercentage = (float) (nameSearchRatios.MatchWithoutPlurals * PercentageMultiplier) / totalPoints;
            nameSearchPercentage.MatchSpellingsPercentage = (float) (nameSearchRatios.MatchSpellings * PercentageMultiplier) / totalPoints;
            nameSearchPercentage.MatchSoundsLikePercentage = (float) (nameSearchRatios.MatchSoundsLike * PercentageMultiplier) / totalPoints;

            return nameSearchPercentage;
        }

        public NameSearchScores(NameSearchRatios nameSearchRatios)
        {
            this.nameSearchRatios = nameSearchRatios;

            this.nameSearchPercentage = CalculatePercentages(nameSearchRatios);
        }

        // This one will get 100% of the allocated percentage.
        private Tuple<bool, CompanyRecord> GetCompanyNameExactMatchNoNormalization(string companyName)
        {
            bool result = false;
            CompanyRecord companyRecord = null;

            // Look for exact match. No cleanse.
            if (CompanyRecords.CompanyDictionary.ContainsKey(companyName))
            {
                companyRecord = CompanyRecords.CompanyDictionary[companyName];
                result = true;
            }

            return new Tuple<bool, CompanyRecord>(result, companyRecord);
        }
        
        // All of these will get 100% of the allocated percentage.
        private Dictionary<string, CompanyRecordScore> GetCompanyNameExactMatchWithNormalization(string cleansedCompanyName, CompanyRecord companyRecord)
        {
            Dictionary<string, CompanyRecordScore> result = new Dictionary<string, CompanyRecordScore>();

            if (cleansedCompanyName == companyRecord.CleansedCompanyName)
                result.Add(companyRecord.CompanyName, new CompanyRecordScore(companyRecord, (int) (nameSearchPercentage.ExactMatchWithNormalizationPercentage + 0.5), CompanyScoreType.ExactMatchWithNormalization));

            return result;
        }

        private Dictionary<string, CompanyRecordScore> GetCompanyNameExactMatchWithNormalizationOutOfOrder(string[] companyWords, CompanyRecord companyRecord)
        {
            Dictionary<string, CompanyRecordScore> result = new Dictionary<string, CompanyRecordScore>();
            int matchCount = 0;
            int Score = 0;

            for (int i = 0; i < companyWords.GetLength(0); i++)
            {
                if (companyRecord.CleansedCompanyNameWords.Contains(companyWords[i]))
                    matchCount++;
            }

            int cleansedWordCount = companyRecord.CleansedCompanyNameWords.GetLength(0);
            int companyWordCount = companyWords.GetLength(0);

            float matchPercentage = GetMatchPercentage(matchCount, cleansedWordCount, companyWordCount);

            matchPercentage = (nameSearchPercentage.ExactMatchWithNormalizationOutOfOrderPercentage * matchPercentage) / PercentageMultiplier;

            if (matchPercentage >= nameSearchPercentage.ExactMatchWithNormalizationOutOfOrderPercentage)
            {
                Score = (int) (matchPercentage + 0.5);
                result.Add(companyRecord.CompanyName, new CompanyRecordScore(companyRecord, Score, CompanyScoreType.ExactMatchWithNormalizationOutOfOrder));
            }

            return result;
        }

        private static float GetMatchPercentage(int matchCount, int cleansedWordCount, int companyWordCount)
        {
            float matchPercentage = 0;

            int wordDiffCount = Math.Abs(cleansedWordCount - companyWordCount);

            int Divisor = (cleansedWordCount > companyWordCount) ? cleansedWordCount : companyWordCount;

            matchPercentage =  (matchCount * PercentageMultiplier) / Divisor;

            return matchPercentage;
        }

        private Dictionary<string, CompanyRecordScore> GetCompanyNameSoundsLike(string[] companyWords, CompanyRecord companyListRecord)
        {
            // Let's not consider in order.
            Dictionary<string, CompanyRecordScore> result = new Dictionary<string, CompanyRecordScore>();
            int matchCount = 0;
            int Score = 0;

            List<TokenSound> tokenSoundList = SoundExHash.GetTokenSounds(companyWords.ToList());

            foreach (TokenSound tokenSound in tokenSoundList)
            {
                foreach (TokenSound companyListToken in companyListRecord.tokenSoundList)
                {
                    if (companyListToken.Sound.Contains(tokenSound.Sound))
                        matchCount++;
                }
            }

            int cleansedWordCount = companyListRecord.tokenSoundList.Count;
            int companyWordCount = tokenSoundList.Count;

            float matchPercentage = GetMatchPercentage(matchCount, cleansedWordCount, companyWordCount);

            matchPercentage = (nameSearchPercentage.MatchSoundsLikePercentage * matchPercentage) / PercentageMultiplier;

            if (matchPercentage >= nameSearchPercentage.ExactMatchWithNormalizationOutOfOrderPercentage)
            {
                Score = (int) (matchPercentage + 0.5);
                result.Add(companyListRecord.CompanyName, new CompanyRecordScore(companyListRecord, Score, CompanyScoreType.MatchSoundsLike));
            }

            return result;
        }

        private void AddScores(Dictionary<string, CompanyRecordScore> source, ref Dictionary<string, CompanyRecordScore> destination)
        {
            // Given a score, keep the higher score in the result, since a higher result can also yield lower scores.

            Dictionary<string, CompanyRecordScore> replaceTheseWith = new Dictionary<string, CompanyRecordScore>();

            foreach (var record in source)
            {
                string companyName = record.Key;
                CompanyRecordScore companyRecordScore = record.Value;

                if (destination.ContainsKey(companyName))
                {
                    var destinationRecord = destination[companyName];
                    if (destinationRecord.Score < companyRecordScore.Score)
                        replaceTheseWith.Add(companyName, companyRecordScore);
                }
                else
                    destination.Add(companyName, companyRecordScore);
            }

            foreach (var record in replaceTheseWith)
            {
                string companyName = record.Key;
                CompanyRecordScore companyRecordScore = record.Value;

                source[companyName] = companyRecordScore;
            }
        }

        public Dictionary<string, CompanyRecordScore> ProcessCompanyRecordScores(string companyName)
        {
            Dictionary<string, CompanyRecordScore> result = new Dictionary<string, CompanyRecordScore>();

            var ExactMatch = GetCompanyNameExactMatchNoNormalization(companyName);
            if (ExactMatch.Item1)
                result.Add(companyName, new CompanyRecordScore(ExactMatch.Item2, (int) (nameSearchPercentage.ExactMatchNoNormalizationPercentage + 0.5), CompanyScoreType.ExactMatchNoNormalization));

            string cleansedCompanyName = NormalizeName.GetCleansedCompanyName(companyName);
            string[] cleansedCompanyWords = cleansedCompanyName.Split(' ');

            Dictionary<string, CompanyRecordScore> getScoresList = null;

            foreach (var entry in CompanyRecords.CompanyDictionary)
            {
                string companyNameEntry = entry.Key;
                CompanyRecord companyRecord = entry.Value;

                getScoresList = GetCompanyNameExactMatchWithNormalization(cleansedCompanyName, companyRecord);
                if (getScoresList.Count > 0)
                    AddScores(getScoresList, ref result);

                // Get word matches, out of order or less or more words...
                getScoresList = GetCompanyNameExactMatchWithNormalizationOutOfOrder(cleansedCompanyWords, companyRecord);
                if (getScoresList.Count > 0)
                    AddScores(getScoresList, ref result);

                // Consider sounds like matches
                getScoresList = GetCompanyNameSoundsLike(cleansedCompanyWords, companyRecord);
                if (getScoresList.Count > 0)
                    AddScores(getScoresList, ref result);
            }

            return result;
        }

        public void GetCompanyMatchWithoutPlurals(string CompanyName)
        {
            throw new NotImplementedException();
        }

        public void GetCompanyMatchSpellings(string CompanyName)
        {
            throw new NotImplementedException();
        }

        public void GetSoundsLikeCompanyNameSearchMatches()
        {
            throw new NotImplementedException();
        }
    }
}
