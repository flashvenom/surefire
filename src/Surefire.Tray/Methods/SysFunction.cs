using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SurefireTray
{
    public static class SystemControl
    {
        private static string _logFilePath = @"C:\SUREFIRE\TrayLog.txt";

        public static void BringToFront(string className)
        {
            IntPtr hwnd = FindWindow(className, null);
            if (hwnd != IntPtr.Zero)
            {
                SetForegroundWindow(hwnd);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}\n");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Failed to log message: {ex.Message}");
            }
        }

    }
}