using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Quickfire.Blazor.Domain.Clients.Services
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

        // Global Notes Methods
        public async Task<List<GlobalNote>> GetGlobalNotesByEntityAsync(EntityType entityType, int entityId)
        {
            using var context = _contextFactory.CreateDbContext();

            var notes = await context.GlobalNotes
                .Where(n => n.EntityType == entityType && n.EntityId == entityId && !n.Deleted)
                .OrderByDescending(n => n.Pinned)
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notes;
        }

        public async Task<GlobalNote> AddGlobalNoteAsync(GlobalNote note)
        {
            using var context = _contextFactory.CreateDbContext();

            var currentUser = _stateService.CurrentUser;
            note.AuthorId = currentUser.Id;
            note.AuthorName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();

            context.GlobalNotes.Add(note);
            await context.SaveChangesAsync();

            return note;
        }

        public async Task UpdateGlobalNoteAsync(GlobalNote note)
        {
            using var context = _contextFactory.CreateDbContext();

            var existingNote = await context.GlobalNotes
                .FirstOrDefaultAsync(n => n.Id == note.Id);

            if (existingNote != null)
            {
                existingNote.Text = note.Text;
                existingNote.Tags = note.Tags;
                existingNote.Pinned = note.Pinned;
                existingNote.ReminderDate = note.ReminderDate;
                existingNote.HasMarkdown = note.HasMarkdown;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteGlobalNoteAsync(int noteId)
        {
            using var context = _contextFactory.CreateDbContext();

            var note = await context.GlobalNotes.FindAsync(noteId);

            if (note != null)
            {
                note.Deleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task TogglePinGlobalNoteAsync(int noteId)
        {
            using var context = _contextFactory.CreateDbContext();

            var note = await context.GlobalNotes.FindAsync(noteId);

            if (note != null)
            {
                note.Pinned = !note.Pinned;
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<GlobalNote>> SearchGlobalNotesAsync(string searchText, EntityType? entityType = null)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.GlobalNotes.Where(n => !n.Deleted);

            if (entityType.HasValue)
            {
                query = query.Where(n => n.EntityType == entityType.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(n =>
                    n.Text.Contains(searchText) ||
                    n.Tags.Contains(searchText) ||
                    n.AuthorName.Contains(searchText));
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }


        // Primary Shared Methods
        public IQueryable<Client> GetAllClients()
        {
            return _context.Clients
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.PhoneNumbers)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.EmailAddresses)
                .AsQueryable();
        }
        public async Task<Client> GetClientById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var client = await context.Clients
                .Include(c => c.Contacts)
                    .ThenInclude(c => c.EmailAddresses)
                .Include(c => c.Contacts)
                    .ThenInclude(c => c.PhoneNumbers)
                .Include(c => c.Certificates)
                    .ThenInclude(cert => cert.CreatedBy)
                .Include(c => c.Certificates)
                    .ThenInclude(cert => cert.ModifiedBy)
                .Include(c => c.Policies.OrderByDescending(p => p.EffectiveDate))
                    .ThenInclude(p => p.Carrier)
                .Include(c => c.Attachments)
                    .ThenInclude(c => c.Folder)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Wholesaler)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Product)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Renewals)
                .Include(c => c.FormDocs)
                    .ThenInclude(fd => fd.FormDocRevisions)
                .Include(c => c.FormDocs)
                    .ThenInclude(fd => fd.FormPdf)
                .Include(c => c.FormDocs)
                    .ThenInclude(fd => fd.FormsLibraryVersion)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            return client;
        }

        public async Task<Client> GetClientDetailsById(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();
            var client = await context.Clients
                .AsNoTracking()
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Carrier)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Wholesaler)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Product)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Vehicles)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Drivers)
                 .Include(c => c.Policies)
                    .ThenInclude(p => p.WorkCompCoverage)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.RatingBases)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.PhoneNumbers)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.EmailAddresses)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.Address)
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                    .ThenInclude(pc => pc.PhoneNumbers)
                .Include(c => c.PrimaryContact)
                    .ThenInclude(pc => pc.EmailAddresses)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);
            return client;
        }

        // Lookup client by name with smart matching
        public async Task<Client?> GetClientByNameAsync(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName)) return null;
            using var context = _contextFactory.CreateDbContext();
            var allClients = await context.Clients.ToListAsync();

            // 1. Try exact match (case-insensitive)
            var matches = allClients.Where(c => c.Name.Equals(clientName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 1) return matches.First();
            if (matches.Count > 1) return matches.OrderBy(c => c.Name.Length).First();

            // 2. Try first word match
            var firstWord = clientName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(firstWord))
            {
                matches = allClients.Where(c => c.Name.StartsWith(firstWord, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matches.Count == 1) return matches.First();
                if (matches.Count > 1)
                {
                    // Try two-word match
                    var words = clientName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 1)
                    {
                        var firstTwo = string.Join(' ', words.Take(2));
                        var twoWordMatches = matches.Where(c => c.Name.StartsWith(firstTwo, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (twoWordMatches.Count == 1) return twoWordMatches.First();
                        if (twoWordMatches.Count > 1) return twoWordMatches.OrderBy(c => c.Name.Length).First();
                    }
                    // Otherwise, return shortest match
                    return matches.OrderBy(c => c.Name.Length).First();
                }
            }

            // 3. Try normalized (remove special chars, case-insensitive)
            string Normalize(string s) => new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLower();
            var normalizedInput = Normalize(clientName);
            matches = allClients.Where(c => Normalize(c.Name) == normalizedInput).ToList();
            if (matches.Count == 1) return matches.First();
            if (matches.Count > 1) return matches.OrderBy(c => c.Name.Length).First();

            // 4. Try contains (case-insensitive, fallback)
            matches = allClients.Where(c => c.Name.IndexOf(clientName, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (matches.Count == 1) return matches.First();
            if (matches.Count > 1) return matches.OrderBy(c => c.Name.Length).First();

            // 5. No match found
            return null;
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

                // Only update logo if a new value is provided
                if (!string.IsNullOrEmpty(client.LogoFilename))
                {
                    existingClient.LogoFilename = client.LogoFilename;
                }
                existingClient.UpdatedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
                await _stateService.InvalidateClientsCacheAsync();
                await _stateService.LoadClient(client.ClientId);
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

            if (currentUser != null)
            {
                context.Attach(currentUser);
                client.CreatedBy = currentUser;
            }

            var hadAnyClients = await context.Clients.AnyAsync();
            var shouldAssignLastLookupClient = !hadAnyClients &&
                currentUser != null &&
                string.IsNullOrWhiteSpace(currentUser.LastLookupClient);

            // Add the client first
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            // If we have a primary contact, set the ClientId and then update the PrimaryContactId
            if (client.PrimaryContact != null)
            {
                // Set the ClientId for the contact
                client.PrimaryContact.ClientId = client.ClientId;

                // Save the contact changes
                await context.SaveChangesAsync();

                // Now update the client's PrimaryContactId
                client.PrimaryContactId = client.PrimaryContact.ContactId;
                await context.SaveChangesAsync();
            }

            if (shouldAssignLastLookupClient && currentUser != null)
            {
                currentUser.LastLookupClient = client.ClientId.ToString();
                await context.SaveChangesAsync();
            }

            // Invalidate the clients cache since we added a new client
            await _stateService.InvalidateClientsCacheAsync();

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
                
                // Invalidate the clients cache since DateOpened changed (affects sort order)
                await _stateService.InvalidateClientsCacheAsync();
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

                await _context.SaveChangesAsync();
                
                // Notify state service of the update
                await _stateService.LoadClient(clientId);
            }
            else
            {
                throw new Exception("Client not found.");
            }
        }
        public async Task<List<Contact>> GetContactsByClientIdAsync(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Retrieve the contacts for the specified client with navigation properties
            var contacts = await context.Contacts
                .Where(c => c.ClientId == clientId)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .Include(c => c.PrimaryEmail)
                .Include(c => c.PrimaryPhone)
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

            // Retrieve the contacts for the specified client with navigation properties
            var contacts = await context.Contacts
                .Where(c => c.ClientId == clientId)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .Include(c => c.PrimaryEmail)
                .Include(c => c.PrimaryPhone)
                .ToListAsync();

            return contacts;
        }



        // Business Details
        public async Task<BusinessDetails> GetBusinessDetailsByClientId(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();
            var businessDetails = await context.BusinessDetails.FirstOrDefaultAsync(bd => bd.ClientId == clientId);
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
            
            Console.WriteLine($"[UpdateBusinessDetailsAsync] Looking for BusinessDetailsId: {businessDetails.BusinessDetailsId}, ClientId: {businessDetails.ClientId}");
            
            var existingEntity = await context.BusinessDetails.FirstOrDefaultAsync(bd => bd.BusinessDetailsId == businessDetails.BusinessDetailsId);

            if (existingEntity != null)
            {
                Console.WriteLine($"[UpdateBusinessDetailsAsync] Found existing entity. Setting values...");
                context.Entry(existingEntity).CurrentValues.SetValues(businessDetails);
                
                // Force EF to detect changes
                context.Entry(existingEntity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                Console.WriteLine($"[UpdateBusinessDetailsAsync] Calling SaveChangesAsync...");
                var changeCount = await context.SaveChangesAsync();
                Console.WriteLine($"[UpdateBusinessDetailsAsync] SaveChangesAsync completed. Changes saved: {changeCount}");
            }
            else
            {
                Console.WriteLine($"[UpdateBusinessDetailsAsync] ERROR: Entity not found with BusinessDetailsId: {businessDetails.BusinessDetailsId}");
                throw new InvalidOperationException($"BusinessDetails entity not found with ID: {businessDetails.BusinessDetailsId}");
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
                .Include(l => l.FormDocs)
                    .ThenInclude(fd => fd.FormsLibraryVersion)
                .Include(l => l.LeadNotes)
                .Include(l => l.CreatedBy)
                .Include(r => r.Submissions)
                    .ThenInclude(s => s.Carrier)
                        .ThenInclude(c => c.Contacts)
                .Include(r => r.Submissions)
                    .ThenInclude(s => s.Wholesaler)
                        .ThenInclude(w => w.Contacts)
                .Include(r => r.Submissions)
                    
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
        public async Task<List<LeadNote>> GetLeadNotesAsync(int leadId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.LeadNotes
                .Where(n => n.LeadId == leadId && !n.Deleted)
                .OrderByDescending(n => n.DateCreated)
                .AsNoTracking()
                .ToListAsync();
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

        public async Task<PhoneNumber> AddPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            using var context = _contextFactory.CreateDbContext();
            context.PhoneNumbers.Add(phoneNumber);
            await context.SaveChangesAsync();
            return phoneNumber;
        }

        public async Task<EmailAddress> AddEmailAddressAsync(EmailAddress emailAddress)
        {
            using var context = _contextFactory.CreateDbContext();
            context.EmailAddresses.Add(emailAddress);
            await context.SaveChangesAsync();
            return emailAddress;
        }
    }

}
