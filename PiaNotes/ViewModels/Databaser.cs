using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using PiaNotes.Models;
using Windows.Storage;

namespace PiaNotes.ViewModels
{
    public class Databaser
    {
        //Set connection string (ROOT USER IS FOR TESTING ONLY, Use system instead)
        //private const string ConnectionString = "SERVER = pianotesmysql.mysql.database.azure.com; PORT=3306; DATABASE = pianotes; Uid = notesAdmin@pianotesmysql; Pwd = !Pianotes223; SslMode = Preferred;";
        private const string ConnectionString = "SERVER = pianotesql.mysql.database.azure.com; PORT=3306; DATABASE = pianotes; Uid = epicadmin@pianotesql; Pwd = PiaNote$; SslMode = Preferred;";
        private const string DataTable = "musicsheet";
        //Function for checking connection status.
        public bool CheckConnection()
        {
            try
            {
                //Create and use the ConnectionString to do something. Then throw the connection away
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                {
                    //Set state to false
                    bool state = false;

                    //Open Connection
                    sqlconn.Open();

                    //If connection opened correctly. And gives state as Open back.
                    if (sqlconn.State.ToString() == "Open")
                    {
                        //Set state true
                        state = true;

                        //Close connection
                        sqlconn.Close();
                        return state;
                    }
                    else
                    {
                        //Close connection
                        sqlconn.Close();
                        return state;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        //Search through MusicSheets in the database with a limit
        public List<MusicSheet> Search(string select, string whereA1, string whereB1, string whereA2, string whereB2, int limit, int offset)
        {
            try
            {
                //Set query standaard.
                var Select = $"SELECT * FROM {DataTable} ";
                var Where = $"";
                var And = $"";
                var OrderBy = $" ORDER BY MusicSheet.UploadDate DESC ";
                var Limit = $"";
                var Offset = $"";

                //If function has specific selected, change it in the query.
                if (select != null) { Select = $"SELECT {select} FROM {DataTable} "; }

                //If both Wheres are specified add a WHERE to the query.
                if (whereA1 != null && whereB1 != null)
                {
                    if (whereA1 == "Id")
                    {
                        Where = $"WHERE UPPER({whereA1}) LIKE UPPER('{whereB1}') ";
                    }
                    else
                    {
                        Where = $"WHERE UPPER({whereA1}) LIKE UPPER('%{whereB1}%') ";
                    }
                }

                if (whereA2 != null && whereB2 != null)
                {
                    if (whereA2 == "Id")
                    {
                        And = $" AND UPPER({whereA2}) LIKE UPPER('{whereB2}') ";
                    }
                    else
                    {
                        And = $" AND UPPER({whereA2}) LIKE UPPER('%{whereB2}%') ";
                    }
                }

                //If a limit is specified, add a LIMIT to the query
                if (limit != 0) { Limit = $"LIMIT {limit} "; }

                //If offset is specified, check if LIMIT is also specified, if yes, add OFFSET to query
                if (offset != 0 && limit != 0) { Offset = $"OFFSET {offset} "; }

                //Build the sql into a string
                string sql = Select + Where + And + OrderBy + Limit + Offset;

                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(sql, sqlconn))
                {
                    //List of Searched sheets
                    List<MusicSheet> result = new List<MusicSheet>();

                    //Open connection to database
                    sqlconn.Open();

                    //Prepare statement for execution
                    cmd.Prepare();

                    //Execute SQL command
                    var go = cmd.ExecuteReader();

                    //New list to remember ids.
                    List<int> list = new List<int>();

                    bool read = true;
                    while (read == true)
                    {
                        //Read results
                        go.Read();
                        //Check if the result is filled and if the id has not yet been added to the list.
                        if (go.HasRows == true && list.Contains(go.GetInt32(0)) == false)
                        {
                            //Get found Id, Title and Path
                            var FoundId = go.GetInt32(0); var FoundTitle = go.GetString(1); var FoundFile = go.GetString(2);

                            //Make a new MusicSheet using found data and add it to the list
                            result.Add(new MusicSheet(FoundId, FoundTitle, FoundFile));
                            //Add id to the list to check next cycle
                            list.Add(go.GetInt32(0));
                        }
                        else read = false;
                    }

                    //Close connection to database and return results
                    sqlconn.Close();
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<MusicSheet> Search(string select, string whereA, string whereB, int limit, int offset)
        {
           return Search(select, whereA, whereB,null,null, limit, offset);
        }

        // Get a file from the database by ID
        public async Task<StorageFile> GetAFileAsync(int id)
        {
            try { 
            //Build sql query
            string sql = $"SELECT FileBytes, FileName FROM {DataTable} Where Id = @ID";

                //Setup connection and sql command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(sql, sqlconn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    //Open connection
                    sqlconn.Open();

                    //Execute query
                    var go = cmd.ExecuteReader();
                    go.Read();

                    //Get values for use
                    var bytes = go.GetValue(0);
                    Byte[] byteArray = (Byte[])bytes;
                    string fileName = go.GetString(1);

                    //Close connection to server
                    sqlconn.Close();

                    //Create and return a storage file created from database
                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(sampleFile, byteArray);
                    return sampleFile;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool Upload(string title, Byte[] fileBytes, string fileName)
        {
            try
            {
                string sql = $"INSERT INTO {DataTable} (Title, FileBytes, FileName) VALUES (@title, @fileBytes, @fileName);";
                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(sql, sqlconn))
                {
                    cmd.Parameters.Add("@title", MySqlDbType.VarChar, title.Length).Value = title;
                    cmd.Parameters.Add("@fileBytes", MySqlDbType.VarBinary, fileBytes.Length).Value = fileBytes;
                    cmd.Parameters.Add("@fileName", MySqlDbType.VarChar, fileName.Length).Value = fileName;
                    //Open connection to database
                    sqlconn.Open();

                    //Prepare statement for execution
                    cmd.Prepare();

                    //Execute SQL command
                    cmd.ExecuteNonQuery();

                    //Close connection to database and return results
                    sqlconn.Close();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                string sql = $"DELETE FROM {DataTable} WHERE Id = {id};";
                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(sql, sqlconn))
                {
                    //Open connection to database
                    sqlconn.Open();

                    //Prepare statement for execution
                    cmd.Prepare();

                    //Execute SQL command
                    cmd.ExecuteNonQuery();

                    //Close connection to database and return results
                    sqlconn.Close();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}