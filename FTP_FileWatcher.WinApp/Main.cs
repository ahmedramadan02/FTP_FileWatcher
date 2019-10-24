using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FTP_FileWatcherLib;

namespace FTP_FileWatcher.WinApp
{
    public partial class Main : Form
    {
        //Global Objs
        //TODO: Add SFTP feature, use AWS SFTP servers
        public static string downloadPath = "./SDE_Assignment/Download"; //put in resources!
        public static string tmpPath = "./SDE_Assignment/tmp";
        static FTP_FileDownloader ftpClient;
        static DBManager dbMan;
        public static System.IO.FileStream hLogFile;

        //Check for updates every 10min
        //1. Downlaod the database file
        //2. Query on files date
        //3. If last hour then download files
        //timerCheck.Interval = 1000 * 60 * 10;
        System.Threading.Timer t;

        public Main()
        {
            InitializeComponent();

            // Create new Client
            ftpClient = new FTP_FileDownloader
                ("XXXusernameXXX", "XXXpasswordXXX", "XXXhostXXX", new FTP_FileDownloader.dlgtLog(log));
            ftpClient.SetSettings(downloadPath, tmpPath);

            //initial LogFile
            //Multithreading is not being understanded yet!
            //This var can't be initialized before ftp SetSettings method to be called!
            hLogFile
            = new System.IO.FileStream("./SDE_Assignment/log.txt",
                System.IO.FileMode.OpenOrCreate);

            //Check Conductivity
            tsLblStatus.Text = "Hey dude! " + DateTime.Now;
            tsLblStatus.Text = "Connecting...";
            if (ftpClient.PingServer() != -1)
                log("Connected to the FTP Server");
            else
            {
                log("[Error]::>Connection Failed");
                return;
            }

            //Create new database connection
            dbMan = new DBManager();

            //Dispose
            hLogFile.Close();
            hLogFile = null;
            GC.Collect();
        }

        public void log(string msg)
        {
            if (hLogFile == null) {
                //initial LogFile
                //Multithreading is not being understanded yet!
                hLogFile
                = new System.IO.FileStream("./SDE_Assignment/log.txt",
                    System.IO.FileMode.OpenOrCreate);
            }

            //Format the msg
            msg = "[" + DateTime.Now + "]" + "\t" + msg + "\r\n";

            //Msg on the console first
            tsLblStatus.Text = msg;

            //Write to the file
            if (hLogFile.CanRead)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(msg);
                hLogFile.Write(info, 0, info.Length);
                hLogFile.Flush();
            }
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
        }

        private void TimerCallBack(object state)
        {
            ProcessTick();
        }

        private void ProcessTick()
        {
            try
            {
                //1. Download Database file
                FTP_File db = new FTP_File(@"ahmed_sabry\FileLoggerDB.sqlite");
                db.LocalPath = "./SDE_Assignment/tmp/FileLoggerDB.sqlite";
                if (ftpClient.DownloadFile(db.FileInfo.FilePath, db.LocalPath))
                {
                    //Log operation
                    log("Database downloaded successsfully!");

                    //Query on files date
                    //if not found
                    dbMan.TestConnection();
                    List<FTP_File> tmpLst = dbMan.GetFilesToDownload();

                    //3. If last hour then download files
                    //if there are files
                    //then download it in another thread
                    if (tmpLst != null)
                    {
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

        private void btnConnect_Click(object sender, EventArgs e)
        {
           t = new 
                System.Threading.Timer(TimerCallBack, null, 0, 1000 * 60 * 10);

           btnConnect.Enabled = false;
           btnDisconnect.Enabled = true;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            t.Dispose();
            log("Disconnected...");
        }

    }
}
