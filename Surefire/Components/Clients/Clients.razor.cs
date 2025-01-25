using Surefire.Domain.Logs;
using Surefire.Domain.Ember;
using Surefire.Domain.Plugins;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Forms.Services;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Clients.Services;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Shared.Helpers;
using Surefire.Domain.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Surefire.Components.Clients
{
    public class ClientsBase : ComponentBase, IDisposable
    {
        [Inject] protected StateService? _stateService { get; set; }
        [Inject] protected FormService? FormService { get; set; }
        [Inject] protected ClientService? ClientService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected EmberService? EmberService { get; set; }
        [Inject] protected PluginManager? _plugin { get; set; }
        [Inject] protected ILoggingService? _logs { get; set; }

        [Parameter, SupplyParameterFromQuery]
        public int LoadClientId { get; set; }

        [Parameter]
        public bool showCreatePolicy { get; set; } = false;

        [CascadingParameter] public Action<string>? UpdateHeader { get; set; }

        // Client Properties
        protected Client? selectedClient;
        protected List<ClientListItem> clients = new();
        protected List<ClientListItem> filteredClients = new();
        // Search
        protected string searchTerm = string.Empty;
        // Related Goods
        protected List<Policy> currentPolicies = new();
        protected List<Policy> pastPolicies = new();
        protected List<Contact> primaryContactList = new();
        protected List<Contact> secondaryContactList = new();
        protected List<string> phoneNumbers = new();
        protected List<Contact> loadedContacts = new();
        protected List<FormPdf> allFormPdfs = new();
        // UI State
        protected bool isLoading = false;
        protected string dynamicClass = "sf-quicklist-close";
        protected FluentTabs? tabInterface;
        private CancellationTokenSource? _dataSyncCts;
        private CancellationTokenSource? _clientListCts;
        // Utility
        protected bool utilityLoading = false;
        protected string utilityStatus = "";


        // - - - - - OnInit - - - - - - /
        protected override async Task OnInitializedAsync()
        {
            UpdateHeader?.Invoke("Clients");
            _stateService.LoadClientFromSearch = LoadClientFromSearchBar;

            var formPdfsTask = FormService?.GetAllFormPdfs();
            var clientsTask = ClientService?.GetClientListAsync();
            await Task.WhenAll(formPdfsTask, clientsTask);
            allFormPdfs = await formPdfsTask;
            clients = await clientsTask;
            filteredClients = clients.Take(50).ToList();
            
            //Check if database is empty
            if(clients.Count == 0)
            {
                NavigationManager.NavigateTo($"/Clients/Create", false);
            }
            else
            {
                await LoadClient(LoadClientId);
            }
        }

        // - - Main Methods
        private async Task LoadClient(int loadClientId)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                selectedClient = await ClientService.GetClientById(loadClientId) ??
                 await ClientService.GetClientById(clients.FirstOrDefault()?.ClientId ?? 0);


                // Create the contact lists
                if (selectedClient?.PrimaryContact != null)
                {
                    primaryContactList = new List<Contact> { selectedClient.PrimaryContact };
                }
                secondaryContactList = selectedClient?.Contacts
                    .Where(c => c.ContactId != selectedClient.PrimaryContact?.ContactId)
                    .ToList() ?? new List<Contact>();

                // Sort the policies to current and past lists
                var today = DateTime.Today;
                currentPolicies = selectedClient?.Policies
                    .Where(p => p.EffectiveDate <= p.ExpirationDate && p.ExpirationDate >= today)
                    .OrderByDescending(p => p.EffectiveDate)
                    .ToList() ?? new List<Policy>();

                pastPolicies = selectedClient?.Policies
                    .Where(p => p.ExpirationDate < today)
                    .OrderByDescending(p => p.EffectiveDate)
                    .ToList() ?? new List<Policy>();

                searchTerm = string.Empty;
                phoneNumbers = GetClientAndContactPhoneNumbers(selectedClient);
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
                Console.WriteLine("Making new filter list for clients");
                filteredClients = clients.Take(40).ToList();
            }
            else
            {
                Console.WriteLine("Filtering clients");
                filteredClients = clients
                    .Where(client => client.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Take(20)
                    .ToList();
            }
        }
        protected async Task LoadClientClickHandler(int clientId)
        {
            ExpandDetails(0);
            searchTerm = string.Empty;
            _stateService.ClientId = clientId;
            // NavigationService.SetLastClientPage($"/Clients/{clientId}");
            NavigationManager.NavigateTo($"/Clients/{clientId}", false);
            await LoadClient(clientId);
        }
        public async Task LoadClientFromSearchBar(int newClientId)
        {
            if (newClientId != selectedClient.ClientId)
            {
                ExpandDetails(0);
                searchTerm = string.Empty;
                _stateService.ClientId = newClientId;
                NavigationManager.NavigateTo($"/Clients/{newClientId}", false);
                await LoadClient(newClientId);
            }
        }

        // - - User Interface
        protected void SetShowCreatePolicy()
        {
            _ = tabInterface.GoToTabAsync("tab-2");
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
        private void UpdatePolicyLists()
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
                    // Debounce delay
                    await Task.Delay(150, cancellationToken);

                    // Perform filtering operation
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

        //Outlook Interop SmartSearches
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
        public List<string> GetAllEmailAddresses() =>
            (selectedClient?.Email != null ? new[] { selectedClient.Email } : Enumerable.Empty<string>())
            .Concat(primaryContactList?.Select(c => c.Email) ?? Enumerable.Empty<string>())
            .Concat(secondaryContactList?.Select(c => c.Email) ?? Enumerable.Empty<string>())
            .Where(email => !string.IsNullOrEmpty(email))
            .ToList();

        // - - Plugins
        protected async Task ForceImportPolicies()
        {
            Console.WriteLine("Forcing policy import...");
            await RunDataSync(true);
        }
        private async Task RunDataSync(bool forceUpdate)
        {
            _dataSyncCts?.Cancel(); // Cancel any previous task
            _dataSyncCts = new CancellationTokenSource();
            var cancellationToken = _dataSyncCts.Token;

            try
            {
                utilityStatus = "Syncing Policies...";
                utilityLoading = true;

                var plugin = _plugin.GetPluginByName("Applied Epic Cloud API");
                if (plugin == null || !plugin.IsActive)
                {
                    Console.Error.WriteLine("DataSync plugin not found or is inactive.");
                    utilityStatus = "Sync plugin not available.";
                    utilityLoading = false;
                    return;
                }

                _stateService.UpdateStatus(utilityStatus, true);
                if(selectedClient == null)
                {
                    NavigationManager.NavigateTo("/");
                }
                // Execute the DataSync method on the plugin
                var response = await plugin.ExecuteAsync("DataSync", new object[] { selectedClient.ClientId, forceUpdate }, cancellationToken);

                if (response?.success == true && response.cleanup == true)
                {
                    await ReloadSelectedClient();
                }
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine("DataSync operation canceled.");
            }
            catch (Exception ex)
            {
                await _logs.LogAsync(LogLevel.Error, ex.Message, "Clients.razor - RunDataSync");
                Console.Error.WriteLine($"Error running DataSync plugins: {ex.Message}");
            }
            finally
            {
                // Update the last opened date
                await ClientService.UpdateLastOpenedAsync(selectedClient.ClientId, DateTime.Now);
                selectedClient.DateOpened = DateTime.Now;
                filteredClients = clients.OrderByDescending(c => c.DateOpened).Take(40).ToList();

                // Update UI after completion
                UpdatePolicyLists();

                utilityStatus = "";
                utilityLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        protected async Task UtilImportContacts()
        {
            utilityStatus = "Grabbing contacts from DataSync API...";
            utilityLoading = true;
            _stateService.UpdateStatus(utilityStatus, true);
            try
            {
                Console.WriteLine("Looking for DataSync Plugins...");

                //Looks for and runs any plugins with a GetContacts type and  method
                //var responses = await _stateService.RunPluginMethodAsync<IDataSyncPlugin>(async plugin => await plugin.GetContacts(selectedClient.eClientId));
                var responses = await _stateService.RunPluginMethodAsync("GetContacts", new object[] { selectedClient.eClientId }, CancellationToken.None);

                if (responses.FirstOrDefault() != null)
                {
                    if (responses.FirstOrDefault()?.success == true && responses.FirstOrDefault()?.contacts.Count > 0)
                    {
                        foreach (var contactItem in responses.FirstOrDefault()?.contacts)
                        {
                            loadedContacts.Add(contactItem);
                        }
                    }
                    utilityStatus = responses.FirstOrDefault()?.message;
                }

            }
            catch (Exception ex)
            {
                await _logs.LogAsync(LogLevel.Error, ex.Message, "Clients.razor - GetContacts");
                Console.Error.WriteLine($"Error running GetContacts plugins: {ex.Message}");
                utilityStatus = "Error running GetContacts plugins...";
                utilityLoading = false;
            }
            utilityLoading = false;
            _stateService.UpdateStatus(utilityStatus, false);
        }

        // - - Utilities
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
                    primaryContactList.Clear();
                    secondaryContactList.Clear();
                    await LoadClient(selectedClient.ClientId);
                    await tabInterface.GoToTabAsync("tab-1");
                    NavigationManager.NavigateTo($"/Clients/{selectedClient.ClientId}", false);
                    await InvokeAsync(StateHasChanged); // Update the UI
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error adding contacts: {ex.Message}");
                }
            }
        }
        private async Task ReloadSelectedClient()
        {
            Console.WriteLine("Reloading selected");
            try
            {
                // Re-fetch the client from the database to get updated policies
                selectedClient = await ClientService.GetClientById(selectedClient.ClientId);

                // Update contact lists if necessary
                if (selectedClient?.PrimaryContact != null)
                {
                    primaryContactList = new List<Contact> { selectedClient.PrimaryContact };
                }

                secondaryContactList = selectedClient?.Contacts
                    .Where(c => c.ContactId != selectedClient.PrimaryContact?.ContactId)
                    .ToList() ?? new List<Contact>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating selected client policies: {ex.Message}");
            }
        }
        protected async void HandleAttachmentAdded()
        {
            // Trigger re-render to refresh child components
            Console.WriteLine("Triggering parent refresh after attachment added...");
            //await LoadClient(selectedClient.ClientId);
        }
        public List<string> GetClientAndContactPhoneNumbers(Client selectedClient)
        {
            List<string> phoneNumbers = new List<string>();

            // Add client phone number
            if (selectedClient != null)
            {
                if (!string.IsNullOrEmpty(selectedClient.PhoneNumber))
                {
                    phoneNumbers.Add("+1" + selectedClient.PhoneNumber);
                }
            }

            // Add contact phone numbers
            if (selectedClient != null && selectedClient.Contacts != null)
            {
                foreach (var contact in selectedClient.Contacts)
                {
                    if (!string.IsNullOrEmpty(contact.Phone))
                    {
                        phoneNumbers.Add("+1" + contact.Phone);
                    }
                    if (!string.IsNullOrEmpty(contact.Mobile))
                    {
                        phoneNumbers.Add("+1" + contact.Mobile);
                    }
                }
            }

            return phoneNumbers;
        }
        public void Dispose()
        {
            _dataSyncCts?.Cancel();
            _clientListCts?.Cancel();
            _dataSyncCts?.Dispose();
            _clientListCts?.Dispose();
        }
    }
}
