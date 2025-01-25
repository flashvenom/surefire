using Surefire.Data;
using Surefire.Domain.Plugins;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Concurrent;
using RingCentral;
using Newtonsoft.Json;

namespace Surefire.Domain.Shared.Services
{
    public class StateService
    {
        // Database context and service provider
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;

        public StateService(IServiceProvider serviceProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;
        }

        //=============================================
        //             ** STATIC DATA **              //
        //=============================================
        
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        public string DatabaseProvider { get; private set; } = string.Empty;
        public string SurefireVersion = "v0.0.0";

        // Static lists Props ----------------------------------------------------------------//
        private Task<List<Carrier>>? _allCarriersTask;
        private Task<List<Carrier>>? _allWholesalersTask;
        private Task<List<Product>>? _allProductsTask;
        private Task<List<ApplicationUser>>? _allUsersTask;
        public Task<List<Carrier>> AllCarriers => _allCarriersTask ??= LoadCarriersAsync();
        public Task<List<Carrier>> AllWholesalers => _allWholesalersTask ??= LoadWholesalersAsync();
        public Task<List<Product>> AllProducts => _allProductsTask ??= LoadProductsAsync();
        public Task<List<ApplicationUser>> AllUsers => _allUsersTask ??= LoadUsersAsync();

        // Static lists Methods---------------------------------------------------------------//
        public async Task InitializeStateAsync(Task<AuthenticationState> authStateTask)
        {
            Console.WriteLine($"State Service Initialized: {_isInitialized}");
            if (_isInitialized) return;
            Console.WriteLine($"-----Initializing State Service ({_isInitialized})-----");

            // User initialization
            SurefireVersion = _configuration["Surefire:System:Version"] ?? "v0.0.0";
            var authState = await authStateTask;
            var user = authState.User;
            using var context = _dbContextFactory.CreateDbContext();
            DatabaseProvider = context.Database.ProviderName ?? string.Empty;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                CurrentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (CurrentUser != null)
                {
                    // Update LastLogin timestamp
                    CurrentUser.LastLogin = DateTime.UtcNow;
                    context.Users.Update(CurrentUser);
                    await context.SaveChangesAsync();
                }
            }

            // Data initialization
            _allCarriersTask ??= LoadCarriersAsync();
            _allWholesalersTask ??= LoadWholesalersAsync();
            _allProductsTask ??= LoadProductsAsync();
            _allUsersTask ??= LoadUsersAsync();
            var setMostRecentlyOpenedClientIdTask = SetMostRecentlyOpenedClientIdAsync();
            var refreshCallLogsTask = RefreshCallLogsAsync(CancellationToken.None);
            var refreshRecentPaymentsTask = RefreshRecentPaymentsAsync(CancellationToken.None);

            await Task.WhenAll(_allCarriersTask, _allWholesalersTask, _allProductsTask, _allUsersTask, setMostRecentlyOpenedClientIdTask, refreshCallLogsTask, refreshRecentPaymentsTask);

