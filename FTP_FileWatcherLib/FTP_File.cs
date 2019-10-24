using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTP_FileWatcherLib
{
    public class FTP_File
    {
        private FTP_FileInfo _fileInfo;
        public FTP_FileInfo FileInfo
        {
            get { return _fileInfo; }
            set { _fileInfo = value; }
        }

        private string fileName = "";
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private bool isDownloaded = false;
        public bool IsDownloaded
        {
            get { return isDownloaded; }
            set { isDownloaded = value; }
        }

        private string localPath = "";
        public string LocalPath
        {
            get { return localPath; }
            set { localPath = value; }
        }

        private string fileExtension;
        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }

        private long fileSize = 0;
        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        //Constructors
        public FTP_File(string filePath, string updateTime = "")
        {
            //Check if SQLite file or not
            if (updateTime == "") {
                this._fileInfo.isDatabase = true;
            }

            //Set FTP_INFO
            this._fileInfo.FilePath = filePath;
            this._fileInfo.UpdateTime = updateTime;

            //Extract file info
            ExtractNameAndExt(filePath);
        }

        private void ExtractNameAndExt(string path) {
            if (path == "") {
                throw new ExceptionManager("File Path is not valid!");
            }

            string[] arr;
            if (path.Contains("/"))
                arr = path.Split('/');
            else
                arr = path.Split('\\');

            //FileName
            fileName = arr[arr.Count() - 1];

            //Get Extension
            fileExtension = fileName.Split('.')[1];
        }

        //public static void IsExists() { }
    }
}
