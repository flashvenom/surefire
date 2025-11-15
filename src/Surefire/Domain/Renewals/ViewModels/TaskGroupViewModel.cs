using Surefire.Data;
using Surefire.Domain.Renewals.Models;

namespace Surefire.Domain.Renewals.ViewModels
{
    /// <summary>
    /// View model for TaskMaster dialog
    /// </summary>
    public class TaskMasterDialogViewModel
    {
        public int TaskMasterId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? DaysBeforeExpiration { get; set; }
        public string? ForType { get; set; }
        public bool Important { get; set; }

        // Default assigned staff member
        public string? DefaultAssignedToId { get; set; }

        // MasterSubTasks
        public List<MasterSubTaskViewModel> MasterSubTasks { get; set; } = new List<MasterSubTaskViewModel>();
    }

    public class MasterSubTaskViewModel
    {
        public int MasterSubTaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OrderNumber { get; set; }
    }

    /// <summary>
    /// View model for TaskGroup dialog
    /// </summary>
    public class TaskGroupDialogViewModel
    {
        public int TaskGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