            _isInitialized = true;
            Console.WriteLine($"Initialization of State Service is Done ({_isInitialized})-----");
        }
        private async Task<List<Carrier>> LoadCarriersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Carriers.ToListAsync();
        }
        private async Task<List<Carrier>> LoadWholesalersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Carriers.Where(c => c.Wholesaler).ToListAsync();
        }
        private async Task<List<Product>> LoadProductsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Products.ToListAsync();
        }
        private async Task<List<ApplicationUser>> LoadUsersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Users.ToListAsync();
        }
        // CurrentUser Props -----------------------------------------------------------------//
        public ApplicationUser? CurrentUser { get; private set; }
        


        //=============================================
        //               RENEWAL STATE               //
        //=============================================
        public int HtmlRenewalId { get; set; } = 0;
        public int HtmlMonth { get; set; } = DateTime.Now.Month;
        public int HtmlYear { get; set; } = DateTime.Now.Year;
        public string HtmlTab { get; set; } = "tab-1";
        public string HtmlUser { get; set; } = "Everyone";
        public string HtmlView { get; set; } = "list";
        public bool IsLoading { get; set; } = false;
        public List<RenewalListItemViewModel> RenewalList { get; set; } = new();
        public List<Policy> PolicyOrphanList { get; set; } = new();


        //=============================================
        //             CLIENTS SCREEN                 //
        //=============================================
        public event Action? OnAttachmentListUpdated;
        public void NotifyAttachmentListUpdated() => OnAttachmentListUpdated?.Invoke();
        public Func<int, Task> LoadClientFromSearch { get; set; }
        public string ClientTab { get; set; } = "tab-1";
        public int ClientId { get; set; }
        public async Task SetMostRecentlyOpenedClientIdAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Fetch the most recently opened client ID and set it to the ClientId property
            ClientId = await context.Clients
                .OrderByDescending(c => c.DateOpened)
                .Select(c => (int?)c.ClientId)
                .FirstOrDefaultAsync() ?? 0;
        }

        //=============================================
        //                 CALL LOGS                  //
        //=============================================
        private readonly ConcurrentDictionary<string, List<CallLogRecord>> _callLogCache = new();
        private DateTime _lastFetched = DateTime.MinValue;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);
        public async Task<List<CallLogRecord>> GetCallLogsAsync(List<string> phoneNumbers, bool showAll, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            if (_lastFetched.Add(_cacheDuration) < now)
            {
                await RefreshCallLogsAsync(cancellationToken);
            }
            if(showAll)
            {
                return _callLogCache.Values.SelectMany(logs => logs).ToList();
            }
            else
            {
                return phoneNumbers == null || !phoneNumbers.Any()
                ? _callLogCache.Values.SelectMany(logs => logs).ToList()
                : _callLogCache.Values.SelectMany(logs => logs)
                    .Where(log => phoneNumbers.Contains(log.from?.phoneNumber) || phoneNumbers.Contains(log.to?.phoneNumber))
                    .OrderByDescending(log => log.startTime)
                    .ToList();
            }
        }
        private async Task RefreshCallLogsAsync(CancellationToken cancellationToken)
        {
            var plugin = _serviceProvider.GetService<ICallLogPlugin>();

            if (plugin == null || !plugin.IsActive)
            {
                Console.Error.WriteLine("RingCentral plugin not available.");
                return;
            }

            try
            {
                var callLogs = await plugin.GetCallLogsAsync(DateTime.UtcNow.AddDays(-45), DateTime.UtcNow, cancellationToken);
                _callLogCache.Clear();

                foreach (var log in callLogs)
                {
                    var key = log.from?.phoneNumber ?? log.to?.phoneNumber ?? "unknown";
                    if (!_callLogCache.ContainsKey(key))
                        _callLogCache[key] = new List<CallLogRecord>();

                    _callLogCache[key].Add(log);
                }

                _lastFetched = DateTime.UtcNow;
                Console.WriteLine($"Call logs refreshed with {_callLogCache.Count} entries.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to refresh call logs: {ex.Message}");
            }
        }

        //=============================================
        //               RECENT PAYMENTS              //
        //=============================================
        private readonly ConcurrentDictionary<string, List<RecentTransactions>> _paymentCache = new();
        private DateTime _lastPaymentFetchTime = DateTime.MinValue;
        private readonly TimeSpan _paymentCacheDuration = TimeSpan.FromMinutes(15);
        public async Task<List<RecentTransactions>> GetRecentPaymentsAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            if (_lastPaymentFetchTime.Add(_paymentCacheDuration) < now)
            {
                await RefreshRecentPaymentsAsync(cancellationToken);
            }

            return _paymentCache.Values.SelectMany(transactions => transactions).ToList();
        }
        private async Task RefreshRecentPaymentsAsync(CancellationToken cancellationToken)
        {
            var plugin = _serviceProvider.GetService<IPayLogPlugin>();

            if (plugin == null || !plugin.IsActive)
            {
                Console.Error.WriteLine("ePayPolicy plugin not available.");
                return;
            }

            try
            {
                var response = await plugin.GetRecentPayments(cancellationToken);

                if (response != null && response.success)
                {
                    var transactionsResponse = JsonConvert.DeserializeObject<TransactionsResponse>(response.jsonresponse);
                    var transactions = transactionsResponse?.Transactions ?? new List<RecentTransactions>();

                    _paymentCache.Clear();
                    foreach (var transaction in transactions)
                    {
                        var key = transaction.Email ?? "unknown";
                        if (!_paymentCache.ContainsKey(key))
                        {
                            _paymentCache[key] = new List<RecentTransactions>();
                        }

                        _paymentCache[key].Add(transaction);
                    }

                    _lastPaymentFetchTime = DateTime.UtcNow;
                    Console.WriteLine($"Recent payments refreshed with {_paymentCache.Values.Sum(list => list.Count)} transactions.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to refresh recent payments: {ex.Message}");
            }
        }
        
        //=============================================
        //               STATUS MESSAGE               //
        //=============================================
        public event Action? OnStatusChanged;
        public event Action? OnSectionChanged;
        private string _traceSection = "Home";
        private string _statusMessage = "Loading...";
        private bool _statusLoading = false;
        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    NotifyStatusChanged();
                }
            }
        }
        public bool StatusLoading
        {
            get => _statusLoading;
            private set
            {
                if (_statusLoading != value)
                {
                    _statusLoading = value;
                    NotifyStatusChanged();
                }
            }
        }
        public void UpdateStatus(string newStatus, bool? isLoading = null)
        {
            StatusMessage = newStatus;
            if (isLoading.HasValue)
            {
                StatusLoading = isLoading.Value;
            }
        }
        private void NotifyStatusChanged() => OnStatusChanged?.Invoke();



        //=============================================
        //                  PLUGINS                  //
        //=============================================
        // Runs a specific method on all plugins or a specific plugin
        public event Func<string, Task>? OnPluginEventTriggered;
        public async Task TriggerPluginEvent(string eventName)
        {
            if (OnPluginEventTriggered != null)
            {
                await OnPluginEventTriggered.Invoke(eventName);
            }
        }

        public async Task<List<PluginMethodResponse>> RunPluginMethodAsync(string methodName, object[] parameters, CancellationToken cancellationToken)
        {
            var plugins = _serviceProvider.GetServices<IPlugin>().Where(plugin => plugin.IsActive).ToList();
            
            if (!plugins.Any())
            {
                Console.Error.WriteLine("PLUGIN: No active plugins available.");
                return new List<PluginMethodResponse>();
            }

            var responses = new List<PluginMethodResponse>();
            foreach (var plugin in plugins)
            {
                try
                {
                    var response = await plugin.ExecuteAsync(methodName, parameters, cancellationToken);
                    responses.Add(response);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error executing {methodName} on plugin {plugin.Name}: {ex.Message}");
                    responses.Add(new PluginMethodResponse { success = false, message = ex.Message });
                }
            }

            return responses;
        }


        //=============================================
        //              SYSTEM SETTINGS              //
        //=============================================
        public async Task<Settings?> GetSystemSettingsAsync()
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var settings = await context.Settings.FirstOrDefaultAsync();
                return settings;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching system settings: {ex.Message}");
                return null;
            }
        }

        public async Task SaveSystemSettingsAsync(Settings settings)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var existingSettings = await context.Settings.FirstOrDefaultAsync();

                if (existingSettings == null)
                {
                    // If no settings exist, add the new settings
                    context.Settings.Add(settings);
                }
                else
                {
                    // Update the existing settings
                    existingSettings.OpenAiApiKey = settings.OpenAiApiKey;
                    existingSettings.DbType = settings.DbType;
                    existingSettings.DbConnectionString = settings.DbConnectionString;
                    existingSettings.PayLinkStringTemplate = settings.PayLinkStringTemplate;
                    existingSettings.AzureBlobConnectionString = settings.AzureBlobConnectionString;
                    existingSettings.AzureBlobContainerName = settings.AzureBlobContainerName;
                    existingSettings.FileServerMappedPath = settings.FileServerMappedPath;
                    existingSettings.DisablePlugins = settings.DisablePlugins;

                    context.Settings.Update(existingSettings);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving system settings: {ex.Message}");
            }
        }
    }
}
