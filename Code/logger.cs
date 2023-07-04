using System;
using System.IO;
using System.Text;

namespace Game.ModCore
{
    public class LogWriter
    {
        private string _filePath;

        public LogWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void WriteLog(string message)
        {
            FileStream fs = File.Open(_filePath, FileMode.Append);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }
}