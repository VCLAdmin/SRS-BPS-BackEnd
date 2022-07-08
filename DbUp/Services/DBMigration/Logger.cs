using DbUp.Engine.Output;
using DBUp.Utils;
using System;
using System.IO;
using System.Text;

namespace DBUp.Services
{
    public class Logger : IUpgradeLog
    {
        private StringBuilder sb = new StringBuilder();

        public void WriteError(string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }

        public void WriteInformation(string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }

        public void WriteWarning(string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }

        public string GetLog()
        {
            var logtext = sb.ToString();
            return logtext;
        }

        public void LogToTextFile(string msg)
        {
            try
            {
                string filepath = Globals.AppPhysicalPath + @"\logs\";  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + DateTime.Today.ToString("MM.dd.yyyy") + "_DBMigration.txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    //sw.WriteLine(GetLog());
                    sw.WriteLine(DateTime.Now.ToLongTimeString() + "---------------------------------------------------------------------");
                    sw.WriteLine(msg);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}