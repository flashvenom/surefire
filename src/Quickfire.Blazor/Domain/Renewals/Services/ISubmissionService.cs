using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Renewals.Models;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public interface ISubmissionService
    {
        Task<Submission> GetSubmissionByIdAsync(int submissionId);
        Task<Submission> CreateNewSubmissionAsync(int parentId, string type, int? carrierId = null, int? wholesalerId = null);
        Task UpdateSubmissionAsync(Submission submission);
        Task UpdateSubmissionPrimaryContactAsync(int submissionId, int primaryContactId);
        Task<Carrier> UpdateSubmissionCarrierAsync(int submissionId, int carrierId);
        Task<Carrier> UpdateSubmissionWholesalerAsync(int submissionId, int wholesalerId);
        Task RemoveSubmissionCarrierAsync(int submissionId);
        Task RemoveSubmissionWholesalerAsync(int submissionId);
        Task UpdateSubmissionPremiumAsync(int submissionId, int premium);
        Task DeleteSubmissionAsync(int submissionId);
        
        // Task Management
        Task<List<SubmissionTask>> GetSubmissionTasksAsync(int submissionId);
        Task<SubmissionTask> CreateSubmissionTaskAsync(int submissionId, string taskName, string description, DateTime? dueDate);
        Task<SubmissionTask> UpdateSubmissionTaskAsync(int taskId, string taskName, string description, DateTime? dueDate);
        Task ToggleSubmissionTaskCompletionAsync(int taskId, bool isCompleted);
        Task DeleteSubmissionTaskAsync(int taskId);
        
        // Attachment Management
        Task<bool> DeleteAttachmentAsync(int attachmentId);
    }
}
