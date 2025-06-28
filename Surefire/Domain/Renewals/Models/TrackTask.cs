using Surefire.Data;
using Surefire.Domain.Shared;
using Surefire.Domain.Renewals.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Renewals.Models
{
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
        public DateTime? DailyCheckOff { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? GoalDate { get; set; }
        public Renewal Renewal { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
        public string? AssignedToId { get; set; }
        public int? ParentTaskId { get; set; }
        public TrackTask? ParentTask { get; set; }
        public int RenewalId { get; set; }
        public ICollection<TrackTask> Subtasks { get; set; } = new List<TrackTask>();
    }

    /// <summary>
    /// These are the tasks that are used to build different kinds of checklists for employees to guide them
    /// through the procvess of completing a renewal.
    /// </summary>
    public class TaskMaster
    {
        public int TaskMasterId { get; set; }
        public bool Important { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? DaysBeforeExpiration { get; set; }
        public string? ForType { get; set; }

        public ICollection<TaskGroupTaskMaster> TaskGroupTaskMasters { get; set; } = new List<TaskGroupTaskMaster>();

        public string? DefaultAssignedToId { get; set; }
        public ApplicationUser? DefaultAssignedTo { get; set; }

        // MasterSubTasks
        public ICollection<MasterSubTask> MasterSubTasks { get; set; } = new List<MasterSubTask>();

        // Navigation property for subtask assignment (many-to-many)
        public ICollection<TaskMasterSubTask> SubTaskLinks { get; set; } = new List<TaskMasterSubTask>(); // Where this task is the parent
        public ICollection<TaskMasterSubTask> ParentLinks { get; set; } = new List<TaskMasterSubTask>(); // Where this task is the subtask
    }
    /// <summary>
    /// This creates groups of MasterTasks to be used as a "task template" when creating a renewal.
    /// </summary>
    public class TaskGroup
    {
        public int TaskGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation property for the join entity
        public ICollection<TaskGroupTaskMaster> TaskGroupTaskMasters { get; set; } = new List<TaskGroupTaskMaster>();
    }

    /// <summary>
    /// Junction table to support many-to-many relationship between TaskMaster and TaskGroup
    /// This allows a TaskMaster to belong to multiple TaskGroups with different OrderNumbers in each
    /// </summary>
    public class TaskGroupTaskMaster
    {
        public int TaskGroupId { get; set; }
        public int TaskMasterId { get; set; }

        // Navigation Properties to the related entities
        public TaskGroup TaskGroup { get; set; } = null!;
        public TaskMaster TaskMaster { get; set; } = null!;

        // How the item is ordered in the task group
        public int OrderNumber { get; set; }
    }

    public class MasterSubTask
    {
        public int MasterSubTaskId { get; set; }
        public int TaskMasterId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OrderNumber { get; set; }

        // Navigation property
        public TaskMaster TaskMaster { get; set; } = null!;
    }

    public class TaskMasterSubTask
    {
        public int ParentTaskMasterId { get; set; }
        public TaskMaster ParentTaskMaster { get; set; } = null!;

        public int SubTaskMasterId { get; set; }
        public TaskMaster SubTaskMaster { get; set; } = null!;

        public int OrderNumber { get; set; } // Optional, for ordering subtasks
    }
}
