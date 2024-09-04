using Mantis.Domain.Renewals.Models;

namespace Mantis.Domain.Shared.Helpers
{
    public static class DataHelper
    {
        public static int RenewalProgressPercent(ICollection<TrackTask> tasks)
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
    }
}
