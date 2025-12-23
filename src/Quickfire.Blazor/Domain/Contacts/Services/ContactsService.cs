using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Shared.Services;

namespace Quickfire.Blazor.Domain.Contacts.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public ContactService(ApplicationDbContext context, StateService stateService, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = context;
            _stateService = stateService;
            _contextFactory = contextFactory;
        }


        // GET
        public IQueryable<Contact> GetAllContacts()
        {
            return _context.Contacts
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .AsQueryable();
        }
        public async Task<Contact?> GetContactByIdAsync(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .Include(c => c.Client)  
                .Include(c => c.Carrier) 
                .Include(c => c.PhoneNumbers) 
                .Include(c => c.EmailAddresses) 
                .AsNoTracking()  
                .FirstOrDefaultAsync(c => c.ContactId == contactId);
        }
        public async Task<List<Client>> GetContactRelatedClients(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            // Step 1: Find the contact with the given contactId
            var contact = await context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return new List<Client>(); // Return empty list if contact not found
            }

            // Step 2: Find clients where this contact is the primary contact
            var clients = await context.Clients
                .Where(client => client.PrimaryContact.ContactId == contactId)
                .ToListAsync();

            // Step 3: Find other contacts with the same FirstName and LastName
            var duplicateContacts = await context.Contacts
                .Where(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName && c.ContactId != contactId)
                .ToListAsync();

            // Step 4: For each duplicate contact, find clients where these contacts are the primary contact
            foreach (var duplicateContact in duplicateContacts)
            {
                var associatedClients = await context.Clients
                    .Where(client => client.PrimaryContact.ContactId == duplicateContact.ContactId)
                    .ToListAsync();

                clients.AddRange(associatedClients);
            }

            // Step 5: Remove duplicate clients from the list (if any)
            clients = clients.Distinct().ToList();

            return clients;
        }
       

        // CREATE
        public async Task CreateContactAsync(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Contacts.Add(contact);
            await context.SaveChangesAsync();
        }
        public async Task<Contact> QuickAddContactToCarrierAsync(ContactDto contactDto, int carrierid)
        {
            using var context = _contextFactory.CreateDbContext();
            var contact = new Contact
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Title = contactDto.Title,
                Notes = contactDto.Notes,
                DateCreated = DateTime.UtcNow,
                CarrierId = carrierid
            };

            // Add phone numbers if provided
            if (!string.IsNullOrEmpty(contactDto.Phone))
            {
                contact.PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = contactDto.Phone,
                        Type = PhoneType.Office,
                        IsPrimary = true,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };
            }

            if (!string.IsNullOrEmpty(contactDto.Mobile))
            {
                contact.PhoneNumbers ??= new List<PhoneNumber>();
                contact.PhoneNumbers.Add(new PhoneNumber
                {
                    Number = contactDto.Mobile,
                    Type = PhoneType.Mobile,
                    IsPrimary = false,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                });
            }

            if (!string.IsNullOrEmpty(contactDto.Fax))
            {
                contact.PhoneNumbers ??= new List<PhoneNumber>();
                contact.PhoneNumbers.Add(new PhoneNumber
                {
                    Number = contactDto.Fax,
                    Type = PhoneType.Fax,
                    IsPrimary = false,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                });
            }

            // Add email addresses if provided
            if (!string.IsNullOrEmpty(contactDto.Email))
            {
                contact.EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Email = contactDto.Email,
                        IsPrimary = true,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };
            }

            context.Contacts.Add(contact);
            await context.SaveChangesAsync();

            return contact;
        }
        public async Task<PhoneNumber> AddPhoneNumberAsync(PhoneNumber phoneNumber)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // Set DateCreated and DateModified
                phoneNumber.DateCreated = DateTime.UtcNow;
                phoneNumber.DateModified = DateTime.UtcNow;

                // Add the phone number
                context.PhoneNumbers.Add(phoneNumber);
                await context.SaveChangesAsync();

                // If this is primary, update other phone numbers and the contact
                if (phoneNumber.IsPrimary && phoneNumber.ContactId.HasValue)
                {
                    var contact = await context.Contacts
                        .Include(c => c.PhoneNumbers)
                        .FirstOrDefaultAsync(c => c.ContactId == phoneNumber.ContactId);

                    if (contact != null)
                    {
                        // Set all other phone numbers as non-primary
                        foreach (var phone in contact.PhoneNumbers.Where(p => p.PhoneNumberId != phoneNumber.PhoneNumberId))
                        {
                            phone.IsPrimary = false;
                        }

                        // Update the contact's primary phone ID
                        contact.PrimaryPhoneId = phoneNumber.PhoneNumberId;
                        await context.SaveChangesAsync();
                    }
                }

                return phoneNumber;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding phone number: {ex.Message}");
                throw; // Re-throw to allow caller to handle
            }
        }
        public async Task<EmailAddress> AddEmailAddressAsync(EmailAddress emailAddress)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // Set DateCreated and DateModified
                emailAddress.DateCreated = DateTime.UtcNow;
                emailAddress.DateModified = DateTime.UtcNow;

                // Add the email address
                context.EmailAddresses.Add(emailAddress);
                await context.SaveChangesAsync(); // Save to get the ID assigned

                // If this is primary, update other email addresses and the contact
                if (emailAddress.IsPrimary && emailAddress.ContactId.HasValue)
                {
                    var contact = await context.Contacts
                        .Include(c => c.EmailAddresses)
                        .FirstOrDefaultAsync(c => c.ContactId == emailAddress.ContactId);

                    if (contact != null)
                    {
                        // Set all other email addresses as non-primary
                        foreach (var email in contact.EmailAddresses.Where(e => e.EmailAddressId != emailAddress.EmailAddressId))
                        {
                            email.IsPrimary = false;
                        }

                        // Update the contact's primary email ID
                        contact.PrimaryEmailId = emailAddress.EmailAddressId;
                        await context.SaveChangesAsync();
                    }
                }

                return emailAddress;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding email address: {ex.Message}");
                throw; // Re-throw to allow caller to handle
            }
        }


        // UPDATE
        public async Task UpdateContactAsync(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Entry(contact).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task<PhoneNumber> UpdatePhoneNumberAsync(PhoneNumber phoneNumber)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // First retrieve the entity from the database
                var existingPhoneNumber = await context.PhoneNumbers.FindAsync(phoneNumber.PhoneNumberId);

                if (existingPhoneNumber == null)
                {
                    // If the entity doesn't exist in the database, add it as a new entity
                    phoneNumber.DateCreated = DateTime.UtcNow;
                    phoneNumber.DateModified = DateTime.UtcNow;
                    context.PhoneNumbers.Add(phoneNumber);
                }
                else
                {
                    // Update properties of the existing entity
                    existingPhoneNumber.Number = phoneNumber.Number;
                    existingPhoneNumber.Extension = phoneNumber.Extension;
                    existingPhoneNumber.Type = phoneNumber.Type;
                    existingPhoneNumber.TypeOther = phoneNumber.TypeOther;
                    existingPhoneNumber.SMS = phoneNumber.SMS;
                    existingPhoneNumber.DateModified = DateTime.UtcNow;

                    // Handle primary flag separately to avoid conflicts
                    if (phoneNumber.IsPrimary != existingPhoneNumber.IsPrimary)
                    {
                        existingPhoneNumber.IsPrimary = phoneNumber.IsPrimary;

                        // If this is now a primary phone, update the contact's primary phone ID
                        if (existingPhoneNumber.IsPrimary && existingPhoneNumber.ContactId.HasValue)
                        {
                            var contact = await context.Contacts
                                .Include(c => c.PhoneNumbers)
                                .FirstOrDefaultAsync(c => c.ContactId == existingPhoneNumber.ContactId);

                            if (contact != null)
                            {
                                // Set all other phone numbers as non-primary
                                foreach (var phone in contact.PhoneNumbers.Where(p => p.PhoneNumberId != existingPhoneNumber.PhoneNumberId))
                                {
                                    phone.IsPrimary = false;
                                }

                                // Update the contact's primary phone ID
                                contact.PrimaryPhoneId = existingPhoneNumber.PhoneNumberId;
                            }
                        }
                    }

                    // Use the existing entity for further operations
                    phoneNumber = existingPhoneNumber;
                }

                await context.SaveChangesAsync();
                return phoneNumber;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating phone number: {ex.Message}");
                throw;
            }
        }
        public async Task SetPhoneAsPrimaryAsync(int phoneNumberId, int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            // Skip if this is a temporary ID (not yet saved to DB)
            if (phoneNumberId <= 0)
            {
                return;
            }

            var contact = await context.Contacts
                .Include(c => c.PhoneNumbers)
                .FirstOrDefaultAsync(c => c.ContactId == contactId);

            if (contact != null)
            {
                // Set all other phone numbers as non-primary
                foreach (var phone in contact.PhoneNumbers)
                {
                    phone.IsPrimary = (phone.PhoneNumberId == phoneNumberId);
                }

                // Update the contact's primary phone ID
                contact.PrimaryPhoneId = phoneNumberId;
                await context.SaveChangesAsync();
            }
        }
        public async Task<EmailAddress> UpdateEmailAddressAsync(EmailAddress emailAddress)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // First retrieve the entity from the database
                var existingEmailAddress = await context.EmailAddresses.FindAsync(emailAddress.EmailAddressId);

                if (existingEmailAddress == null)
                {
                    // If the entity doesn't exist in the database, add it as a new entity
                    emailAddress.DateCreated = DateTime.UtcNow;
                    emailAddress.DateModified = DateTime.UtcNow;
                    context.EmailAddresses.Add(emailAddress);
                }
                else
                {
                    // Update properties of the existing entity
                    existingEmailAddress.Email = emailAddress.Email;
                    existingEmailAddress.Label = emailAddress.Label;
                    existingEmailAddress.DateModified = DateTime.UtcNow;

                    // Handle primary flag separately to avoid conflicts
                    if (emailAddress.IsPrimary != existingEmailAddress.IsPrimary)
                    {
                        existingEmailAddress.IsPrimary = emailAddress.IsPrimary;

                        // If this is now a primary email, update the contact's primary email ID
                        if (existingEmailAddress.IsPrimary && existingEmailAddress.ContactId.HasValue)
                        {
                            var contact = await context.Contacts
                                .Include(c => c.EmailAddresses)
                                .FirstOrDefaultAsync(c => c.ContactId == existingEmailAddress.ContactId);

                            if (contact != null)
                            {
                                // Set all other email addresses as non-primary
                                foreach (var email in contact.EmailAddresses.Where(e => e.EmailAddressId != existingEmailAddress.EmailAddressId))
                                {
                                    email.IsPrimary = false;
                                }

                                // Update the contact's primary email ID
                                contact.PrimaryEmailId = existingEmailAddress.EmailAddressId;
                            }
                        }
                    }

                    // Use the existing entity for further operations
                    emailAddress = existingEmailAddress;
                }

                await context.SaveChangesAsync();
                return emailAddress;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating email address: {ex.Message}");
                throw; // Re-throw to allow caller to handle
            }
        }
        public async Task SetEmailAsPrimaryAsync(int emailAddressId, int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            // Skip if this is a temporary ID (not yet saved to DB)
            if (emailAddressId <= 0)
            {
                return;
            }

            var contact = await context.Contacts
                .Include(c => c.EmailAddresses)
                .FirstOrDefaultAsync(c => c.ContactId == contactId);

            if (contact != null)
            {
                // Set all other email addresses as non-primary
                foreach (var email in contact.EmailAddresses)
                {
                    email.IsPrimary = (email.EmailAddressId == emailAddressId);
                }

                // Update the contact's primary email ID
                contact.PrimaryEmailId = emailAddressId;
                await context.SaveChangesAsync();
            }
        }


        // DELETE
        public async Task DeletePhoneNumberAsync(int phoneNumberId)
        {
            using var context = _contextFactory.CreateDbContext();
            var phoneNumber = await context.PhoneNumbers.FindAsync(phoneNumberId);
            if (phoneNumber != null)
            {
                bool wasPrimary = phoneNumber.IsPrimary;
                int? contactId = phoneNumber.ContactId;

                context.PhoneNumbers.Remove(phoneNumber);
                await context.SaveChangesAsync();

                // If this was a primary phone and there are other phones, set a new primary
                if (wasPrimary && contactId.HasValue)
                {
                    var contact = await context.Contacts
                        .Include(c => c.PhoneNumbers)
                        .FirstOrDefaultAsync(c => c.ContactId == contactId);

                    if (contact != null && contact.PhoneNumbers.Any())
                    {
                        var newPrimary = contact.PhoneNumbers.First();
                        newPrimary.IsPrimary = true;
                        contact.PrimaryPhoneId = newPrimary.PhoneNumberId;
                        await context.SaveChangesAsync();
                    }
                    else if (contact != null)
                    {
                        // No phone numbers left, clear the primary
                        contact.PrimaryPhoneId = null;
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        public async Task DeleteEmailAddressAsync(int emailAddressId)
        {
            using var context = _contextFactory.CreateDbContext();
            var emailAddress = await context.EmailAddresses.FindAsync(emailAddressId);
            if (emailAddress != null)
            {
                bool wasPrimary = emailAddress.IsPrimary;
                int? contactId = emailAddress.ContactId;

                context.EmailAddresses.Remove(emailAddress);
                await context.SaveChangesAsync();

                // If this was a primary email and there are other emails, set a new primary
                if (wasPrimary && contactId.HasValue)
                {
                    var contact = await context.Contacts
                        .Include(c => c.EmailAddresses)
                        .FirstOrDefaultAsync(c => c.ContactId == contactId);

                    if (contact != null && contact.EmailAddresses.Any())
                    {
                        var newPrimary = contact.EmailAddresses.First();
                        newPrimary.IsPrimary = true;
                        contact.PrimaryEmailId = newPrimary.EmailAddressId;
                        await context.SaveChangesAsync();
                    }
                    else if (contact != null)
                    {
                        // No email addresses left, clear the primary
                        contact.PrimaryEmailId = null;
                        await context.SaveChangesAsync();
                    }
                }
            }
        }




        // SPECIAL FUNCTIONS
        public async Task<Contact> MigrateLegacyContactDataAsync(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .FirstOrDefaultAsync(c => c.ContactId == contactId);

            if (contact == null)
                return null;

            // Since we've already migrated the data and removed the legacy fields,
            // this method is now a no-op. We'll keep it for backward compatibility
            // but it will just return the contact as is.
            return contact;
        }

        /// <summary>
        /// Gets all phone numbers associated with a contact for use with the call history feature
        /// </summary>
        /// <param name="contactId">The ID of the contact to retrieve phone numbers for</param>
        /// <returns>A list of phone number strings in a clean format for matching with call logs</returns>
        public async Task<List<string>> GetPhoneNumbersForContact(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            var phoneNumbers = new List<string>();
            
            // Get the contact with their phone numbers
            var contact = await context.Contacts
                .Include(c => c.PhoneNumbers)
                .FirstOrDefaultAsync(c => c.ContactId == contactId);
                
            if (contact == null)
                return phoneNumbers;
                
            // Add phone numbers from the PhoneNumbers collection
            if (contact.PhoneNumbers != null)
            {
                foreach (var phone in contact.PhoneNumbers)
                {
                    if (!string.IsNullOrEmpty(phone.Number))
                    {
                        // Clean the number to ensure consistent format for matching in call logs
                        var cleanNumber = new string(phone.Number.Where(char.IsDigit).ToArray());
                        // Only add numbers with appropriate length (avoid adding partial numbers)
                        if (cleanNumber.Length >= 7)
                        {
                            phoneNumbers.Add(cleanNumber);
                        }
                    }
                }
            }
            
            return phoneNumbers;
        }

        public async Task<(bool IsPrimaryContact, string ClientName)> CheckIfPrimaryContactAsync(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            
            var client = await context.Clients
                .Where(c => c.PrimaryContactId == contactId)
                .FirstOrDefaultAsync();
                
            if (client != null)
            {
                return (true, client.Name);
            }
            
            return (false, string.Empty);
        }

        // TRASH?
        //public async Task DeleteContactAsync(int contactId)
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    var contact = await GetContactByIdAsync(contactId);
        //    if (contact != null)
        //    {
        //        context.Contacts.Remove(contact);
        //        await context.SaveChangesAsync();
        //    }
        //}
        //public async Task<PhoneNumber> GetPhoneNumberByIdAsync(int phoneNumberId)
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    return await context.PhoneNumbers
        //        .FindAsync(phoneNumberId);
        //}
        //public async Task<EmailAddress> GetEmailAddressByIdAsync(int emailAddressId)
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    return await context.EmailAddresses
        //        .FindAsync(emailAddressId);
        //}
        //public List<ContactDto> GetContactsList()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    return context.Contacts
        //        .Include(c => c.Client)
        //        .Include(c => c.Carrier)
        //        .Select(c => new ContactDto
        //        {
        //            ContactId = c.ContactId,
        //            FirstName = c.FirstName,
        //            LastName = c.LastName,
        //            Title = c.Title,
        //            Email = c.Email,
        //            Phone = c.Phone,
        //            Fax = c.Fax,
        //            Mobile = c.Mobile,
        //            Notes = c.Notes,
        //            DateCreated = c.DateCreated,
        //            AssociatedWith = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : "N/A"
        //        })
        //        .ToList();
        //}

        // DELETE CONTACT
        public async Task DeleteContactAsync(int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            
            // First check if this contact is used as a primary contact for any client
            var clientsUsingAsPrimary = await context.Clients
                .Where(c => c.PrimaryContactId == contactId)
                .ToListAsync();
                
            if (clientsUsingAsPrimary.Any())
            {
                var clientNames = string.Join(", ", clientsUsingAsPrimary.Select(c => c.Name));
                throw new InvalidOperationException($"Cannot delete this contact because it is set as the primary contact for the following client(s): {clientNames}. Please change the primary contact for these clients first.");
            }
            
            // Get the contact with all related data
            var contact = await context.Contacts
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .FirstOrDefaultAsync(c => c.ContactId == contactId);
                
            if (contact != null)
            {
                // Clear primary phone and email references to avoid circular dependency
                contact.PrimaryPhoneId = null;
                contact.PrimaryEmailId = null;
                
                // Save the changes to clear the references first
                await context.SaveChangesAsync();
                
                // Remove all phone numbers associated with this contact
                if (contact.PhoneNumbers != null && contact.PhoneNumbers.Any())
                {
                    context.PhoneNumbers.RemoveRange(contact.PhoneNumbers);
                }
                
                // Remove all email addresses associated with this contact
                if (contact.EmailAddresses != null && contact.EmailAddresses.Any())
                {
                    context.EmailAddresses.RemoveRange(contact.EmailAddresses);
                }
                
                // Remove the contact itself
                context.Contacts.Remove(contact);
                
                await context.SaveChangesAsync();
            }
        }

        // ADD THIS METHOD TO ContactService class
        public async Task SetPrimaryContactForClientAsync(int clientId, int contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            var client = await context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client != null)
            {
                client.PrimaryContactId = contactId;
                await context.SaveChangesAsync();
            }
        }

    }
}
