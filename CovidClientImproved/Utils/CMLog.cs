using System;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;

namespace CovidClientImproved.Utils
{
    public static class CMLog
    {
        private static readonly object _syncLock = new object();
        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private static readonly string _logDirectory = $"{Application.persistentDataPath}/CovidClient/cm_logs";
        private static readonly string _logFileName = $"CMLog_{DateTime.Now:yyyyMMdd}.txt";

        private static readonly int _maxLogSize = 1000000; // 1MB
        private static readonly int _maxLogsCount = 100;
        private static readonly int _maxFiles = 15;
        private static readonly TimeSpan DeleteAfter = TimeSpan.FromDays(1);

        public enum LogLevel { Debug, Info, Warning, Error, Exception }

        public static void HandleExceptions() =>
            AppDomain.CurrentDomain.UnhandledException += ManageExceptions;

        private static void ManageExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            Log(LogLevel.Exception, ex.StackTrace);
        }

        private static void Log(LogLevel level, string message)
        {
            try
            {
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}\r\n";
                lock (_syncLock)
                {
                    WriteToFile(logEntry);
                }
            }
            catch { }
        }

        private static void LogException(LogLevel level, Exception e)
        {
            try
            {
                var logEntry = $"{DateTime.Now:yyy-MM-dd HH:mm:ss} [{level}] {e.Message}\r\n {e.StackTrace}\r\n";
                lock (_syncLock)
                {
                    WriteToFile(logEntry);
                }
            }
            catch { }
        }

        private static void ProcessLogQueue()
        {
            try
            {
                if (!_logQueue.IsEmpty)
                {
                    string logEntry;
                    while (_logQueue.TryDequeue(out logEntry))
                    {
                        WriteToFile(logEntry);
                    }
                }
            }
            catch { }
        }

        private static void WriteToFile(string logEntry)
        {
            try
            {
                EnsureDirectoryExists();

                var logFile = Path.Combine(_logDirectory, _logFileName);
                File.AppendAllText(logFile, logEntry);

                CheckAndRotateLogFile(logFile);
            }
            catch { }
        }

        private static void CheckAndRotateLogFile(string logFile)
        {
            try
            {
                if (!IsLogFileLocked(logFile))
                {
                    if (File.Exists(logFile) && File.ReadAllBytes(logFile).Length > _maxLogSize ||
                        File.ReadAllLines(logFile).Length > _maxLogsCount)
                    {
                        DeleteOldLogFiles();
                        CreateNewLogFile();
                    }
                }
            }
            catch { }
        }

        private static bool IsLogFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch { return true; }
        }

        private static void EnsureDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch { }
        }

        private static void DeleteOldLogFiles()
        {
            try
            {
                if (Directory.Exists(_logDirectory))
                {
                    string[] files = Directory.GetFiles(_logDirectory);

                    foreach (string filePath in files)
                    {
                        FileInfo fileInfo = new FileInfo(filePath);

                        DateTimeOffset creationTime = fileInfo.CreationTimeUtc;

                        if (creationTime < DateTimeOffset.UtcNow - DeleteAfter || files.Length > _maxFiles)
                        {
                            File.Delete(filePath);
                        }
                    }
                }
            }
            catch { }
        }

        private static void CreateNewLogFile()
        {
            try
            {
                var newLogFile = Path.Combine(_logDirectory, _logFileName);
                File.WriteAllText(newLogFile, "");
            }
            catch { }
        }

        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warning(string message) => Log(LogLevel.Warning, message);
        public static void Error(string message) => Log(LogLevel.Error, message);
        public static void Exception(Exception e) => LogException(LogLevel.Exception, e);
        public static void Debug(string message) => Log(LogLevel.Debug, message);
    }
}
