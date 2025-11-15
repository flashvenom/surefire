using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Renewals.Models;

namespace Surefire.Domain.Shared.Services
{
    public interface ISubmissionService
    {
        Task<Submission> GetSubmissionByIdAsync(int submissionId);
        Task<Submission> CreateNewSubmissionAsync(int parentId, string type, int? carrierId = null, int? wholesalerId = null);
        Task UpdateSubmissionAsync(Submission submission);
        Task UpdateSubmissionPrimaryContactAsync(int submissionId, int primaryContactId);
        Task<Carrier> UpdateSubmissionCarrierAsync(int submissionId, int carrierId);
        Task<Carrier> UpdateSubmissionWholesalerAsync(int submissionId, int wholesalerId);
        Task UpdateSubmissionPremiumAsync(int submissionId, int premium);
        Task DeleteSubmissionAsync(int submissionId);
    }
}
