using Surefire.Data;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Surefire.Domain.Clients.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ClientService(ApplicationDbContext context, StateService stateService, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = context;
            _stateService = stateService;
            _contextFactory = contextFactory;
        }


        // Primary Shared Methods
        public IQueryable<Client> GetAllClients()
        {
            return _context.Clients.AsQueryable();
        }
        public async Task<Client> GetClientById(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                .Include(c => c.Locations)
                .Include(c => c.Certificates)
                    .ThenInclude(p => p.CreatedBy)
                .Include(c => c.Contacts)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Carrier)
                .Include(c => c.Attachments)
                    .ThenInclude(c => c.Folder)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Wholesaler)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Product)
                .Include(c => c.FormDocs)
                    .ThenInclude(fd => fd.FormDocRevisions)
                .Include(c => c.FormDocs)
                    .ThenInclude(fd => fd.FormPdf)
                .FirstOrDefaultAsync(c => c.ClientId == id);
            return client;
        }


        // Internal Methods
        public async Task UpdateClientAsync(Client client)
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var existingClient = await context.Clients
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.ClientId == client.ClientId);

            if (existingClient != null)
            {
                // Update the basic client fields
                existingClient.Name = client.Name;
                existingClient.LookupCode = client.LookupCode;
                existingClient.PhoneNumber = client.PhoneNumber;
                existingClient.Email = client.Email;
                existingClient.Website = client.Website;
                existingClient.Comments = client.Comments;

                // Update the associated Address
                if (existingClient.Address != null && client.Address != null)
                {
                    existingClient.Address.AddressLine1 = client.Address.AddressLine1;
                    existingClient.Address.AddressLine2 = client.Address.AddressLine2;
                    existingClient.Address.City = client.Address.City;
                    existingClient.Address.State = client.Address.State;
                    existingClient.Address.PostalCode = client.Address.PostalCode;
                }

                // Update any other navigation properties if necessary
                //existingClient.PrimaryContactId = client.PrimaryContactId;

                // Track who made the changes, if necessary
                existingClient.UpdatedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
        }
        


        // Clients
        public async Task<List<ClientListItem>> GetClientListAsync()
        {
            using var context = _contextFactory.CreateDbContext();
                
            var list = await context.Clients
                .OrderByDescending(c => c.DateOpened)
                .Select(c => new ClientListItem
                {
                    ClientId = c.ClientId,
                    Name = c.Name,
                    DateOpened = c.DateOpened
                })
                .ToListAsync();

            return list;
        }
        public async Task<Client> RemoveLogo(int clientId)
        {
            // Retrieve the client by ID
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null || string.IsNullOrEmpty(client.LogoFilename))
            {
                return null; // No client found or no logo to remove
            }

            // Construct the file path for the logo
            string filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/uploads/logos", client.LogoFilename);

            // Check if the file exists, and if it does, delete it
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Set the LogoFilename to null and update the client in the database
            client.LogoFilename = null;
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return client; // Return the updated client
        }
        public async Task<int> CreateNewClientAsync(Client client)
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);
            client.CreatedBy = currentUser;
            context.Clients.Add(client);
            await context.SaveChangesAsync();
            return client.ClientId;
        }
        public async Task UpdateLastOpenedAsync(int clientId, DateTime lastOpened)
        {
            using var context = _contextFactory.CreateDbContext();

            var client = await context.Clients.FindAsync(clientId); // Get the client by ID

            if (client != null)
            {
                client.DateOpened = lastOpened; // Set the LastOpened date

                context.Clients.Update(client); // Mark the client as modified
                await context.SaveChangesAsync(); // Save changes to the database
            }
            else
            {
                throw new ArgumentException("Client not found for the given clientId.");
            }
        }
        public async Task AddContactsToClientAsync(int clientId, List<Contact> contacts)
        {
            var client = await _context.Clients
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client != null)
            {
                foreach (var contact in contacts)
                {
                    client.Contacts.Add(contact);
                }

                await _context.SaveChangesAsync(); // Persist changes
            }
            else
            {
                throw new Exception("Client not found.");
            }
        }
        public async Task<List<Contact>> GetContactsByClientIdAsync(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Retrieve the contacts for the specified client
            var contacts = await context.Contacts
                .Where(c => c.ClientId == clientId)
                .ToListAsync();

            return contacts;
        }
        public async Task<List<Contact>> GetContactsByRenewalIdAsync(int renewalId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Get the ClientId from the renewal
            var clientId = await context.Renewals
                .Where(r => r.RenewalId == renewalId)
                .Select(r => r.ClientId)
                .FirstOrDefaultAsync();

            // If no ClientId is found, return an empty list
            if (clientId == 0)
            {
                return new List<Contact>();
            }

            // Retrieve the contacts for the specified client
            var contacts = await context.Contacts
                .Where(c => c.ClientId == clientId)
                .ToListAsync();

            return contacts;
        }



        // Business Details
        public async Task<BusinessDetails> GetBusinessDetailsByClientId(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();
            var businessDetails = await context.BusinessDetails
                .FirstOrDefaultAsync(bd => bd.ClientId == clientId);
            return businessDetails;
        }
        public async Task AddBusinessDetailsAsync(BusinessDetails businessDetails)
        {
            using var context = _contextFactory.CreateDbContext();
            context.BusinessDetails.Add(businessDetails);
            await context.SaveChangesAsync();
        }

        public async Task UpdateBusinessDetailsAsync(BusinessDetails businessDetails)
        {
            using var context = _contextFactory.CreateDbContext();
            var existingEntity = await context.BusinessDetails.FirstOrDefaultAsync(bd => bd.BusinessDetailsId == businessDetails.BusinessDetailsId);

            if (existingEntity != null)
            {
                context.Entry(existingEntity).CurrentValues.SetValues(businessDetails);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Entity not found.");
            }
        }



        //Leads
        public async Task<Lead> GetLeadByIdAsync(int id)
        {
            // Retrieve the lead and related data
            var lead = await _context.Leads
                .Include(l => l.FormDocs)
                    .ThenInclude(fd => fd.FormDocRevisions)
                .Include(l => l.FormDocs)
                    .ThenInclude(fd => fd.FormPdf)
                .Include(l => l.LeadNotes)
                .Include(r => r.Submissions)
                    .ThenInclude(s => s.Carrier)
                        .ThenInclude(c => c.Contacts)
                .Include(r => r.Submissions)
                    .ThenInclude(s => s.Wholesaler)
                        .ThenInclude(w => w.Contacts)
                .Include(r => r.Submissions)
                    .ThenInclude(s => s.SubmissionNotes)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.LeadId == id);

            // If the lead is found, update the LastOpened field
            if (lead != null)
            {
                lead.LastOpened = DateTime.UtcNow;  // Update LastOpened to the current UTC date and time
                await _context.SaveChangesAsync();  // Save the change to the database
            }

            return lead;
        }
        public async Task AddLeadAsync(Lead lead)
        {
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateLeadAsync(Lead lead)
        {
            var existingLead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == lead.LeadId);

            // update all the fields for leads
            if (existingLead != null)
            {
                existingLead.CompanyName = lead.CompanyName;
                existingLead.ContactName = lead.ContactName;
                existingLead.PhoneNumber = lead.PhoneNumber;
                existingLead.Email = lead.Email;
                existingLead.Website = lead.Website;
                existingLead.Operations = lead.Operations;
                existingLead.Notes = lead.Notes;
                existingLead.BindDate = lead.BindDate;
                existingLead.LastOpened = DateTime.Now;
                existingLead.Stage = lead.Stage;

                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateLeadSimpleAsync(Lead lead)
        {
            _context.Entry(lead).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public IQueryable<Lead> GetAllLeads()
        {
            return _context.Leads.AsQueryable();
        }
        public async Task<List<Lead>> GetAllLeadsAsync()
        {
            var theleads = await _context.Leads
                .Include(l => l.CreatedBy)
                .Include(l => l.Submissions)
                .Include(l => l.LeadNotes)
                .Include(l => l.Product)
                .OrderBy(x => x.Stage).ToListAsync();
            return theleads;
        }
        public async Task AddLeadNoteAsync(LeadNote newNote)
        {
            _context.LeadNotes.Add(newNote);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateNotesAndPremiumAsync(Submission submission)
        {
            await _context.SaveChangesAsync();
        }


        // Forms
        public async Task<Client> GetClientByCertificateId(int certificateId)
        {
            //Is this an old function we can remove?
            var client = await _context.Certificates
                .Where(cert => cert.CertificateId == certificateId)
                .Include(cert => cert.Client)
                    .ThenInclude(client => client.Address)
                .Include(cert => cert.Client)
                    .ThenInclude(client => client.PrimaryContact)
                .Include(cert => cert.Client)
                    .ThenInclude(client => client.Locations)
                .Include(cert => cert.Client)
                    .ThenInclude(client => client.Contacts)
                .Select(cert => cert.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return client;
        }
    }

}
