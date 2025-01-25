using System;
using System.IO;
using IWshRuntimeLibrary; // Requires a COM reference to Windows Script Host Object Model

public static class AutoStartHelper
{
    public static void AddToStartup()
    {
        string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupFolder, "SurefireTray.lnk");
        string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        if (!System.IO.File.Exists(shortcutPath))
        {
            try
            {
                // Create a shortcut using Windows Script Host
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = executablePath;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(executablePath);
                shortcut.Description = "Surefire Tray App";
                shortcut.IconLocation = executablePath;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add shortcut to startup: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Shortcut already exists in startup folder.");
        }
    }

    public static void RemoveFromStartup()
    {
        string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupFolder, "SurefireTray.lnk");

        if (System.IO.File.Exists(shortcutPath))
        {
            try
            {
                System.IO.File.Delete(shortcutPath);
                Console.WriteLine("Shortcut removed from startup folder.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove shortcut from startup: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Shortcut not found in startup folder.");
        }
    }
}
