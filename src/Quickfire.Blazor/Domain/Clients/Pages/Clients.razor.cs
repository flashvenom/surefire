using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Clients.Services;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Ember;
using Quickfire.Blazor.Domain.Forms.Components;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Forms.Services;
using Quickfire.Blazor.Domain.Logs;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Renewals.Services;
using Quickfire.Blazor.Domain.Shared.Components;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Quickfire.Blazor.Domain.Shared.Services;
using System.Collections.Generic;
using System.Data;

namespace Quickfire.Blazor.Domain.Clients.Components
{
    public partial class ClientsBase : ComponentBase, IDisposable
    {
        // Constants
        private const string TabOverview = "tab-1";
        private const string TabPolicies = "tab-2";
        private const int DefaultListTake = 40;
        private const int InitialListTake = 50;
        private const int SearchResultsTake = 20;

        [Parameter, SupplyParameterFromQuery] public int LoadClientId { get; set; }
        [Parameter] public bool showCreatePolicy { get; set; } = false;
        [CascadingParameter] public Action<string>? UpdateHeader { get; set; }
        [Inject] protected StateService? _stateService { get; set; }
        [Inject] protected FormService? FormService { get; set; }
        [Inject] protected ClientService? ClientService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected EmberService? EmberService { get; set; }
        [Inject] protected ILoggingService? _logs { get; set; }
        [Inject] protected IJSRuntime? JS { get; set; }
        [Inject] protected ClientStateService? ClientStateService { get; set; }
        [Inject] protected RenewalService? RenewalService { get; set; }
        
        // Clients
        protected Quickfire.Blazor.Domain.Clients.Models.Client? selectedClient;
        protected List<ClientListItem> clients = new();
        protected List<ClientListItem> filteredClients = new();
        // Related
        protected List<Policy> currentPolicies = new();
        protected List<Policy> pastPolicies = new();
        protected List<Contact> contactList = new();
        protected List<string> phoneNumbers = new();
        protected List<Contact> loadedContacts = new();
        protected List<FormPdf> allFormPdfs = new();
        // UI
        protected bool isLoading = false;
        protected bool showNotes = true;
        protected bool showProposalCleaner = false;
        protected bool utilityLoading = false;
        protected string dynamicClass = "sf-quicklist-close";
        protected string searchTerm = string.Empty;
        protected string utilityStatus = "";
        protected int currentProposalId;
        protected FluentTabs? tabInterface;
        private CancellationTokenSource? _dataSyncCts;
        private CancellationTokenSource? _clientListCts;
        protected FormDocList formDocListComponent;


        

