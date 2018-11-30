using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using PiaNotes.Models;

namespace PiaNotes.ViewModels
{
    public class Databaser
    {
        //Set connection string (ROOT USER IS FOR TESTING ONLY, Use system instead)
        private const string ConnectionString = "SERVER = localhost; DATABASE = pianotes; Uid = root; Pwd = ";

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
                // Not developed yet.
                throw new NotImplementedException();
            }
        }

        //Search a single MusicSheet in the database using an Id
        public MusicSheet SearchId(int id)
        {
            try
            {
                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand($"SELECT * FROM musicsheet WHERE id = { id }", sqlconn))
                {
                    //Sheet to be used if search comes up empty
                    MusicSheet result = null;

                    //Open connection to database
                    sqlconn.Open();

                    //Prepare statement for execution
                    cmd.Prepare();

                    //Execute SQL command
                    var go = cmd.ExecuteReader();

                    //Read results
                    go.Read();

                    //Get found Id, Title and Path
                    if (go.HasRows == true)
                    {
                        var FoundId = go.GetInt32(0); var FoundTitle = go.GetString(1); var FoundPath = go.GetString(2);
                        //Make a new MusicSheet using found data
                        result = new MusicSheet(FoundId, FoundTitle, FoundPath);
                    }

                    //Close connection to database and return results
                    sqlconn.Close();
                    return result;
                }
            }
            catch
            {
                // Not developed yet.
                throw new NotImplementedException();
            }
        }

        //Search through MusicSheets in the database using a Title
        public List<MusicSheet> SearchTitle(string title)
        {
            try
            {
                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand($"SELECT * FROM musicsheet WHERE Title = '{ title }'", sqlconn))
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
                            var FoundId = go.GetInt32(0); var FoundTitle = go.GetString(1); var FoundPath = go.GetString(2);

                            //Make a new MusicSheet using found data and add it to the list
                            result.Add(new MusicSheet(FoundId, FoundTitle, FoundPath));
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
                // Not developed yet.
                throw new NotImplementedException();
            }
        }

        //Search through MusicSheets in the database with a limit
        public List<MusicSheet> SearchNumber(int limit, int offset)
        {
            try
            {
                //Setup connection and SQL command
                using (MySqlConnection sqlconn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand($"SELECT * FROM musicsheet LIMIT { limit } OFFSET {offset}", sqlconn))
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
                            var FoundId = go.GetInt32(0); var FoundTitle = go.GetString(1); var FoundPath = go.GetString(2);

                            //Make a new MusicSheet using found data and add it to the list
                            result.Add(new MusicSheet(FoundId, FoundTitle, FoundPath));
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
                // Not developed yet.
                throw new NotImplementedException();
            }
        }

        //Search through MusicSheets in the database with a limit
        public List<MusicSheet> Search(string select, string whereA, string whereB, int limit, int offset)
        {
            try
            {
                //Set query standaard.
                var Select = $"SELECT * FROM musicsheet ";
                var Where = $"";
                var Limit = $"";
                var Offset = $"";

                //If function has specific selected, change it in the query.
                if (select != null) { Select = $"SELECT {select} FROM musicsheet "; }
                
                //If both Wheres are specified add a WHERE to the query.
                if (whereA != null && whereB != null) { Where = $"WHERE UPPER({whereA}) LIKE UPPER('{whereB}%') "; }

                //If a limit is specified, add a LIMIT to the query
                if (limit != 0) { Limit = $"LIMIT {limit} "; }

                //If offset is specified, check if LIMIT is also specified, if yes, add OFFSET to query
                if (offset != 0 && limit != 0) { Offset = $"OFFSET {offset} "; }

                //Build the sql into a string
                string sql = Select + Where + Limit + Offset;

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
                            var FoundId = go.GetInt32(0); var FoundTitle = go.GetString(1); var FoundPath = go.GetString(2);

                            //Make a new MusicSheet using found data and add it to the list
                            result.Add(new MusicSheet(FoundId, FoundTitle, FoundPath));
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

        public bool UploadToDB(MusicSheet sheet)
        {
            try
            {
                //Set query standaard.
                var Select = $"SELECT * FROM musicsheet ";
                var Where = $"";
                var Limit = $"";
                var Offset = $"";

                //Build the sql into a string
                string sql = Select + Where + Limit + Offset;

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