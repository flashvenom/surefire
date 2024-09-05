using Mantis.Domain.Renewals.Models;

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
    }
}
