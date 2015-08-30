using System;
using System.IO;
using System.Text;

namespace Core.Utils
{
    public class Log
    {
        public Log(string name)
        {
            LogLocation = AppDomain.CurrentDomain.BaseDirectory + "\\" + name;
        }
        private string LogLocation;

        public void WriteLog(string line)
        {
            using (StreamWriter LogFile = new StreamWriter(LogLocation, true, Encoding.ASCII))
            {
                LogFile.Write(DateTime.Now + " : " + line + "\r\n");
            }
        }
    }
}
