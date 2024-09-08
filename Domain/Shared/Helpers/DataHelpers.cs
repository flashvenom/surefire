using Mantis.Domain.Renewals.Models;
using System.Text.RegularExpressions;

namespace Mantis.Domain.Shared.Helpers
{
    public static class DataHelper
    {
        public static int RenewalProgressPercentWeighted(ICollection<TrackTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return 0;
            }

            int totalTasks = tasks.Count;
            double totalWeight = 0;
            double weightedCompleted = 0;

            for (int i = 0; i < totalTasks; i++)
            {
                // Higher weight for earlier tasks
                double weight = (totalTasks - i) / (double)totalTasks;
                totalWeight += weight;

                if (tasks.ElementAt(i).Completed)
                {
                    weightedCompleted += weight;
                }
            }

            // Calculate weighted completion percentage
            return (int)((weightedCompleted / totalWeight) * 100);
        }

        public static int RenewalProgressPercent(ICollection<TrackTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return 0;
            }

            int totalTasks = tasks.Count;
            int completedTasks = tasks.Count(task => task.Completed);

            // Calculate evenly distributed completion percentage
            return (int)((double)completedTasks / totalTasks * 100);
        }

        public static string BuildUploadPath(int contactId, string? firstName, string? lastName, string subfolder, string extension)
        {
            var cleanName = SanitizeFileName(firstName, lastName);
            var buildName = $"{contactId}-{cleanName}{extension}";

            return buildName;
        }

        private static string SanitizeFileName(string? firstName, string? lastName)
        {
            // Fallback to "headshot" if both names are null or empty
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                return "headshot";
            }

            // If only one of the names is null or empty, use the other
            var fullName = $"{firstName} {lastName}".Trim();

            // Remove any non-alphabetic characters (allowing only letters)
            fullName = Regex.Replace(fullName, "[^a-zA-Z]", "");

            // If the name is completely sanitized to empty (no letters), fallback to "headshot"
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "headshot";
            }

            return fullName;
        }

        public static string CropCarrierName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty; // Return an empty string if input is null or empty
            }

            // Split the input into words
            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Get the first two words
            var firstTwoWords = string.Join(" ", words.Take(2));

            // If the resulting first two words are less than 8 characters, return the first 10 characters
            if (firstTwoWords.Length < 8)
            {
                return input.Length <= 10 ? input : input.Substring(0, 10);
            }

            return firstTwoWords;
        }
    }
}
