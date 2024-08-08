namespace Mantis.Domain.Renewals.ViewModels
{
    public class TasksHomeViewModel
    {
        public ICollection<RenewalViewModel> MarketingEntries { get; set; }
        public List<ImportantTaskViewModel> ImportantTasks { get; set; }
        public List<ImportantTaskViewModel> PastDueTasks { get; set; }
        public List<ImportantTaskViewModel> UpcomingTasks { get; set; }
        public List<ImportantTaskViewModel> HighlightedTasks { get; set; }
    }

    public class ImportantTaskViewModel
    {
        public string InsuredName { get; set; }
        public int InsuredId { get; set; }
        public string TaskName { get; set; }
        public DateTime? GoalDate { get; set; }
        public string Note { get; set; }
        public string Type { get; set; }
        public string PolicyType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DaysLeft { get; set; }
        public string CsrAssigned { get; set; }
        public bool Highlighted { get; set; }
    }
    //public class TaskItemViewModel
    //{
    //    public int Id { get; set; }
    //    public string TaskName { get; set; }
    //    public string Note { get; set; }
    //    public DateTime? GoalDate { get; set; }
    //    public bool Completed { get; set; }
    //    public bool Highlighted { get; set; }
    //}
}
