using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FTP_FileWatcherLib;
using System.Threading;

namespace FTP_FileWatcherApp
{
    class Program
    {
        //Global Objs
        public static string downloadPath = "./SDE_Assignment/Download"; //put in resources!
        public static string tmpPath = "./SDE_Assignment/tmp";
        static FTP_FileDownloader ftpClient;
        static DBManager dbMan;
        public static System.IO.FileStream hLogFile;

        static void Main(string[] args)
        {

            // Create new Client
            ftpClient = new FTP_FileDownloader
                ("XXXusernameXXX", "XXXpasswordXXX", "XXXhostXXX", new FTP_FileDownloader.dlgtLog(log));
            ftpClient.SetSettings(downloadPath, tmpPath);

            //initial LogFile
            //This var can't be initialized before ftp SetSettings method to be called!
            hLogFile
            = new System.IO.FileStream("./SDE_Assignment/log.txt",
                System.IO.FileMode.OpenOrCreate);

            //Check Conductivity
            Console.WriteLine("Hey dude! " + DateTime.Now);
            Console.WriteLine("Connecting...");
            if (ftpClient.PingServer() != -1)
                log("Connected to the FTP Server");
            else
            {
                log("[Error]::>Connection Failed");
                return;
            }

            //Create new database connection
            dbMan = new DBManager();
            
            //Check for updates every 10min
            //1. Downlaod the database file
            //2. Query on files date
            //3. If last hour then download files
            Timer t = new Timer(TimerCallBack, null, 0, 1000*60*10);
            Console.ReadLine();

            //Dispose
            hLogFile.Close();
            hLogFile = null;
            GC.Collect();
        }



        private static void TimerCallBack(object state)
        {
            try
            {
                //1. Download Database file
                FTP_File db = new FTP_File(@"ahmed_sabry\FileLoggerDB.sqlite");
                db.LocalPath = "./SDE_Assignment/tmp/FileLoggerDB.sqlite";
                if (ftpClient.DownloadFile(db.FileInfo.FilePath, db.LocalPath)) {
                    //Log operation
                    log("Database downloaded successsfully!");

                    //Query on files date
                    //if not found
                    dbMan.TestConnection();
                    List<FTP_File> tmpLst = dbMan.GetFilesToDownload();

                    //3. If last hour then download files
                    //if there are files
                    //then download it in another thread
                    if (tmpLst != null) {
                        ftpClient.DownloadFiles(tmpLst);
                    }
                }

                //Clean Memeory
                GC.Collect();
            }
            catch (Exception ex)
            {
                //log the errors
                log("(Error)::>" + ex.Message);
            }
        }

        public static void log(string msg)
        {
            if (hLogFile == null)
            {
                //initial LogFile
                hLogFile
                = new System.IO.FileStream("./SDE_Assignment/log.txt",
                    System.IO.FileMode.OpenOrCreate);
            }

            //Format the msg
            msg = "[" + DateTime.Now + "]" + "\t" + msg + "\r\n";

            //Msg on the console first
            Console.Write(msg);

            //Write to the file
            if (hLogFile.CanRead) {
                byte[] info = new UTF8Encoding(true).GetBytes(msg);
                hLogFile.Write(info, 0, info.Length);
                hLogFile.Flush();
            }
        }
    }
}
