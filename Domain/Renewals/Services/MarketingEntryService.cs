//using Folient.Data;
//using Folient.Domain.Renewals.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Folient.Domain.Renewals.Services
//{
//    public class MarketingEntryService
//    {
//        private readonly ApplicationDbContext _context;

//        public MarketingEntryService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<MarketingEntry>> GetMarketingEntriesAsync()
//        {
//            return await _context.MarketingEntries
//                                 .Include(me => me.Tasks)
//                                 .Include(me => me.Submissions)
//                                 .OrderBy(me => me.ExpirationDate)
//                                 .ToListAsync();
//        }


//    }
//}