        // - - - - - OnInit - - - - - -   -     -        -     /
        protected override async Task OnInitializedAsync()
        {
            UpdateHeader?.Invoke("Clients");
            _stateService.LoadClientFromSearch = LoadClientFromSearchBar;
            _stateService.OnClientUpdated += HandleClientUpdate;

            // Load client-specific state from local storage
            if (ClientStateService != null)
            {
                await ClientStateService.LoadStateAsync();
                if (!string.IsNullOrEmpty(ClientStateService.SearchTerm))
                {
                    searchTerm = ClientStateService.SearchTerm;
                }
            }

            // Load cached data or fetch from services if cache is invalid/empty
            await LoadCachedOrFreshData();

            //Check if database is empty
            if (clients.Count == 0)
            {
                NavigationManager.NavigateTo("/Clients/Create", false);
                return;
            }
            // Always prefer URL parameter if present and valid
            if (LoadClientId > 0)
            {
                await LoadClient(LoadClientId);
            }
            else
            {
                // fallback: load last selected client from state or default
                var fallbackClientId = ClientStateService?.SelectedClientId ?? clients.FirstOrDefault()?.ClientId ?? 0;
                if (fallbackClientId > 0)
                {
                    await LoadClient(fallbackClientId);
                }
            }
        }
        protected void ShowProposalCleaner((int clientId, int proposalId) args)
        {
            currentProposalId = args.proposalId;
            showProposalCleaner = true;
            StateHasChanged();
        }
        private async Task LoadCachedOrFreshData()
        {
            var tasks = new List<Task>();

            // Check and load FormPdfs
            if (ClientStateService != null && ClientStateService.IsFormPdfsCacheValid())
            {
                allFormPdfs = ClientStateService.AllFormPdfs ?? new List<FormPdf>();
            }
            else
            {
                var formPdfsTask = Task.Run(async () =>
                {
                    var pdfs = await FormService?.GetAllFormPdfs();
                    allFormPdfs = pdfs ?? new List<FormPdf>();
                    
                    // Cache the result
                    if (ClientStateService != null)
                    {
                        ClientStateService.SetFormPdfsCache(allFormPdfs);
                    }
                });
                tasks.Add(formPdfsTask);
            }

            // Check and load Clients
            if (ClientStateService != null && ClientStateService.IsClientsCacheValid())
            {
                clients = ClientStateService.AllClients ?? new List<ClientListItem>();
            }
            else
            {
                var clientsTask = Task.Run(async () =>
                {
                    var clientList = await ClientService?.GetClientListAsync();
                    clients = clientList ?? new List<ClientListItem>();
                    
                    // Cache the result
                    if (ClientStateService != null)
                    {
                        ClientStateService.SetClientsCache(clients);
                    }
                });
                tasks.Add(clientsTask);
            }

            // Wait for any remaining tasks to complete
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }

            // Update filtered clients list
            filteredClients = clients.Take(InitialListTake).ToList();

            // Save the updated cache to local storage
            if (ClientStateService != null)
            {
                await ClientStateService.SaveStateAsync();
            }
        }
        private async Task HandleClientUpdate(int clientId)
        {
            if (clientId == selectedClient?.ClientId)
            {
                await ReloadSelectedClient();
                await InvokeAsync(StateHasChanged);
            }
        }

