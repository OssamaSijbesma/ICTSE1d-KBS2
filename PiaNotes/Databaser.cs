using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace PiaNotes
{
    class Databaser
    {
        const string ConnectionString = "SERVER = localhost; DATABASE = pianotes; Uid = root; Pwd = root";

        public void Connect()
        {
            using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
            {
                sqlconn.Open();
                Console.WriteLine(sqlconn.State.ToString());
                var state = sqlconn.State.ToString();
            }
        }
    }
}