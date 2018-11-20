using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PiaNotes
{
    class Databaser
    {
        const string ConnectionString = "SERVER = DESKTOP-PA2O8G9; DATABASE = pianotes; USER ID = root; PASSWORD = ";

        private void Connect()
        {
            using (SqlConnection) { }
        }
    }
}