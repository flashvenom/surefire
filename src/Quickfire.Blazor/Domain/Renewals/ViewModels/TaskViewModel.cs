using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models;

namespace Quickfire.Blazor.Domain.Renewals.ViewModels
{
    public class TrackTaskEditViewModel
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public bool Hidden { get; set; }
        public bool Highlighted { get; set; }
        public string? Notes { get; set; }
        public string? UserName { get; set; }
        public DateTime? GoalDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
        public Renewal Renewal { get; set; } = default!;
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

    public class TasksHomeViewModel
    {
        public ICollection<RenewalViewModel> MarketingEntries { get; set; } = new List<RenewalViewModel>();
        public List<ImportantTaskViewModel> ImportantTasks { get; set; } = new();
        public List<ImportantTaskViewModel> PastDueTasks { get; set; } = new();
        public List<ImportantTaskViewModel> UpcomingTasks { get; set; } = new();
        public List<ImportantTaskViewModel> HighlightedTasks { get; set; } = new();
    }

    public class ImportantTaskViewModel
    {
        public string InsuredName { get; set; } = string.Empty;
        public int InsuredId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime? GoalDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string PolicyType { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string DaysLeft { get; set; } = string.Empty;
        public string CsrAssigned { get; set; } = string.Empty;
        public bool Highlighted { get; set; }
    }
    //public class TaskItemViewModel
    //{
    //    public int Id { get; set; }
    //    public string TaskName { get; set; } = string.Empty;
    //    public string Note { get; set; } = string.Empty;
    //    public DateTime? GoalDate { get; set; }
    //    public bool Completed { get; set; }
    //    public bool Highlighted { get; set; }
    //}
}
