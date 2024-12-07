using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QTool
{
    public static class FileHelper
    {
        public static void Delete(string fileName, int retryCnt = 3)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    if (retryCnt > 1)
                    {
                        Thread.Sleep(500);
                        Delete(fileName, retryCnt - 1);
                        return;
                    }
                }
            }
        }

        public static void Delete(string[] files)
        {
            foreach (var file in files)
            {
                Delete(file);
            }
        }


        public static void ClearHistoryFiles(string path, int reserveDays)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                DateTime utcNow = DateTime.UtcNow;
                foreach (var file in files)
                {
                    if ((utcNow - File.GetLastWriteTimeUtc(file)).TotalDays > reserveDays)
                    {
                        Delete(file);
                    }
                }

                var cPaths = Directory.GetDirectories(path);
                foreach (var cPath in cPaths)
                {
                    ClearHistoryFiles(cPath, reserveDays);

                    if (Directory.Exists(cPath) && (utcNow - Directory.GetLastWriteTimeUtc(path)).TotalDays > reserveDays && Directory.GetFiles(cPath).Length == 0 && Directory.GetDirectories(cPath).Length == 0)
                    {
                        Directory.Delete(cPath);
                    }
                }
            }
        }

        public static void DeleteDirectory(string path, int retryCnt = 3)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch
                {
                    if (retryCnt > 1)
                    {
                        Thread.Sleep(500);
                        DeleteDirectory(path, retryCnt - 1);
                        return;
                    }
                }
            }
        }

        public static string ReplaceInvalidPathChars(this string pathName)
        {
            if (pathName is null)
            {
                throw new ArgumentNullException(nameof(pathName));
            }

            var invalidChars = Path.GetInvalidPathChars();
            var newValue = pathName;
            foreach (var c in invalidChars)
            {
                newValue = newValue.Replace(c, '_');
            }

            return newValue;
        }

        public static void Start(this string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch (Exception e)
            {
                QMessageHelper.Show($"文件“{fileName}”打开失败，失败原因：{e.Message}");
            }
        }
    }
}
