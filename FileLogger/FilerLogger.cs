using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace FileLogger {
    [Flags]
    public enum verbosityLevel {
        notVerbose = 0x01,
        verbose = 0x02,
        veryVerbose = 0x04,
        all = 0x08
    };

    public static class FileLogger {
        private class LogDetail {
            public string Data;
            public DateTime LogDate;

            public LogDetail(string data) {
                Data = data;
                LogDate = DateTime.Now;
            }

            public override string ToString() {
                return LogDate.ToString("hh:mm:ss:fff tt") + " \t" + Data;
            }
        }

        #region "Private variables"
        static string LogFileName = "logs.";
        static bool isVerboseMode;
        static string LogFilePath;
        static Thread thLogging;
        static StreamWriter ofileWrite;
        static ConcurrentQueue<LogDetail> _LogsQ;
        static bool runLogging;
        #endregion

        static verbosityLevel currentLevel { get; set; } = verbosityLevel.notVerbose;

        static FileLogger() {
            //Reading from Config file
            isVerboseMode = currentLevel > verbosityLevel.notVerbose;
            LogFilePath = Environment.CurrentDirectory;

            _LogsQ = new ConcurrentQueue<LogDetail>();


            if (!Directory.Exists(LogFilePath)) {
                Directory.CreateDirectory(LogFilePath);
            }

            CreateFileIfDateChanged(DateTime.Now);
            runLogging = true;
            thLogging = new Thread(StartLogging);
            thLogging.Start();
        }

        public static void AppendToLog(string message, verbosityLevel level = verbosityLevel.notVerbose) {
            if(level <= currentLevel)
                _LogsQ.Enqueue(new LogDetail(message));
        }

        static void StartLogging() {
            LogDetail oTmp;
            string sFileName;
            while (runLogging) {
                if (_LogsQ.Count > 0) {
                    _LogsQ.TryDequeue(out oTmp);

                    //When Date change create new file.
                    sFileName = LogFilePath + "\\" + LogFileName + DateTime.Today.ToString("yyyy_MM_dd") + ".log";

                    //File is not exist so create new file.
                    if (!File.Exists(sFileName)) {
                        FileStream fs = File.Create(sFileName);
                        fs.Close();

                        if (ofileWrite != null) {
                            ofileWrite.Close();
                        }
                        ofileWrite = new StreamWriter(sFileName, true);
                    }

                    ofileWrite.WriteLine(oTmp.LogDate.ToString("hh:mm:ss:fff tt") + " \t" + oTmp.Data);
                    ofileWrite.Flush();
                }
                else {
                    Thread.Sleep(500);
                }

            }
            if (ofileWrite != null) {
                ofileWrite.Close();
            }
        }

        private static DateTime logFileDate;
        static void CreateFileIfDateChanged(DateTime logDate) {
            if (logFileDate.Date == logDate.Date) { return; }

            logFileDate = logDate.Date;

            string fileName = Path.Combine(LogFilePath, LogFileName + logFileDate.ToString("yyyy_MM_dd") + ".log");

            if (ofileWrite != null) { ofileWrite.Close(); }

            ofileWrite = new StreamWriter(fileName, true);

        }
    }
}
