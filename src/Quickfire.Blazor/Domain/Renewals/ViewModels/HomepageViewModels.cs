using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models;

namespace Quickfire.Blazor.Domain.Renewals.ViewModels
{
    public class HomePageTasksViewModel
    {
        public int RenewalId { get; set; }
        public bool Highlighted { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string TaskNote { get; set; } = string.Empty;
        public DateTime? GoalDate { get; set; }
        public string PolicyProduct { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public DateTime RenewalDate { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    public class HomePageRenFlowTasksViewModel
    {
        public int TrackTaskId { get; set; }
        public DateTime? DailyCheckOff { get; set; }
        public int RenewalId { get; set; }
        public bool Highlighted { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? ParentTaskName { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string TaskNote { get; set; } = string.Empty;
        public DateTime? GoalDate { get; set; }
        public string PolicyProduct { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public DateTime RenewalDate { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    public class HomePageViewModel
    {
        public List<HomePageTasksViewModel> UpcomingTasks { get; set; } = new();
        public List<HomePageTasksViewModel> HighlightedTasks { get; set; } = new();
        public List<HomePageTasksViewModel> IncompleteTasks { get; set; } = new();
    }

}
