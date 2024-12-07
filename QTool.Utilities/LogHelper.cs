using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class LogHelper
    {
        private readonly static ConcurrentQueue<Msg> _queue = new ConcurrentQueue<Msg>();

        static LogHelper()
        {
            LogPath = GlobalHelper.CreatePath("Logs");

            Task task = new Task(() =>
            {
                do
                {
                    while (_queue.TryDequeue(out Msg msg))
                    {
                        SaveToFile(msg);
                    }

                    Task.Delay(1000).Wait();
                } while (true);
            });

            task.Start();
        }


        /// <summary>
        /// 日志目录
        /// </summary>
        public static string LogPath { get; }

        private static void SaveToFile(Msg msg, int retryCount = 3)
        {
            lock (_queue)
            {
                var logFile = GetLogFile(msg.Time);
                File.AppendAllLines(logFile, new string[] { msg.Time.ToString("HH:mm:ss"), msg.Content, string.Empty, string.Empty }, Encoding.UTF8);
            }
        }

        private static ConcurrentDictionary<DateTime, string> _logFileMaps = new ConcurrentDictionary<DateTime, string>();
        private static string GetLogFile(DateTime time)
        {
            if (!_logFileMaps.TryGetValue(time.Date, out string logFile))
            {
                logFile = Path.Combine(LogPath, time.ToString("yyyyMMdd'.log'"));
                _logFileMaps.TryAdd(time.Date, logFile);
            }
            return logFile;
        }

        public static void WriteLog(string msg)
        {
            _queue.Enqueue(new Msg(msg));
        }

        public static void WriteLog(Exception exception)
        {
            WriteLog(exception.ToString());
        }

        class Msg
        {
            public Msg(string content)
            {
                Time = DateTime.Now;
                Content = content;
            }

            public DateTime Time { get; }

            public string Content { get; }
        }
    }
}
