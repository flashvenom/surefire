using Surefire.Data;
using Surefire.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Surefire.Domain.Logs
{
    public interface ILoggingService
    {
        Task LogAsync(LogLevel logLevel, string message, string source = null, Exception exception = null);
        Task<List<Log>> GetLogsForCurrentUserAsync();
    }

    public class LoggingService : ILoggingService
    {
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public LoggingService(StateService stateService, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _dbContextFactory = dbContextFactory;
        }

        public async Task LogAsync(LogLevel logLevel, string message, string source = null, Exception exception = null)
        {
            Console.WriteLine(message);
            using var context = _dbContextFactory.CreateDbContext();
            ApplicationUser? currentUser = null;
            try
            {
                currentUser = _stateService.CurrentUser;
                if (currentUser != null)
                {
                    context.Users.Attach(currentUser);
                }
            }
            catch
            {
                //Nothing
            }

            var log = new Log
            {
                LogLevel = logLevel,
                Message = message,
                Source = source,
                User = currentUser,
                Exception = exception?.ToString(),
                Timestamp = DateTime.UtcNow
            };

            context.Logs.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task<List<Log>> GetLogsForCurrentUserAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;

            if (currentUser == null)
            {
                return new List<Log>(); // No user logged in
            }

            return await context.Logs
                .Where(log => log.User != null && log.User.Id == currentUser.Id)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }

    public class Log
    {
        public int LogId { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? Exception { get; set; }
        public string? Source { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
