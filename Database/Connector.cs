using System;
using MySql.Data.MySqlClient;

namespace ProjectD.Database
{
    public class Connector
    {
        public MySqlConnection Connection;
        public string dbstring { get; }

        public static string getString()
        {
            return "server=project-d.mysql.database.azure.com; user=projectAdmin@project-d;pwd=Admin123;database=database";
        }
    }
}
