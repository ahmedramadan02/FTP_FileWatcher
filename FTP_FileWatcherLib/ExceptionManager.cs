using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTP_FileWatcherLib
{
    public class ExceptionManager : Exception
    {
        public ExceptionManager(string _msg) : base(_msg) { }
        public ExceptionManager(string _msg, Exception _inner) : base(_msg,_inner) { }
    }
}
