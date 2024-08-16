using Mantis.Data;
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

    }
}
