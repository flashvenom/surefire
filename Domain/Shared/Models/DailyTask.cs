using Mantis.Data;

namespace Mantis.Domain.Shared
{
    public class DailyTask
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public bool Completed { get; set; } = false;
        public bool Highlighted { get; set; } = false;
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
    }
}
