using System.Collections.Generic;
using System.Linq;

namespace SurefireTray
{
    public static class WindowsControl
    {
        public static void PerformWindowsFunction(string emberFunction, List<string> parameters)
        {
            switch (emberFunction)
            {
                case "Windows_OpenFolder":
                    OpenFolder(parameters.FirstOrDefault());
                    break;
                case "Windows_OpenFile":
                    OpenFile(parameters.FirstOrDefault());
                    break;
                default:
                    SystemControl.Log($"Unknown ember windowscontrol function: {emberFunction}");
                    break;
            }
        }

        public static void OpenFolder(string folderPath)
        {
            SystemControl.Log($"Finding to Open: {folderPath}");
            if (!string.IsNullOrEmpty(folderPath))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", folderPath);
                    SystemControl.Log($"Opened folder in Explorer: {folderPath}");

                    // Bring the Explorer window to the front
                    SystemControl.BringToFront("CabinetWClass"); // "CabinetWClass" is the class name for Explorer windows
                }
                catch (System.Exception ex)
                {
                    SystemControl.Log($"Error opening folder in Explorer: {ex.Message}");
                }
            }
        }
        public static void OpenFile(string filePath)
        {
            SystemControl.Log($"Finding to Open: {filePath}");
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Open the file using the default application
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // Ensures the file opens with the associated application
                    });
                    SystemControl.Log($"Opened file: {filePath}");

                    // Optionally, bring the associated application to the front
                    // Remove this if unnecessary or unclear
                    SystemControl.BringToFront("CabinetWClass"); // Adjust class name if needed for specific apps
                }
                catch (System.Exception ex)
                {
                    SystemControl.Log($"Error opening file: {ex.Message}");
                }
            }
            else
            {
                SystemControl.Log("No file path provided to open.");
            }
        }

    }
}
