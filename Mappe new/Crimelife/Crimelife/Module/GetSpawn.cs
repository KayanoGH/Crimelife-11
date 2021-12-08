using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MySql.Data.MySqlClient;


namespace Crimelife
{
    class GetSpawn : Script
    {


        public static string Spawn(string name)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(Configuration.connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText =
                    "SELECT * FROM accounts WHERE Username=@name";
                command.Parameters.AddWithValue("@name", name);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return Convert.ToString(reader.GetInt32("Spawn"));
                    }
                }

                connection.Close();
            }
            catch (Exception exception)
            {
                NAPI.Util.ConsoleOutput($"GetAktenDescriptionById: {exception.StackTrace}");
                NAPI.Util.ConsoleOutput($"GetAktenDescriptionById: {exception.Message}");
            }

            return "";
        }
    }
}
