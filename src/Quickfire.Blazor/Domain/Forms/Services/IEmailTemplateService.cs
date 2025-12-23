using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Forms.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quickfire.Blazor.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<List<EmailTemplate>> GetAllTemplatesAsync();
        Task<EmailTemplate> GetTemplateByIdAsync(int id);
        Task<EmailTemplate> CreateTemplateAsync(EmailTemplate template);
        Task<EmailTemplate> UpdateTemplateAsync(EmailTemplate template);
        Task DeleteTemplateAsync(int id);
        Task<List<EmailTemplate>> GetActiveTemplatesAsync();
    }
}