        // - - - - - Main Methods - - - - - -   -   -     -    /
        private async Task LoadClient(int loadClientId)
        {
            Console.WriteLine($"[DEBUG] Loading client from service for ClientId {loadClientId}, showing spinner.");
            isLoading = true;
            StateHasChanged();

            try
            {
                selectedClient = await ClientService.GetClientById(loadClientId) ??
                 await ClientService.GetClientById(clients.FirstOrDefault()?.ClientId ?? 0);

                // Always update SelectedClientId when loading a client
                if (selectedClient != null && ClientStateService != null)
                {
                    ClientStateService.SelectedClientId = selectedClient.ClientId;
                }

                // Create the contact list
                contactList = selectedClient?.Contacts?.ToList() ?? new List<Contact>();

                // Update policy lists
                UpdatePolicyLists();

                searchTerm = string.Empty;
                phoneNumbers = GetClientAndContactPhoneNumbers(selectedClient);

                // Save only the small state to ClientStateService
                if (ClientStateService != null)
                {
                    await ClientStateService.SaveStateAsync();
                }
            }
            catch (Exception ex)
            {
                await _logs.LogAsync(LogLevel.Error, ex.ToString(), "LoadClient in Clients.razor");
                Console.Error.WriteLine($"Error loading client: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }

            // Initiate policy sync and import
            await RunDataSync(false);
        }
        private async Task FilterClients()
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                //Making new filter list for clients
                filteredClients = clients.Take(DefaultListTake).ToList();
            }
            else
            {
                //Filtering clients
                filteredClients = clients
                    .Where(client => client.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Take(SearchResultsTake)
                    .ToList();
            }
        }
        protected async Task LoadClientClickHandler(int clientId)
        {
            ExpandDetails(0);
            searchTerm = string.Empty;
            if (ClientStateService != null)
            {
                ClientStateService.ActiveTab = TabOverview;
                ClientStateService.SelectedClientId = clientId;
            }
            await tabInterface.GoToTabAsync(TabOverview);
            NavigationManager.NavigateTo($"/Clients/{clientId}", false);
            await LoadClient(clientId);
        }
        public async Task LoadClientFromSearchBar(int newClientId)
        {
            if (newClientId != selectedClient?.ClientId)
            {
                ExpandDetails(0);
                searchTerm = string.Empty;
                if (ClientStateService != null)
                {
                    ClientStateService.ActiveTab = TabOverview;
                    ClientStateService.SelectedClientId = newClientId;
                }
                NavigationManager.NavigateTo($"/Clients/{newClientId}", false);
                await LoadClient(newClientId);
            }
        }
        public List<string> GetClientAndContactPhoneNumbers(Quickfire.Blazor.Domain.Clients.Models.Client? selectedClient)
        {
            var phoneNumbers = new List<string>();

            // Add client's legacy phone number if it exists
            if (selectedClient != null && !string.IsNullOrEmpty(selectedClient.PhoneNumber))
            {
                phoneNumbers.Add(selectedClient.PhoneNumber);
            }

            // Add contact phone numbers
            if (selectedClient?.Contacts != null)
            {
                foreach (var contact in selectedClient.Contacts)
                {
                    // Add primary phone if it exists
                    if (contact.PrimaryPhone != null)
                    {
                        phoneNumbers.Add(contact.PrimaryPhone.Number);
                    }

                    // Add all other phone numbers
                    if (contact.PhoneNumbers != null)
                    {
                        phoneNumbers.AddRange(
                            contact.PhoneNumbers
                                .Where(p => !p.IsPrimary) // Skip primary as it's already added
                                .Select(p => p.Number)
                        );
                    }
                }
            }

            return phoneNumbers.Where(phone => !string.IsNullOrEmpty(phone)).Distinct().ToList();
        }

        // - - - - - User Interface - - - - - -   -   -     -   /
        protected void SetShowCreatePolicy()
        {
            _ = tabInterface.GoToTabAsync(TabPolicies);
            showCreatePolicy = true;
        }
        protected void SetHideCreatePolicy()
        {
            showCreatePolicy = false;
        }
        protected void ExpandDetails(int? forceIt = null)
        {
            if (forceIt == 1)
            {
                dynamicClass = "sf-quicklist";
            }
            else if (forceIt == 0)
            {
                dynamicClass = "sf-quicklist-close";
            }
            else
            {
                dynamicClass = dynamicClass == "sf-quicklist" ? "sf-quicklist-close" : "sf-quicklist";
            }
        }
        protected void UpdatePolicyLists()
        {
            var today = DateTime.Today;

            // Get the updated policies from the client
            var allPolicies = selectedClient?.Policies ?? new List<Policy>();

            // Update currentPolicies and pastPolicies
            currentPolicies = allPolicies
                .Where(p => p.EffectiveDate <= p.ExpirationDate && p.ExpirationDate >= today)
                .OrderByDescending(p => p.EffectiveDate)
                .ToList();

            pastPolicies = allPolicies
                .Where(p => p.ExpirationDate < today)
                .OrderByDescending(p => p.EffectiveDate)
                .ToList();
        }
        protected void OnInputChanged(Microsoft.AspNetCore.Components.ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            _clientListCts?.Cancel(); // Cancel any previous task
            _clientListCts = new CancellationTokenSource();
            var cancellationToken = _clientListCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(150, cancellationToken);

                    await InvokeAsync(async () =>
                    {
                        await FilterClients();
                        StateHasChanged();
                    });
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, safe to ignore
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in OnInputChanged: {ex.Message}");
                }
            }, cancellationToken);
        }
        protected async Task OnActiveTabChanged()
        {
            if (ClientStateService != null)
            {
                await ClientStateService.SaveStateAsync();
            }
        }
        public void showNotesToggle()
        {
            showNotes = !showNotes;
            StateHasChanged();
        }

        protected async Task ShowNewFormDialog()
        {
            // Ensure we're on the Forms tab so the component is rendered
            if (formDocListComponent == null)
            {
                ClientStateService.ActiveTab = "tab-4"; // Forms tab
                await InvokeAsync(StateHasChanged);
                await Task.Delay(50);
            }

            if (formDocListComponent != null)
            {
                await formDocListComponent.OpenNewFormDialog();
            }
        }

        protected async Task CreateNewCertificate()
        {
            if (FormService == null || selectedClient?.ClientId is null or 0)
            {
                return;
            }

            var newCertificateId = await FormService.CreateCertificate(selectedClient.ClientId);
            if (newCertificateId > 0)
            {
                // Allow any calling dialog to close before redirecting
                await InvokeAsync(StateHasChanged);
                await Task.Delay(50);

                NavigationManager.NavigateTo($"/Forms/Certificate/{newCertificateId}");
            }
        }


        // - - - - - Outlook Interop - - - - - -   -   -     -  /
        public async Task OutlookSearchForThisPolicy(string policyNumber)
        {
            var policySearchList = StringHelper.GeneratePolicyVariations(policyNumber);
            await EmberService.RunEmberFunction("OutlookSearch_Policy", policySearchList);
        }
        public async Task OutlookSearchBroad()
        {
            List<string> emailAddresses = GetAllEmailAddresses();
            await EmberService.RunEmberFunction("OutlookSearch_EmailBroad", emailAddresses);
        }
        public async Task OutlookSearchStrict()
        {
            List<string> emailAddresses = GetAllEmailAddresses();
            await EmberService.RunEmberFunction("OutlookSearch_EmailStrictToFrom", emailAddresses);
        }
        public async Task OutlookSearchSmart()
        {
            var policySearchList = StringHelper.GenerateCompanyNameVariations(selectedClient.Name);
            await EmberService.RunEmberFunction("OutlookSearch_SmartSearch", policySearchList);
        }
        public List<string> GetAllEmailAddresses()
        {
            var emailAddresses = new List<string>();

            // Add client's legacy email if it exists
            if (!string.IsNullOrEmpty(selectedClient?.Email))
            {
                emailAddresses.Add(selectedClient.Email);
            }

            // Add all contact email addresses
            if (selectedClient?.Contacts != null)
            {
                foreach (var contact in selectedClient.Contacts)
                {
                    // Add primary email if it exists
                    if (contact.PrimaryEmail != null)
                    {
                        emailAddresses.Add(contact.PrimaryEmail.Email);
                    }

                    // Add all other email addresses
                    if (contact.EmailAddresses != null)
                    {
                        emailAddresses.AddRange(
                            contact.EmailAddresses
                                .Where(e => !e.IsPrimary) // Skip primary as it's already added
                                .Select(e => e.Email)
                        );
                    }
                }
            }

            return emailAddresses.Where(email => !string.IsNullOrEmpty(email)).Distinct().ToList();
        }
        public async Task OutlookNewEmail(string toEmail = null, string subject = null, string body = null)
        {
            if (toEmail != null && subject != null && body != null)
            {
                var myParams = new List<string> { toEmail, subject, body };
                await EmberService.RunEmberFunction("OutlookEmail_CreateNew", myParams);
            }
        }

        // - - - - - Plugins  - - - - - -   -     -        -    /
        protected async Task ForceImportPolicies()
        {
            Console.WriteLine("Forcing policy import...");
            await RunDataSync(true);
        }
        protected async Task RunDataSync(bool forceUpdate)
        {
            utilityStatus = "Epic policy sync has been removed.";
            utilityLoading = false;
            _stateService.UpdateStatus(utilityStatus, false);
            await Task.CompletedTask;
        }

        
        protected async Task UtilImportContacts()
        {
            if (selectedClient == null)
            {
                return;
            }

            utilityStatus = "Epic contact import has been removed.";
            utilityLoading = false;
            _stateService.UpdateStatus(utilityStatus, false);
            await Task.CompletedTask;
        }

        // - - - - - Utilities - - - - - -   -     -        -    /
        protected async Task CreateNewForm(int formPdfId)
        {
            try
            {
                int newFormDocId = await FormService.CreateFormDoc(formPdfId, selectedClient.ClientId);
                NavigationManager.NavigateTo($"/Forms/Editor/{newFormDocId}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating new form: {ex.Message}");
            }
        }
        protected async Task AddLoadedContactsToClient()
        {
            if (selectedClient != null && loadedContacts.Any())
            {
                try
                {
                    await ClientService.AddContactsToClientAsync(selectedClient.ClientId, loadedContacts.ToList());
                    loadedContacts.Clear();
                    contactList.Clear();
                    await LoadClient(selectedClient.ClientId);
                    await tabInterface.GoToTabAsync("tab-1");
                    NavigationManager.NavigateTo($"/Clients/{selectedClient.ClientId}", false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error adding contacts: {ex.Message}");
                }
            }
        }
        private async Task ReloadSelectedClient()
        {
            Console.WriteLine("[CLINT] Reloading selected client and all related data");
            try
            {
                // Re-fetch the client from the database with all related data
                selectedClient = await ClientService.GetClientById(selectedClient.ClientId);

                // Update contact list
                contactList = selectedClient?.Contacts?.ToList() ?? new List<Contact>();

                // Update phone numbers
                phoneNumbers = GetClientAndContactPhoneNumbers(selectedClient);

                // Update policy lists
                UpdatePolicyLists();

                // Update client list if needed
                var updatedClient = clients.FirstOrDefault(c => c.ClientId == selectedClient.ClientId);
                if (updatedClient != null)
                {
                    updatedClient.DateOpened = selectedClient.DateOpened;
                    filteredClients = clients.OrderByDescending(c => c.DateOpened).Take(DefaultListTake).ToList();
                    
                    // Also update the cached client list if it exists
                    if (ClientStateService != null && ClientStateService.AllClients != null)
                    {
                        var cachedClient = ClientStateService.AllClients.FirstOrDefault(c => c.ClientId == selectedClient.ClientId);
                        if (cachedClient != null)
                        {
                            cachedClient.DateOpened = selectedClient.DateOpened;
                            // Save the updated cache
                            await ClientStateService.SaveStateAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating selected client data: {ex.Message}");
            }
        }
        protected void HandleAttachmentAdded()
        {
            Console.WriteLine("Triggering parent refresh after attachment added...");
        }
        
        protected async Task HandleUtilityDataUpdated()
        {
            Console.WriteLine("Utility data updated - reloading client data...");
            await ReloadSelectedClient();
            await InvokeAsync(StateHasChanged);
        }

        // - - - - - Misc - - - -  - -   -      -        -     /
        protected string GetClientSinceDate()
        {
            if (selectedClient?.Policies?.Any() == true)
            {
                var firstPolicy = selectedClient.Policies
                    .OrderBy(p => p.EffectiveDate)
                    .FirstOrDefault();

                if (firstPolicy != null)
                {
                    return firstPolicy.EffectiveDate.ToString("MMM yyyy");
                }
            }

            return selectedClient?.CreatedDate.ToString("MMM yyyy") ?? "Unknown";
        }
        protected int GetTotalPolicyCount()
        {
            return selectedClient?.Policies?.Count ?? 0;
        }
        protected string GetTotalActivePremium()
        {
            if (currentPolicies?.Any() == true)
            {
                var totalPremium = currentPolicies
                    .Where(p => p.Premium > 0)
                    .Sum(p => p.Premium);

                return totalPremium.ToString("C0");
            }

            return "$0";
        }
        public void Dispose()
        {
            _dataSyncCts?.Cancel();
            _clientListCts?.Cancel();
            _dataSyncCts?.Dispose();
            _clientListCts?.Dispose();
            if (_stateService != null)
            {
                _stateService.OnClientUpdated -= HandleClientUpdate;
                if (_stateService.LoadClientFromSearch == LoadClientFromSearchBar)
                {
                    _stateService.LoadClientFromSearch = null;
                }
            }
        }
    }
}
