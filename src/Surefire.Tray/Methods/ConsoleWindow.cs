using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SurefireTray
{
    public static class ConsoleWindow
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private static bool _consoleVisible = true;
        private static string _logFilePath = @"C:\SUREFIRE\TrayLog.txt";

        public static void ShowConsoleWindow()
        {
            if (!_consoleVisible)
            {
                AllocConsole();
                _consoleVisible = true;

                // Redirect the console output to ensure a valid handle
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

                // Replay log file contents
                ReplayLog();
            }
        }

        public static void HideConsoleWindow()
        {
            if (_consoleVisible)
            {
                Console.WriteLine("Closing Debug Console...");
                Console.Out.Flush(); // Ensure all output is written before closing
                FreeConsole();
                _consoleVisible = false;
            }
        }

        public static void ToggleConsoleWindow()
        {
            if (_consoleVisible)
            {
                HideConsoleWindow();
            }
            else
            {
                ShowConsoleWindow();
            }
        }

        private static void ReplayLog()
        {
            if (File.Exists(_logFilePath))
            {
                string[] logLines = File.ReadAllLines(_logFilePath);
                foreach (string line in logLines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine("No log file found. Starting fresh...");
            }
        }

    }
}
