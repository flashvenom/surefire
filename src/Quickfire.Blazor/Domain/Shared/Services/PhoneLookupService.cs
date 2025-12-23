using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public class PhoneLookupResult
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public int? ContactId { get; set; }
        public string? ClientName { get; set; }
        public int? ClientId { get; set; }
        public string? CarrierName { get; set; }
        public int? CarrierId { get; set; }
        public bool IsFound { get; set; }
        
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(ContactName))
                {
                    var name = ContactName;
                    if (!string.IsNullOrEmpty(ClientName))
                    {
                        name += $" ({ClientName})";
                    }
                    else if (!string.IsNullOrEmpty(CarrierName))
                    {
                        name += $" ({CarrierName})";
                    }
                    return name;
                }
                return PhoneNumber;
            }
        }
    }

    public class PhoneLookupService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public PhoneLookupService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Looks up a phone number in the database to find associated contact and client information
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup (will be normalized)</param>
        /// <returns>PhoneLookupResult with contact and client information if found</returns>
        public async Task<PhoneLookupResult> LookupPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return new PhoneLookupResult { PhoneNumber = phoneNumber };
            }

            // Normalize the phone number for consistent lookup
            var normalizedPhone = StringHelper.NormalizePhoneNumber(phoneNumber);
            var cleanPhone = StringHelper.CleanPhoneNumber(phoneNumber);
            
            using var context = _contextFactory.CreateDbContext();
            
            try
            {
                // Get all phone records and filter in memory for better matching
                var phoneRecords = await context.PhoneNumbers
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Client)
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Carrier)
                    .ToListAsync();

                // Look for the phone number in the PhoneNumbers table with multiple format matching
                var phoneRecord = phoneRecords.FirstOrDefault(p => 
                    p.Number == normalizedPhone || 
                    p.Number == phoneNumber ||
                    p.Number == cleanPhone ||
                    StringHelper.NormalizePhoneNumber(p.Number) == normalizedPhone ||
                    StringHelper.CleanPhoneNumber(p.Number) == cleanPhone);

                if (phoneRecord?.Contact != null)
                {
                    return new PhoneLookupResult
                    {
                        PhoneNumber = phoneNumber,
                        ContactName = phoneRecord.Contact.FullName,
                        ContactTitle = phoneRecord.Contact.Title,
                        ContactId = phoneRecord.Contact.ContactId,
                        ClientName = phoneRecord.Contact.Client?.Name,
                        ClientId = phoneRecord.Contact.ClientId,
                        CarrierName = phoneRecord.Contact.Carrier?.CarrierName,
                        CarrierId = phoneRecord.Contact.CarrierId,
                        IsFound = true
                    };
                }

                // If not found in PhoneNumbers table, check if it's a client's legacy phone number
                var clients = await context.Clients.ToListAsync();
                var clientWithPhone = clients.FirstOrDefault(c => 
                    c.PhoneNumber == normalizedPhone || 
                    c.PhoneNumber == phoneNumber ||
                    c.PhoneNumber == cleanPhone ||
                    (c.PhoneNumber != null && StringHelper.NormalizePhoneNumber(c.PhoneNumber) == normalizedPhone) ||
                    (c.PhoneNumber != null && StringHelper.CleanPhoneNumber(c.PhoneNumber) == cleanPhone));

                if (clientWithPhone != null)
                {
                    return new PhoneLookupResult
                    {
                        PhoneNumber = phoneNumber,
                        ClientName = clientWithPhone.Name,
                        ClientId = clientWithPhone.ClientId,
                        IsFound = true
                    };
                }

                // Return not found result
                return new PhoneLookupResult
                {
                    PhoneNumber = phoneNumber,
                    IsFound = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error looking up phone number {phoneNumber}: {ex.Message}");
                return new PhoneLookupResult
                {
                    PhoneNumber = phoneNumber,
                    IsFound = false
                };
            }
        }

        /// <summary>
        /// Batch lookup multiple phone numbers for better performance
        /// </summary>
        /// <param name="phoneNumbers">List of phone numbers to lookup</param>
        /// <returns>Dictionary of phone number to lookup result</returns>
        public async Task<Dictionary<string, PhoneLookupResult>> LookupPhoneNumbersAsync(IEnumerable<string> phoneNumbers)
        {
            var results = new Dictionary<string, PhoneLookupResult>();
            var normalizedNumbers = new List<string>();
            var originalToNormalized = new Dictionary<string, string>();

            // Normalize all phone numbers
            foreach (var phone in phoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var normalized = StringHelper.NormalizePhoneNumber(phone);
                normalizedNumbers.Add(normalized);
                originalToNormalized[phone] = normalized;
                
                Console.WriteLine($"PhoneLookupService: Normalized '{phone}' to '{normalized}'");
            }

            if (!normalizedNumbers.Any())
            {
                return results;
            }

            using var context = _contextFactory.CreateDbContext();
            
            try
            {
                // Get all phone records and filter in memory for better matching
                var phoneRecords = await context.PhoneNumbers
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Client)
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Carrier)
                    .ToListAsync();
                
                Console.WriteLine($"PhoneLookupService: Found {phoneRecords.Count} phone records in database");
                foreach (var phone in phoneRecords.Take(5)) // Log first 5 for debugging
                {
                    Console.WriteLine($"  DB Phone: '{phone.Number}' -> Contact: {phone.Contact?.FullName ?? "None"}");
                }

                // Get all clients for legacy phone number matching
                var clients = await context.Clients.ToListAsync();
                
                Console.WriteLine($"PhoneLookupService: Found {clients.Count} clients in database");
                foreach (var client in clients.Where(c => !string.IsNullOrEmpty(c.PhoneNumber)).Take(5)) // Log first 5 with phones
                {
                    Console.WriteLine($"  DB Client: '{client.PhoneNumber}' -> {client.Name}");
                }

                // Create lookup results for each phone number
                foreach (var phone in phoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    var normalized = originalToNormalized[phone];
                    var cleanPhone = StringHelper.CleanPhoneNumber(phone);
                    
                    // Check phone records first with multiple format matching
                    var phoneRecord = phoneRecords.FirstOrDefault(p => 
                        p.Number == normalized || 
                        p.Number == phone ||
                        p.Number == cleanPhone ||
                        StringHelper.NormalizePhoneNumber(p.Number) == normalized ||
                        StringHelper.CleanPhoneNumber(p.Number) == cleanPhone);

                    if (phoneRecord?.Contact != null)
                    {
                        Console.WriteLine($"PhoneLookupService: Found contact match for '{phone}' -> '{phoneRecord.Contact.FullName}'");
                        results[phone] = new PhoneLookupResult
                        {
                            PhoneNumber = phone,
                            ContactName = phoneRecord.Contact.FullName,
                            ContactTitle = phoneRecord.Contact.Title,
                            ContactId = phoneRecord.Contact.ContactId,
                            ClientName = phoneRecord.Contact.Client?.Name,
                            ClientId = phoneRecord.Contact.ClientId,
                            CarrierName = phoneRecord.Contact.Carrier?.CarrierName,
                            CarrierId = phoneRecord.Contact.CarrierId,
                            IsFound = true
                        };
                    }
                    else
                    {
                        // Check client legacy phone numbers with multiple format matching
                        var clientWithPhone = clients.FirstOrDefault(c => 
                            c.PhoneNumber == normalized || 
                            c.PhoneNumber == phone ||
                            c.PhoneNumber == cleanPhone ||
                            (c.PhoneNumber != null && StringHelper.NormalizePhoneNumber(c.PhoneNumber) == normalized) ||
                            (c.PhoneNumber != null && StringHelper.CleanPhoneNumber(c.PhoneNumber) == cleanPhone));

                        if (clientWithPhone != null)
                        {
                            Console.WriteLine($"PhoneLookupService: Found client match for '{phone}' -> '{clientWithPhone.Name}'");
                            results[phone] = new PhoneLookupResult
                            {
                                PhoneNumber = phone,
                                ClientName = clientWithPhone.Name,
                                ClientId = clientWithPhone.ClientId,
                                IsFound = true
                            };
                        }
                        else
                        {
                            results[phone] = new PhoneLookupResult
                            {
                                PhoneNumber = phone,
                                IsFound = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error batch looking up phone numbers: {ex.Message}");
                
                // Return not found results for all phone numbers
                foreach (var phone in phoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    results[phone] = new PhoneLookupResult
                    {
                        PhoneNumber = phone,
                        IsFound = false
                    };
                }
            }

            return results;
        }
    }
} 