using Surefire.Data;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.ViewModels;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Surefire.Domain.Shared.Services
{
    public class HomeService
    {
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public HomeService(StateService stateService, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _dbContextFactory = dbContextFactory;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//
        // GET trackTasks [CurrentUser]
        public async Task<List<HomePageTasksViewModel>> GetHomePageTasksAsync()
        {
            var today = DateTime.Today;
            var currentUser = _stateService.CurrentUser;

            using var context = _dbContextFactory.CreateDbContext();
            // Fetch all TrackTasks from the database for the current user
            var tasks = await context.TrackTasks
                .AsNoTracking()
                .AsSplitQuery()
                .Where(t => t.Completed == false)
                .Where(t => t.GoalDate != null || t.Highlighted == true)
                .Where(t => t.AssignedTo == null && t.Renewal.AssignedTo == currentUser)
                .Select(t => new
                {
                    t.Renewal.RenewalId,
                    t.TaskName,
                    t.Notes,
                    t.Highlighted,
                    t.GoalDate,
                    ClientName = t.Renewal.Client.Name,
                    t.Renewal.Client.ClientId,
                    PolicyProduct = t.Renewal.Policy.Product.LineCode,
                    t.Renewal.RenewalDate
                })
                .ToListAsync();

            // Separate highlighted tasks
            var highlightedTasks = tasks
                .Where(t => t.Highlighted)
                .ToList();

            // For non-highlighted tasks, select the nearest task per renewal
            var nearestTasks = tasks
                .Where(t => !t.Highlighted)
                .GroupBy(t => t.RenewalId)
                .Select(g => g.OrderBy(t => t.GoalDate ?? DateTime.MaxValue).FirstOrDefault())
                .ToList();

            // Combine highlighted tasks and nearest non-highlighted tasks
            var combinedTasks = highlightedTasks
                .Union(nearestTasks)
                .ToList();

            // Create the ViewModel list
            var result = combinedTasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.ClientName,
                ClientId = t.ClientId,
                PolicyProduct = t.PolicyProduct ?? "10",
                RenewalDate = t.RenewalDate,
                Priority = t.GoalDate.HasValue
            ? (t.GoalDate < today
                ? $"<span style='color:red'>{(today - t.GoalDate.Value).Days} Days Late</span>"
                : $"{(t.GoalDate.Value - today).Days} Days Left")
            : "ASAP"
            }).OrderByDescending(t => t.Highlighted)
              .ThenByDescending(t => t.GoalDate.HasValue && t.GoalDate < today)
              .ThenBy(t => t.GoalDate)
              .Take(20)
              .ToList();

            return result;
        }
        // GET trackTasks [CurrentUser IS subassigned]
        public async Task<List<HomePageTasksViewModel>> GetHomePageSubTasksAsync()
        {
            var today = DateTime.Today;

            var currentUser = _stateService.CurrentUser;
            if (currentUser == null)
            {
                throw new InvalidOperationException("User initialization failed or user is not authenticated.");
            }

            using var context = _dbContextFactory.CreateDbContext();
            // Fetch all TrackTasks for the current user that are incomplete
            var tasks = await context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(p => p.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false) // Only incomplete tasks
                .Where(t => t.AssignedTo == currentUser) // Assigned to the current user
                .AsNoTracking()
                .Select(t => new
                {
                    t.Renewal.RenewalId,
                    t.TaskName,
                    t.Notes,
                    t.Highlighted,
                    t.GoalDate,
                    ClientName = t.Renewal.Client.Name,
                    t.Renewal.Client.ClientId,
                    PolicyProduct = t.Renewal.Policy.Product.LineCode,
                    PolicyNumber = t.Renewal.Policy.PolicyNumber,
                    t.Renewal.RenewalDate
                })
                .ToListAsync();

            // Separate highlighted tasks (to appear at the top)
            var highlightedTasks = tasks
                .Where(t => t.Highlighted)
                .ToList();

            // For non-highlighted tasks, order by nearest GoalDate or RenewalDate
            var nonHighlightedTasks = tasks
                .Where(t => !t.Highlighted)
                .OrderBy(t => t.GoalDate ?? t.RenewalDate)
                .ToList();

            // Combine highlighted tasks with non-highlighted ones
            var combinedTasks = highlightedTasks
                .Concat(nonHighlightedTasks)
                .Take(40) // Take only 40 tasks
                .ToList();

            // Create the ViewModel list
            var result = combinedTasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.ClientName,
                ClientId = t.ClientId,
                PolicyProduct = t.PolicyProduct ?? "10", // Default if null
                RenewalDate = t.RenewalDate,
                PolicyNumber = t.PolicyNumber,
                Priority = t.GoalDate.HasValue
                    ? (t.GoalDate < today
                        ? $"<span style='color:red'>{(today - t.GoalDate.Value).Days} Days Late</span>"
                        : $"{(t.GoalDate.Value - today).Days} Days Left")
                    : "ASAP"
            }).ToList();

            // Order by Highlighted tasks first, then the rest by GoalDate or RenewalDate
            return result;
        }
        // GET trackTasks [CurrentUser AND incomplete]
        public async Task<List<HomePageTasksViewModel>> GetIncompleteTasksForCurrentUserAsync()
        {
            var today = DateTime.Today;
            var currentUser = _stateService.CurrentUser;

            using var context = _dbContextFactory.CreateDbContext();
            // Fetch all incomplete tasks assigned to the current user where RenewalDate is before today
            var tasks = await context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(s => s.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false) // Only incomplete tasks
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser) // Assigned to current user
                .Where(t => t.Renewal.RenewalDate < today) // Only tasks with past RenewalDate (before today)
                .OrderBy(t => t.Renewal.RenewalDate) // Order by RenewalDate with oldest at the top
                .Take(20) // Limit to 20 tasks
                .AsNoTracking()
                .ToListAsync();

            // Create the ViewModel list
            var result = tasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.Renewal.RenewalId,
                TaskName = t.TaskName,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.Renewal.Client.Name ?? "NA",
                ClientId = t.Renewal.Client.ClientId,
                PolicyProduct = t.Renewal.Policy?.Product?.LineCode ?? "Unknown",
                RenewalDate = t.Renewal.RenewalDate,
                Priority = $"Renewal Date: {t.Renewal.RenewalDate.ToShortDateString()}"
            }).ToList();

            return result;
        }
        // GET dailyTasks [CurrentUser]
        public async Task<List<DailyTask>> GetDailyTasksAsync()
        {
            var currentUser = _stateService.CurrentUser;
            var today = DateTime.Today;

            using var context = _dbContextFactory.CreateDbContext();

            var tasks = await context.DailyTasks
                .Where(task => !task.Completed)
                .Where(t => t.AssignedToId == currentUser.Id)
                .OrderBy(t => t.Order)
                .ToListAsync();

            return tasks;
        }
        // GET dailyTasks [CurrentUser AND completed]
        public async Task<List<DailyTask>> GetDailyCompletedTasksAsync()
        {
            var currentUser = _stateService.CurrentUser;
            var today = DateTime.Today;

            using var context = _dbContextFactory.CreateDbContext();

            var tasks = await context.DailyTasks
                .Where(task => task.Completed && task.CompletedDate.HasValue && task.CompletedDate.Value.Date == today)
                .Where(t => t.AssignedToId == currentUser.Id)
                .ToListAsync();

            return tasks;
        }
        // GET renewal policies [CurrentUser AND expiringSoon]
        public async Task<List<Policy>> GetUpcomingRenewalsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var upcomingDate = currentDate.AddDays(14);

            using var context = _dbContextFactory.CreateDbContext();
            var upcomingRenewals = await context.Policies
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Where(p => p.ExpirationDate >= currentDate && p.ExpirationDate <= upcomingDate)
                .AsNoTracking()
                .ToListAsync();

            return upcomingRenewals;
        }
        // GET LEADS [All]
        public async Task<List<Lead>> GetAllLeadsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var theleads = await context.Leads
                .Include(l => l.CreatedBy)
                .Include(l => l.Submissions)
                .Include(l => l.LeadNotes)
                .Include(l => l.Product)
                .Where(l => l.Stage < 3)
                .OrderBy(x => x.Stage).ToListAsync();
            return theleads;
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // MAIN LAYOUT ---------------------------------------------------------------- //
        public async Task<CallInfoMatchResult> GetCallerInfo(CallInfo callInfo)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Clean the caller ID phone number (remove non-numeric characters)
            string cleanedCallerId = CleanPhoneNumber(callInfo.CallerId);

            if(cleanedCallerId != "")
            {
                // Search for matching clients using the Contains method
                var matchingClients = await context.Clients
                    .Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(cleanedCallerId))
                    .ToListAsync();

                // Search for matching contacts using the Contains method
                var matchingContacts = await context.Contacts
                    .Where(c => (c.Phone != null && c.Phone.Contains(cleanedCallerId)) ||
                        (c.Mobile != null && c.Mobile.Contains(cleanedCallerId)))
                    .Include(c => c.Client) // Include the related Client
                    .ToListAsync();
                if (matchingContacts.Any())
                {
                    // Return the first matching contact
                    return new CallInfoMatchResult { MatchedContact = matchingContacts.First() };
                }
                if (matchingClients.Any())
                {
                    // Return the first matching client
                    return new CallInfoMatchResult { MatchedClient = matchingClients.First() };
                }
            }
            return null;
        }
        private string CleanPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return string.Empty;

            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }
    }
}