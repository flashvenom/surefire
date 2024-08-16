using Mantis.Data;
using Mantis.Domain.Shared;
using Mantis.Domain.Renewals.Models;

namespace Mantis.Domain.Renewals.Models
{
    public class TaskMaster
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public bool Important { get; set; }
        public int? ParentTaskId { get; set; }
        public string TaskName { get; set; }
        public string? Description { get; set; }
        public int? DaysBeforeExpiration { get; set; }
        public string? ForType { get; set; }
    }

    public class TrackTask
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; }
        public bool Completed { get; set; } = false;
        public bool Hidden { get; set; } = false;
        public bool Highlighted { get; set; } = false;
        public string? Notes { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        public Renewal Renewal { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? GoalDate { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
    }
}
