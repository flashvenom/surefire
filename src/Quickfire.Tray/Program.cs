using System;
using System.Windows.Forms;

namespace Quickfire.Tray
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AutoStartHelper.AddToStartup(); // Add shortcut to startup
            Console.WriteLine("Starting Quickfire.Tray");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SystemTray());
        }
    }
}
