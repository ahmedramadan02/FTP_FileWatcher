using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTP_FileWatcherLib
{
    public static class Utilities
    {
        //this function will create new folder for downlaoded files
        //with a certain schema related to its time
        //For example if there are new files on tha last hours
        //the files will be downloaded in such directory like 'SDE-2015614-22'
        public static string CreateDirForDownloadedFiles()
        {
            //Format the name of the directory
            string dirName = "SDE-";
            string timeFormat = Convert.ToString(DateTime.Now.Year) +
                                Convert.ToString(DateTime.Now.Month) +
                                Convert.ToString(DateTime.Now.Day);
            timeFormat += "-" + Convert.ToString(DateTime.Now.Hour);

            dirName += timeFormat;
            string dirFullPath = "." + "/SDE_Assignment/Download/";

            try
            {
                //Create the directory if not exists
                if (!System.IO.Directory.Exists(dirFullPath + dirName))
                    System.IO.Directory.CreateDirectory(dirFullPath + dirName);

                //Accumlate the dirPath
                dirFullPath += dirName;

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw;
            }

            // if ok return the directory name for the next process
            return dirFullPath;
        }

        public static void CreateDir(string dirPath) {
            if (!System.IO.Directory.Exists(dirPath))
                System.IO.Directory.CreateDirectory(dirPath);
        }
    }
}
