using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

// using STFO.Logic.Data;

namespace STFO.Logic.Custom
{
    public class SoundExDb
    {

        public SoundExDb()
        { }

        private static IDbCommand GetSoundEx(ConnectionManager oConnectionManager, string expression)
        {
            string sql = @"select SoundEx('" + expression + "')";
            IDbCommand oCommand = oConnectionManager.GetCommandObject(sql);
            oCommand.CommandType = CommandType.Text;

            return oCommand;
        }

        public static string GetSoundEx(string input)
        {
            string result = string.Empty;

            SqlCommand cmd = (SqlCommand) GetSoundEx(ConnectionManager.Current, input);

            SqlDataReader dr = (SqlDataReader)cmd.ExecuteReader();

            while (dr.Read())
                result = dr.GetString(0);


            return result;
        }

        public static List<string> GetSoundEx(List<string> input)
        {
            List<string> result = new List<string>();

            foreach (string str in input)
                result.Add(GetSoundEx(str));

            return result;
        }
    }
}
