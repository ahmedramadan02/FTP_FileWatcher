using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace FTP_FileWatcherLib
{
    public class DBManager
    {
        private static string connectionString = @"Data Source=.\SDE_Assignment\tmp\FileLoggerDB.sqlite";
        private string databaseLocalPath = @".\SDE_Assignment\tmp\FileLoggerDB.sqlite";

        public string DatabaseLocalPath
        {
            get { return databaseLocalPath; }
            set { databaseLocalPath = value; }
        }
    
        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public void TestConnection()
        {
            //1. Check Files Existance 
            if (!System.IO.File.Exists(databaseLocalPath))
                throw new System.IO.FileNotFoundException("database file is not found, plz download it and then try again");

            //2. Check Connection
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        Console.WriteLine("database opened sucessfully!...");
                        conn.Close();
                    }
                }
                catch (SQLiteException)
                {
                    conn.Close();
                    throw;
                }
            }
        }

        //Query upon SQLite
        public List<FTP_File> GetFilesToDownload()
        {
            //Query for the files
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
                //Open the connection
                conn.Open();

                //Construct the command
                //Note that the time format of SQLite is YYYY-MM-DD HH:MM:SS
                //And note that the time system is 24hours
                SQLiteCommand comm = new SQLiteCommand();
                comm.CommandText = "SELECT * FROM FileInfo WHERE datetime(Update_Time) > datetime('now', '-1 hours')";
                comm.Connection = conn; //assoicate the command to the connection channel

                SQLiteDataReader rd = comm.ExecuteReader();
                if (rd.HasRows)
                {
                    //if there are items, create a new list
                    List<FTP_File> lstFiles = new List<FTP_File>();
                    while (rd.Read())
                    {
                        //Fill the list with files
                        lstFiles.Add(new FTP_File(Convert.ToString(rd["FTP_Path"]),
                                                  Convert.ToString(rd["Update_Time"])));
                    }

                    //Close the data reader
                    rd.Close();
                    comm = null;
                    conn.Close();

                    //Return the list
                    return lstFiles;
                }
                else //return null, you need to check on it
                {
                    //Close the data reader
                    rd.Close();
                    comm = null;
                    conn.Close();
                    throw new ExceptionManager("No new files!...");
                }
            }            
   
        }


        //Get the true SQLite time
        //Note that the time format of SQLite is YYYY-MM-DD HH:MM:SS
        //And note that the time system is 24hours
        public string GetSqliteTimeFormat() {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        ~DBManager() {
            GC.Collect();
        }
    }
}
