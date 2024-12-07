using System;
using System.Management;
using CefSharp.Wpf;
using CefSharp;
using QTool.Controls;

namespace QTool.View.Utilities
{
    class WinHelper
    {
        public ulong GetPhysicalMemory()
        {
            try
            {
                //获得物理内存
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (mo["TotalPhysicalMemory"] != null)
                    {
                        var val = mo["TotalPhysicalMemory"];

                        if (val is ulong)
                        {
                            return (ulong)val;
                        }
                        else if (ulong.TryParse(val.ToString(), out ulong i))
                        {
                            return i;
                        }
                    }
                }
            }
            catch { }

            return 0;
        }


        /// <summary>
        /// 初始化Cef ChromiumWebBrowser
        /// </summary>
        public static void InitCef()
        {
            try
            {
                var settings = new CefSettings();
                settings.Locale = "zh-CN";
                settings.UserAgent = QRequestHeader.UserAgent;
                settings.AcceptLanguageList = QRequestHeader.AcceptLanguage;
                settings.WindowlessRenderingEnabled = false;
                Cef.Initialize(settings);
            }
            catch (Exception ex)
            {
                QMessageWindow.Show(ex.Message);
            }
        }
    }

}
