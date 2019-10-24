using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;

namespace FTP_FileWatcherLib
{
    public partial class FTP_FileDownloader
    {
        //Data
        private static string username;
        private static string password;
        private static string hostname;

        //Settings
        private static FTP_Settings ftpSettings;

        private readonly int bufferSize = 2048;
        private Queue<FTP_File> filesToDownload;

        //Percentage of downlaod
        private float _percentage = 0f;
        public float Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        } 

        //Delegate for log
        public delegate void dlgtLog(string data);
        private dlgtLog d;

        public FTP_FileDownloader(string _username, string _password, string _hostname)
        {
            //initial data
            username = _username;
            password = _password;
            hostname = _hostname;
        }

        public FTP_FileDownloader(string _username, string _password, string _hostname, dlgtLog _d)
        {
            //initial data
            username = _username;
            password = _password;
            hostname = _hostname;

            //initial delegate
            d = _d;
        }

        //Setup the workspace
        //Create folders if not exists
        //This procedure will be called on changing the ftpSettings Property
        private static void SetupWorkSpace() {
            Utilities.CreateDir("./SDE_Assignment"); //Create the mainDirectory
            Utilities.CreateDir(ftpSettings._downloadPath); //DownloadPath
            Utilities.CreateDir(ftpSettings._tmpPath); //tmp Path
        }

        //Check if the server is down
        public int PingServer() {
            string[] temp = this.Get_FilesonServer("/");

            if (temp == null) return -1;
            else return 1;            
        }

        //Set the default values for download and temp folders
        public void SetSettings(string __downPath = "./SDE_Assignment/Download", 
            string __tmpPath = "./SDE_Assignment/tmp") {
            ftpSettings._downloadPath = __downPath;
            ftpSettings._tmpPath = __tmpPath;

            //Create folders if not exists
            SetupWorkSpace();
        }

        //Downloading and uploading multiple files in new thread
        //The files on queue will be downloaded
        //public void DownloadFiles(string[] filesPaths) { }
        //public void DownloadFiles(string FTP_FolderName){ }
        public void DownloadFiles(List<FTP_File> files) {
            //Create new folder for the files
            string downloadDir = Utilities.CreateDirForDownloadedFiles();

            //Queue the files
            filesToDownload = new Queue<FTP_File>();
            foreach (var file in files)
            {
                //Provide each file with the downlaod path 
                file.LocalPath = downloadDir + "/" + file.FileName;                     

                //put it into queue
                filesToDownload.Enqueue(file);

            }

            //create a new thread
            Thread t = new Thread(unused => DownloadQueue());
            t.Priority = ThreadPriority.Highest;
            t.IsBackground = true;
            t.Start();
        }

        //This function will return true if database finished download
        //I called it critical download because all processes will be based on it
        //So the DB downlaod must be finsihed in order continue 
        public bool DownloadFile(string remoteFile, string localFile)
        {
            lock (this)
            {
                d.Invoke("Downloading critical...");
                Thread t = new Thread(unused => Downloading(remoteFile, localFile));
                t.IsBackground = true;
                t.Priority = ThreadPriority.Highest;
                t.Start();
                t.Join();
            }

            d.Invoke("Downlaod critical completed...");
            return true;
        }

        private void DownloadQueue() { 
            for (int i = 0; i <= filesToDownload.Count; i++)
			{
                FTP_File tmpFile = filesToDownload.Dequeue();
                Downloading(tmpFile.FileInfo.FilePath
                    ,tmpFile.LocalPath);
			}

            d.Invoke("All Files downloaded successfully");
        }

        private void Downloading(string remoteFile, string localFile)
        {
            try
            {
                //Prepare request
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                //These sizes will used later to calculate the percentage
                Int32 _currentFileSize = Convert.ToInt32(GetFileSize(remoteFile));
                Int32 _currentStreamSize = 0;

                //Get Response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                Stream ftpStream = ftpResponse.GetResponseStream(); //FileStreamGet

                //Save the response
                FileStream fs = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[Convert.ToInt32(GetFileSize(remoteFile))];

                int bytesRead = ftpStream.Read(byteBuffer, 0, _currentFileSize);
               

                //Start Downloading
                while (bytesRead > 0)
                {
                    //Get the file 
                    fs.Write(byteBuffer, 0, bytesRead);
                    bytesRead = ftpStream.Read(byteBuffer, 0, _currentFileSize);

                    //Calculate the percentage
                    _currentStreamSize = Convert.ToInt32(fs.Length);
                    CalculateDownloadPercentage(_currentFileSize,_currentStreamSize);
                    d.Invoke("Downloading " + _percentage + "%");
                    
                }

                fs.Close();
                ftpStream.Close();
                ftpRequest = null;
                ftpResponse.Close();

                //Send Log
                if (d != null)
                    d.Invoke("downlaoding...");

                //Kill Thread
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private long GetFileSize(string filename) {
            long size = 0;

            //Send size request
            FtpWebRequest sizeRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + filename);
            sizeRequest.Credentials = new NetworkCredential(username, password);
            sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;

            //Get response
            FtpWebResponse sizeResponse = (FtpWebResponse)sizeRequest.GetResponse();
            size = sizeResponse.ContentLength;

            return size;
        }

        //Function to calculate the download percentage
        private void CalculateDownloadPercentage(int _size, int _streamSize) {
            float lpercentage = (float)(_streamSize*100 / _size);
            _percentage = lpercentage;
        }

        ~FTP_FileDownloader() {
            GC.Collect();
        }
    }
}
