using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Quickfire.Desktop.Services
{
    public interface IQuickfireTrayService
    {
        void EnsureStarted();
    }

    public sealed class QuickfireTrayService : IQuickfireTrayService
    {
        private readonly ILogger<QuickfireTrayService> _logger;
        private bool _launchAttempted;

        public QuickfireTrayService(ILogger<QuickfireTrayService> logger)
        {
            _logger = logger;
        }

        public void EnsureStarted()
        {
            if (_launchAttempted)
            {
                return;
            }

            _launchAttempted = true;

            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            try
            {
                if (Process.GetProcessesByName("Quickfire.Tray").Any())
                {
                    _logger.LogInformation("Quickfire.Tray is already running.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Unable to check for existing Quickfire.Tray process.");
            }

            var trayPath = ResolveTrayPath();
            if (trayPath is null)
            {
                _logger.LogWarning("Quickfire.Tray executable not found; skipping tray launch.");
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo(trayPath)
                {
                    WorkingDirectory = Path.GetDirectoryName(trayPath) ?? AppContext.BaseDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(startInfo);
                _logger.LogInformation("Quickfire.Tray started from {TrayPath}.", trayPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Quickfire.Tray from {TrayPath}.", trayPath);
            }
        }

        private static string? ResolveTrayPath()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDirectory, "Tray", "Quickfire.Tray.exe"),
                Path.Combine(baseDirectory, "Quickfire.Tray.exe")
            };

            return candidates.FirstOrDefault(File.Exists);
        }
    }
}
