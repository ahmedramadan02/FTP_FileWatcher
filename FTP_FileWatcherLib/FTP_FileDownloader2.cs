using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTP_FileWatcherLib
{

    //this is part2 of the class FTP_FileDownloader
    //this part is not used in our projects
    //but may be used in the serverSide
    public partial class FTP_FileDownloader
    {
        //Other functions may be used in ftpClient for extensibility 
        //Not required for this project
        private void UploadFile(string localFile, string remoteFile)
        {
            try
            {
                //Request Upload
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                //Prepare the file to Upload
                FileStream lfs = new FileStream(localFile, FileMode.OpenOrCreate);
                byte[] byteBuffer = new byte[lfs.Length];
                int bytesSend = lfs.Read(byteBuffer, 0, Convert.ToInt32(lfs.Length));

                //Uploading
                Stream ftpStream = null; //Error
                while (bytesSend != -1)
                {
                    ftpStream.Write(byteBuffer, 0, bytesSend);
                    bytesSend = lfs.Read(byteBuffer, 0, Convert.ToInt32(lfs.Length));
                }

                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Rename(string oldName, string newName)
        {
            try
            {
                //Create Rename request
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + oldName);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = newName;

                //check response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                ftpResponse.Close();
                ftpResponse = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] Get_FilesonServer(string dir)
        {
            string[] filesInDir = new string[50];

            try
            {
                //Create Rename request
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + dir);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                //Get Response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                Stream ftpStream = ftpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(ftpStream);
                string dirRaw = null;

                while (sr.Peek() != -1)
                {
                    dirRaw += sr.ReadLine() + "|";
                }

                filesInDir = dirRaw.Split("|".ToCharArray());

                //null all
                ftpResponse.Close();
                sr.Dispose();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception)
            {

                throw;
            }

            return filesInDir;
        }

        public void Delete(string filename)
        {
            try
            {
                //Create Request
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + filename);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UsePassive = true;
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                //Get Response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                //Clean
                ftpRequest = null;
                ftpResponse.Close();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CreateDir(string dirName)
        {
            try
            {
                //Create Request
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(hostname + "/" + dirName);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UsePassive = true;
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

                //Get Response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                //Clean
                ftpResponse.Close();
                ftpRequest = null;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
