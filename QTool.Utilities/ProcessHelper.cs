﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class ProcessHelper
    {

        /// 该函数设置由不同线程产生的窗口的显示状态  
        /// </summary>  
        /// <param name="hWnd">窗口句柄</param>  
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>  
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>  
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>  
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。  
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。   
        /// </summary>  
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>  
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>  
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOW = 5;
        public static bool HandleRunningInstance(Process instance)
        {
            var result = ShowWindowAsync(instance.MainWindowHandle, SW_SHOW);//显示 
            if (result)
            {
                return SetForegroundWindow(instance.MainWindowHandle);//当到最前端  
            }
            else
            {
                return false;
            }
        }


        public static bool CheckProcess()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processList = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (var process in processList)
            {
                if (process.Id != currentProcess.Id && string.Compare(process.StartInfo.FileName, currentProcess.StartInfo.FileName, true) == 0)
                {
                    if (HandleRunningInstance(process))
                    {
                        return true;
                    }
                    else
                    {
                        process.Kill();
                    }
                }
            }

            return false;
        }
    }
}
