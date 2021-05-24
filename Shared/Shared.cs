using System;
using MySql.Data.MySqlClient;
using ProjectD.Database;

namespace Project_D.Shared
{
    public class Shared
	{
        /// <summary>
        /// Checks if a session code exists in the database
        /// </summary>
        /// <param name="hm">home model with session code</param>
        /// <returns></returns>
        public static bool DoesSessionExist(string sessioncode)
        {
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT DISTINCT sessionCode FROM database.sessions where sessionCode = @SessionCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", sessioncode);

                MySqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }


        /// <summary>
        /// return int with amount of people currently in the session
        /// </summary>
        /// <param name="sessionCode">the current session code</param>
        /// <returns></returns>
        public static int GetPeopleInSession(string sessionCode)
        {
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT DISTINCT COUNT(*) FROM database.usersinsession WHERE sessioncode = @SessionCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", sessionCode);

                int rowsreturned = Convert.ToInt32(command.ExecuteScalar());

                return rowsreturned;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public static bool AddUserCodeToSession(string userCode, string sessionCode)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());
            try
            {
                Connection.Open();
                string stringToInsert = @"INSERT INTO usersinsession (usercode, sessioncode) VALUES (@userCode, @sessionCode)";

                using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                {
                    // Add parameters here
                    command.Parameters.AddWithValue("@SessionCode", sessionCode);
                    command.Parameters.AddWithValue("@UserCode", userCode);

                    command.Prepare();
                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public static bool IsUserCodeInSession(string userCode, string sessionCode)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT COUNT(*) FROM database.usersinsession WHERE sessioncode = @SessionCode AND usercode = @userCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", sessionCode);
                command.Parameters.AddWithValue("@UserCode", userCode);

                int rowsreturned = Convert.ToInt32(command.ExecuteScalar());

                if (rowsreturned > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }
    }
}
