using Mantis.Data;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Mantis.Domain.Contacts.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new contact
        public async Task CreateContactAsync(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        // Get a contact by its ID
        public async Task<Contact?> GetContactByIdAsync(int contactId)
        {
            return await _context.Contacts
                .Include(c => c.Client)   // Include related Client entity if needed
                .Include(c => c.Carrier)  // Include related Carrier entity if needed
                .FirstOrDefaultAsync(c => c.ContactId == contactId);
        }

        // Get a clients associated to a contact by contact ID
        public async Task<List<Client>> GetContactRelatedClients(int contactId)
        {
            // Step 1: Find the contact with the given contactId
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return new List<Client>(); // Return empty list if contact not found
            }

            // Step 2: Find clients where this contact is the primary contact
            var clients = await _context.Clients
                .Where(client => client.PrimaryContact.ContactId == contactId)
                .ToListAsync();

            // Step 3: Find other contacts with the same FirstName and LastName
            var duplicateContacts = await _context.Contacts
                .Where(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName && c.ContactId != contactId)
                .ToListAsync();

            // Step 4: For each duplicate contact, find clients where these contacts are the primary contact
            foreach (var duplicateContact in duplicateContacts)
            {
                var associatedClients = await _context.Clients
                    .Where(client => client.PrimaryContact.ContactId == duplicateContact.ContactId)
                    .ToListAsync();

                clients.AddRange(associatedClients);
            }

            // Step 5: Remove duplicate clients from the list (if any)
            clients = clients.Distinct().ToList();

            return clients;
        }

        // Update an existing contact
        public async Task UpdateContactAsync(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Delete a contact by its ID
        public async Task DeleteContactAsync(int contactId)
        {
            var contact = await GetContactByIdAsync(contactId);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }

        public List<ContactDto> GetContactsList()
        {
            return _context.Contacts
                .Include(c => c.Client)
                .Include(c => c.Carrier)
                .Select(c => new ContactDto
                {
                    ContactId = c.ContactId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Title = c.Title,
                    Email = c.Email,
                    Phone = c.Phone,
                    Fax = c.Fax,
                    Mobile = c.Mobile,
                    Notes = c.Notes,
                    DateCreated = c.DateCreated,
                    AssociatedWith = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : "N/A"
                })
                .ToList();
        }

    }
}
