using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICOMPANY.CommonFunctions.Logger
{

    public interface IDynamicLogger
    {
        fn.CommonOperationResult log(object text);
    }
    public static class Logger
    {
        //Класс, занимающийся логгированием
        public static string fileName = "";
        public static bool logIsOn = true;
        public static LogDirectionEnum logDirection = LogDirectionEnum.toConsole;
        public static bool imTheAspNetService = false;
        public static void prepare(bool killLogs = false)
        {
            if (logDirection == LogDirectionEnum.bothToConAndFile || logDirection == LogDirectionEnum.toFile)
            {
                FileInfo file = new FileInfo(fileName);
                if (killLogs || !file.Exists)
                {
                    StreamWriter sw = file.CreateText();
                    sw.Close();
                }
            }
            //            System.IO.File.Delete(fileName);

            //string path = Path.GetTempFileName();
        }

        public static void writeToConsole(string s)
        {
            if (imTheAspNetService)
            {
                Debug.WriteLine(s);
            }
            else
            {
                Console.WriteLine(s);
            }
        }
        public static void writeToFile(string s)
        {
            string writePath = fileName;
            StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default);
            sw.WriteLine(s);
            sw.Close();
        }

        public static fn.CommonOperationResult log(object text)
        {
            return log("", text);
        }
        public static fn.CommonOperationResult log(object domain, object text)
        {
            try
            {
                string s = Convert.ToString(domain).ToUpper() + "_" + fn.toStringNullConvertion(text);
                if (logIsOn)
                {
                    switch (logDirection)
                    {
                        case LogDirectionEnum.toFile:
                            writeToFile(s);
                            break;
                        case LogDirectionEnum.toConsole:
                            writeToConsole(s);
                            break;
                        case LogDirectionEnum.bothToConAndFile:
                            writeToFile(s);
                            writeToConsole(s);
                            break;
                    }
                }
                return fn.CommonOperationResult.sayOk();
            }
            catch (Exception ex)
            {
                return fn.CommonOperationResult.sayFail(ex.Message);

            }

        }
    }

    public class DynamicLogger : IDynamicLogger
    {
        public DynamicLogger(string _fileName, string _alias, LogDirectionEnum _logDirection = LogDirectionEnum.toConsole)
        {
            fileName = _fileName;
            alias = _alias;
            logDirection = _logDirection;
            if (logDirectionAffectsFile(logDirection)) prepare();
        }

        private bool logDirectionAffectsFile(LogDirectionEnum logDirection)
        { return logDirection == LogDirectionEnum.bothToConAndFile || logDirection == LogDirectionEnum.toFile; }

        //Класс, занимающийся логгированием
        private string alias = "";
        private string fileName = "";
        public bool logIsOn { get; set; } = true;
        public LogDirectionEnum logDirection;
        public bool imTheAspNetService = false;
        public void prepare(bool killLogs = false)
        {
            if (logDirectionAffectsFile(logDirection))
            {
                FileInfo file = new FileInfo(fileName);
                if (killLogs || !file.Exists)
                {
                    StreamWriter sw = file.CreateText();
                    sw.Close();
                }
            }
            //            System.IO.File.Delete(fileName);

            //string path = Path.GetTempFileName();
        }

        private void writeToConsole(string s)
        {
            if (imTheAspNetService)
            {
                Debug.WriteLine(s);
            }
            else
            {
                Console.WriteLine(s);
            }
        }
        private void writeToFile(string s)
        {
            string writePath = fileName;
            StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default);
            sw.WriteLine(s);
            sw.Close();
        }
        public fn.CommonOperationResult log(object text)
        {
            try
            {
                string s = Convert.ToString(alias).ToUpper() + "_" + fn.toStringNullConvertion(text);
                if (logIsOn)
                {
                    switch (logDirection)
                    {
                        case LogDirectionEnum.toFile:
                            writeToFile(s);
                            break;
                        case LogDirectionEnum.toConsole:
                            writeToConsole(s);
                            break;
                        case LogDirectionEnum.bothToConAndFile:
                            writeToFile(s);
                            writeToConsole(s);
                            break;
                    }
                }
                return fn.CommonOperationResult.sayOk();
            }
            catch (Exception ex)
            {
                return fn.CommonOperationResult.sayFail(ex.Message);
            }
        }
    }
    public enum LogDirectionEnum
    {
        toConsole = 1,
        toFile = 2,
        bothToConAndFile = 3

    }
}
