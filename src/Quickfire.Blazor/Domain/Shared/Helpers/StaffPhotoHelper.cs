using System;

namespace Quickfire.Blazor.Domain.Shared.Helpers
{
    public static class StaffPhotoHelper
    {
        public const string DefaultPhotoFileName = "default.jpg";
        private static readonly string[] LegacyDefaults = { "default.jpg", "default.png" };

        public static string ResolveFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return DefaultPhotoFileName;
            }

            var normalized = fileName.Trim().Replace('\\', '/');
            var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var leaf = segments.Length > 0 ? segments[^1] : string.Empty;

            if (string.IsNullOrWhiteSpace(leaf) || IsLegacyDefault(leaf))
            {
                return DefaultPhotoFileName;
            }

            return leaf;
        }

        public static string BuildUrl(string? fileName)
        {
            return $"/uploads/headshots/{ResolveFileName(fileName)}";
        }

        private static bool IsLegacyDefault(string fileName)
        {
            foreach (var legacy in LegacyDefaults)
            {
                if (string.Equals(fileName, legacy, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
