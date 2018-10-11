using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using STFO.Logic.Data;

namespace STFO.Logic.Custom
{
    public class TokenSound
    {
        public string Word { get; set; } = null;
        public string Sound { get; set; } = null;
    }

    public class CompanyRecord
    {
        public string CompanyName { get; set; } = null;
        public int CompanyId { get; set; } = 0;
        public string CleansedCompanyName { get; set; } = null;
        public string[] CleansedCompanyNameWords { get; set; } = null;
        public List<TokenSound> tokenSoundList = null;
    }

    public static class CompanyRecords
    {
        public static Dictionary<string, CompanyRecord> CompanyDictionary = null;

        static CompanyRecords()
        {
            InitializeCompanyRecords();
        }

        // Helper class to Add or Update CompanyRecords.
        public static bool InsertCompanyRecord(string companyName, CompanyRecord companyRecord)
        {
            bool result = false;
            
            if (companyName == null || companyName == string.Empty || companyName == "")
                throw new ApplicationException("Invalid string for CompanyName {companyName}");

            if (companyRecord == null)
                throw new ApplicationException("Must assign CompanyRecord type value before use");

            if (companyName.CompareTo(companyRecord.CompanyName) != 0)
                companyRecord.CompanyName = companyName; 

            object lockit = new object();

            lock(lockit)
            {
                companyRecord.CleansedCompanyName = NormalizeName.GetCleansedCompanyName(companyRecord.CompanyName);
                companyRecord.CleansedCompanyNameWords = companyRecord.CleansedCompanyName.Split(' ');
                List<TokenSound> tokenSoundList = SoundExHash.GetTokenSounds(new List<string>(companyRecord.CleansedCompanyNameWords));

                if (!CompanyDictionary.ContainsKey(companyName))
                    CompanyDictionary.Add(companyName, companyRecord);
                else
                    CompanyDictionary[companyName] = companyRecord;

                result = true;
            }

            return result;
        }

        public static void InitializeCompanyRecords()
        {
            CompanyDictionary = new Dictionary<string, CompanyRecord>();
            List<string> ExceptionList = new List<string>();

            string queryString = @"select Id, CompanyName from NameSearch";
            
            using (SqlConnection connection = new SqlConnection(ConnectionManager.Current.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        CompanyRecord companyRecord = new CompanyRecord();

                        companyRecord.CompanyId = reader.GetInt32(0);
                        companyRecord.CompanyName = reader.GetString(1);
                        
                        if (companyRecord.CompanyName != "" && companyRecord.CompanyName != string.Empty && companyRecord.CompanyName != null)
                        {
                            if (!CompanyDictionary.ContainsKey(companyRecord.CompanyName))
                            {
                                companyRecord.CleansedCompanyName = NormalizeName.GetCleansedCompanyName(companyRecord.CompanyName);
                                companyRecord.CleansedCompanyNameWords = companyRecord.CleansedCompanyName.Split(' ');

                                companyRecord.tokenSoundList = SoundExHash.GetTokenSounds(new List<string>(companyRecord.CleansedCompanyNameWords));

                                CompanyDictionary.Add(companyRecord.CompanyName, companyRecord);
                            }
                            else
                                ExceptionList.Add(companyRecord.CompanyName);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                    reader = null;
                }
                command.Dispose();
                command = null;
            }

            return;
        }
    }
}
