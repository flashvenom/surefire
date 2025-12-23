using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Quickfire.Blazor.Domain.Clients.Models;
using System.Linq;
using System.Collections.Generic;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Shared.Services;

namespace Quickfire.Blazor.Domain.Clients.Services
{
    public class ClientStateService
    {
        private readonly AppJsInterop _jsInterop;
        private const string StateKey = "ClientsPageState";

        // State properties to persist
        public int? SelectedClientId { get; set; }
        public string? SearchTerm { get; set; }
        public string? ActiveTab { get; set; }
        public bool? DisablePlugins { get; set; }
        public List<ClientListItem>? FilteredClients { get; set; }

        // New cached data for rarely changing items
        public List<FormPdf>? AllFormPdfs { get; set; }
        public DateTime? AllFormPdfsLastLoaded { get; set; }
        public List<ClientListItem>? AllClients { get; set; }
        public DateTime? AllClientsLastLoaded { get; set; }

        // Cache duration for rarely changing data (default: 30 minutes)
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ClientStateService(AppJsInterop jsInterop)
        {
            _jsInterop = jsInterop;
        }

        /// <summary>
        /// Check if cached FormPdfs are still valid (not expired)
        /// </summary>
        public bool IsFormPdfsCacheValid()
        {
            return AllFormPdfs != null && 
                   AllFormPdfsLastLoaded.HasValue && 
                   DateTime.UtcNow - AllFormPdfsLastLoaded.Value < CacheDuration;
        }

        /// <summary>
        /// Check if cached Clients are still valid (not expired)
        /// </summary>
        public bool IsClientsCacheValid()
        {
            return AllClients != null && 
                   AllClientsLastLoaded.HasValue && 
                   DateTime.UtcNow - AllClientsLastLoaded.Value < CacheDuration;
        }

        /// <summary>
        /// Set cached FormPdfs with timestamp
        /// </summary>
        public void SetFormPdfsCache(List<FormPdf> formPdfs)
        {
            AllFormPdfs = formPdfs;
            AllFormPdfsLastLoaded = DateTime.UtcNow;
        }

        /// <summary>
        /// Set cached Clients with timestamp
        /// </summary>
        public void SetClientsCache(List<ClientListItem> clients)
        {
            AllClients = clients;
            AllClientsLastLoaded = DateTime.UtcNow;
        }

        /// <summary>
        /// Clear the FormPdfs cache (useful when forms are added/modified)
        /// </summary>
        public async Task InvalidateFormPdfsCacheAsync()
        {
            AllFormPdfs = null;
            AllFormPdfsLastLoaded = null;
            await SaveStateAsync();
        }

        /// <summary>
        /// Clear the Clients cache (useful when clients are added/modified)
        /// </summary>
        public async Task InvalidateClientsCacheAsync()
        {
            AllClients = null;
            AllClientsLastLoaded = null;
            FilteredClients = null; // Also clear filtered clients since they depend on the main list
            await SaveStateAsync();
        }

        public async Task SaveStateAsync()
        {
            var state = new PersistedState
            {
                SelectedClientId = SelectedClientId,
                SearchTerm = SearchTerm,
                ActiveTab = ActiveTab,
                DisablePlugins = DisablePlugins,
                FilteredClients = FilteredClients,
                AllFormPdfs = AllFormPdfs,
                AllFormPdfsLastLoaded = AllFormPdfsLastLoaded,
                AllClients = AllClients,
                AllClientsLastLoaded = AllClientsLastLoaded
            };
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = false
            };
            var json = JsonSerializer.Serialize(state, options);
            await _jsInterop.LocalStorageSetItemAsync(StateKey, json);
        }

        public async Task LoadStateAsync()
        {
            var json = await _jsInterop.LocalStorageGetItemAsync(StateKey);
            if (!string.IsNullOrEmpty(json))
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = false
                };
                var state = JsonSerializer.Deserialize<PersistedState>(json, options);
                if (state != null)
                {
                    SelectedClientId = state.SelectedClientId;
                    SearchTerm = state.SearchTerm;
                    ActiveTab = state.ActiveTab;
                    DisablePlugins = state.DisablePlugins;
                    FilteredClients = state.FilteredClients;
                    AllFormPdfs = state.AllFormPdfs;
                    AllFormPdfsLastLoaded = state.AllFormPdfsLastLoaded;
                    AllClients = state.AllClients;
                    AllClientsLastLoaded = state.AllClientsLastLoaded;
                }
            }
        }

        public async Task ClearStateAsync()
        {
            await _jsInterop.LocalStorageRemoveItemAsync(StateKey);
            SelectedClientId = null;
            SearchTerm = null;
            ActiveTab = null;
            DisablePlugins = null;
            FilteredClients = null;
            AllFormPdfs = null;
            AllFormPdfsLastLoaded = null;
            AllClients = null;
            AllClientsLastLoaded = null;
        }

        /// <summary>
        /// Invalidates the cached client data for a specific client
        /// </summary>
        public async Task InvalidateClientCacheAsync(int clientId)
        {
            // Update the filtered clients list if it exists
            if (FilteredClients != null)
            {
                var clientToUpdate = FilteredClients.FirstOrDefault(c => c.ClientId == clientId);
                if (clientToUpdate != null)
                {
                    // Remove it so it gets refreshed on next load
                    FilteredClients.Remove(clientToUpdate);
                }
            }

            // Update the main clients cache if it exists
            if (AllClients != null)
            {
                var clientToUpdate = AllClients.FirstOrDefault(c => c.ClientId == clientId);
                if (clientToUpdate != null)
                {
                    // Remove it so it gets refreshed on next load
                    AllClients.Remove(clientToUpdate);
                }
            }
            
            // Save the updated state
            await SaveStateAsync();
        }

        /// <summary>
        /// Refreshes the currently selected client data
        /// </summary>
        public async Task RefreshSelectedClientAsync()
        {
            if (SelectedClientId.HasValue)
            {
                await InvalidateClientCacheAsync(SelectedClientId.Value);
            }
        }

        /// <summary>
        /// Clears all client-related caches
        /// </summary>
        public async Task ClearAllClientCachesAsync()
        {
            FilteredClients = null;
            AllClients = null;
            AllClientsLastLoaded = null;
            
            await SaveStateAsync();
        }

        /// <summary>
        /// Clears all caches including forms
        /// </summary>
        public async Task ClearAllCachesAsync()
        {
            await ClearAllClientCachesAsync();
            AllFormPdfs = null;
            AllFormPdfsLastLoaded = null;
            await SaveStateAsync();
        }

        private class PersistedState
        {
            public int? SelectedClientId { get; set; }
            public string? SearchTerm { get; set; }
            public string? ActiveTab { get; set; }
            public bool? DisablePlugins { get; set; }
            public List<ClientListItem>? FilteredClients { get; set; }
            public List<FormPdf>? AllFormPdfs { get; set; }
            public DateTime? AllFormPdfsLastLoaded { get; set; }
            public List<ClientListItem>? AllClients { get; set; }
            public DateTime? AllClientsLastLoaded { get; set; }
        }
    }
}
