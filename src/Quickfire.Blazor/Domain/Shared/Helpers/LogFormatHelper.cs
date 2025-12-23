using System.Text.RegularExpressions;

namespace Quickfire.Blazor.Domain.Shared.Helpers
{
    public static class LogFormatHelper
    {
        private static readonly Regex LogEntryRegex = new Regex(
            @"^\[(?<timestamp>[^\]]+)\]\s+(?<level>\w+):\s*(?<message>.*)",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

        public static LogEntry ParseLogEntry(string logText)
        {
            var match = LogEntryRegex.Match(logText);
            
            if (match.Success)
            {
                return new LogEntry
                {
                    Timestamp = match.Groups["timestamp"].Value,
                    Level = match.Groups["level"].Value,
                    Message = match.Groups["message"].Value
                };
            }

            // Fallback for unparseable logs
            return new LogEntry
            {
                Timestamp = "",
                Level = "Unknown",
                Message = logText
            };
        }

        public static string GetLogLevelCssClass(string level)
        {
            return level.ToLower() switch
            {
                "information" => "log-level-info",
                "info" => "log-level-info",
                "warning" => "log-level-warning",
                "error" => "log-level-error",
                "debug" => "log-level-debug",
                "trace" => "log-level-trace",
                _ => "log-level-unknown"
            };
        }

        public static string GetDisplayLevel(string level)
        {
            return level.ToLower() switch
            {
                "information" => "Info",
                _ => level
            };
        }

        public static string FormatTimestamp(DateTime timestamp)
        {
            // Format: MM/dd/yyyy HH:mm:ss AM/PM with leading zeros
            return timestamp.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        public static string TruncateMessage(string message, int maxLength = 200)
        {
            if (string.IsNullOrEmpty(message) || message.Length <= maxLength)
                return message;

            return message.Substring(0, maxLength - 3) + "...";
        }
    }

    public class LogEntry
    {
        public string Timestamp { get; set; } = "";
        public string Level { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
