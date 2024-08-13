using Mantis.Data;
using Mantis.Domain.Renewals.Models;

namespace Mantis.Domain.Renewals.ViewModels
{
    public class HomePageTasksViewModel
    {
        public int RenewalId { get; set; }
        public bool Highlighted { get; set; }
        public string TaskName { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public string TaskNote { get; set; }
        public DateTime? GoalDate { get; set; }
        public string PolicyProduct { get; set; }
        public DateTime RenewalDate { get; set; }
        public string Priority { get; set; }
    }

    public class HomePageViewModel
    {
        public List<HomePageTasksViewModel> UpcomingTasks { get; set; }
        public List<HomePageTasksViewModel> HighlightedTasks { get; set; }
        public List<HomePageTasksViewModel> IncompleteTasks { get; set; }
    }

}
