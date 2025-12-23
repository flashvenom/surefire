using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Quickfire.Blazor.Domain.Forms.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public EmailTemplateService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return await context.EmailTemplates
                    .AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading email templates: {ex.Message}");
                return new List<EmailTemplate>();
            }
        }

        public async Task<EmailTemplate> GetTemplateByIdAsync(int id)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var template = await context.EmailTemplates.FindAsync(id);
            if (template == null)
            {
                throw new KeyNotFoundException($"Email template with ID {id} not found.");
            }
            return template;
        }

        public async Task<EmailTemplate> CreateTemplateAsync(EmailTemplate template)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            template.CreatedAt = DateTime.UtcNow;
            
            await context.EmailTemplates.AddAsync(template);
            await context.SaveChangesAsync();
            
            return template;
        }

        public async Task<EmailTemplate> UpdateTemplateAsync(EmailTemplate template)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var existingTemplate = await context.EmailTemplates.FindAsync(template.EmailTemplateId);
            
            if (existingTemplate == null)
            {
                throw new KeyNotFoundException($"Email template with ID {template.EmailTemplateId} not found.");
            }
            
            // Update properties
            existingTemplate.Name = template.Name;
            existingTemplate.Subject = template.Subject;
            existingTemplate.Body = template.Body;
            existingTemplate.Description = template.Description;
            existingTemplate.IsActive = template.IsActive;
            existingTemplate.UpdatedAt = DateTime.UtcNow;
            // Add missing properties
            existingTemplate.NeedsContact = template.NeedsContact;
            existingTemplate.NeedsPolicy = template.NeedsPolicy;
            existingTemplate.NeedsProduct = template.NeedsProduct;
            existingTemplate.NeedsPayment = template.NeedsPayment;
            existingTemplate.NeedsDownPayment = template.NeedsDownPayment;
            existingTemplate.CustomFunction = template.CustomFunction;

            context.EmailTemplates.Update(existingTemplate);
            await context.SaveChangesAsync();

            return existingTemplate;
        }

        public async Task DeleteTemplateAsync(int id)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var template = await context.EmailTemplates.FindAsync(id);
            
            if (template == null)
            {
                throw new KeyNotFoundException($"Email template with ID {id} not found.");
            }
            
            context.EmailTemplates.Remove(template);
            await context.SaveChangesAsync();
        }

        public async Task<List<EmailTemplate>> GetActiveTemplatesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return await context.EmailTemplates
                    .AsNoTracking()
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading active email templates: {ex.Message}");
                return new List<EmailTemplate>();
            }
        }
    }
} 