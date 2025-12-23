using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Carriers.Services;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace Quickfire.Blazor.Domain.Carriers.Components
{
    public class CarrierPickerBase : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected StateService StateService { get; set; }
        [Inject] protected CarrierService CarrierService { get; set; }

        // Parameters
        [Parameter] public Carrier? InitialSelectedCarrier { get; set; }
        [Parameter] public EventCallback<Carrier> OnCarrierSelected { get; set; }
        [Parameter] public EventCallback<Carrier> OnWholesalerSelected { get; set; }
        [Parameter] public EventCallback<SubmissionCreateRequest> OnCreateSubmission { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        // Filter Properties
        private string _searchFilter = string.Empty;
        protected string SearchFilter 
        { 
            get => _searchFilter;
            set 
            {
                if (_searchFilter != value)
                {
                    _searchFilter = value;
                    ApplyFilters();
                }
            }
        }
        
        private string _notesSearchFilter = string.Empty;
        protected string NotesSearchFilter 
        { 
            get => _notesSearchFilter;
            set 
            {
                if (_notesSearchFilter != value)
                {
                    _notesSearchFilter = value;
                    ApplyFilters();
                }
            }
        }
        
        protected int? SelectedProductId { get; set; }
        public List<Carrier> AllCarriers { get; set; } = new();
        public List<Product> AllProducts { get; set; } = new();
        protected double MinPremium { get; set; } = 1;
        protected bool NewVenturesOnly { get; set; } = false;
        protected bool ShowAdmitted { get; set; } = true;
        protected bool ShowNonAdmitted { get; set; } = true;
        protected bool PeoOnly { get; set; } = false;
        protected bool? ShowAdmittedOnly { get; set; }
        protected bool? ShowPEOOnly { get; set; }

        // State Properties
        protected List<string> AvailableStates { get; set; } = new() { "CA", "NV", "CO", "NY", "OTHERS" };
        protected Dictionary<string, bool> SelectedStates { get; set; } = new();

        // Data Properties
        protected List<Carrier> FilteredCarriers { get; set; } = new();
        protected Carrier? SelectedCarrier { get; set; }
        protected List<Carrier> WholesalersForCarrier { get; set; } = new();

        // Backing fields for checkbox bindings
        protected bool _admittedFilter = false;
        protected bool _peoFilter = false;

        protected bool AdmittedFilterValue
        {
            get => _admittedFilter;
            set
            {
                if (_admittedFilter != value)
                {
                    _admittedFilter = value;
                    ShowAdmittedOnly = value ? true : null;
                    ApplyFilters();
                }
            }
        }

        protected bool PEOFilterValue
        {
            get => _peoFilter;
            set
            {
                if (_peoFilter != value)
                {
                    _peoFilter = value;
                    ShowPEOOnly = value ? true : null;
                    ApplyFilters();
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            // Initialize state checkboxes (all selected by default)
            foreach (var state in AvailableStates)
            {
                SelectedStates[state] = true;
            }

            // Always fetch a fresh list of carriers from the database
            if (!AllCarriers.Any())
            {
                AllCarriers = await CarrierService.GetAllCarriersAsync();
            }

            if (!AllProducts.Any())
            {
                AllProducts = await StateService.AllProducts;
            }

            // Set initial selected carrier
            SelectedCarrier = InitialSelectedCarrier;

            // Apply initial filters
            ApplyFilters();
        }

        protected async void SelectCarrier(Carrier carrier)
        {
            Console.WriteLine("Selected carrier: " + carrier.CarrierName);
            SelectedCarrier = FilteredCarriers.FirstOrDefault(c => c.CarrierId == carrier.CarrierId);
            
            // Load wholesalers that have access to this carrier if it's an issuing carrier
            if (SelectedCarrier != null && SelectedCarrier.IssuingCarrier && !SelectedCarrier.Wholesaler)
            {
                WholesalersForCarrier = await CarrierService.GetWholesalersForCarrierAsync(SelectedCarrier.CarrierId);
            }
            else
            {
                WholesalersForCarrier.Clear();
            }
            
            StateHasChanged();
        }

        protected void OnProductChanged(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int?, Product> args)
        {
            SelectedProductId = args.Value;
            ApplyFilters();
        }

        protected void ClearProductFilter()
        {
            SelectedProductId = null;
            ApplyFilters();
        }

        protected void OnSearchInput(Microsoft.AspNetCore.Components.ChangeEventArgs e)
        {
            SearchFilter = e?.Value?.ToString() ?? string.Empty;
        }

        protected void OnNotesSearchInput(Microsoft.AspNetCore.Components.ChangeEventArgs e)
        {
            NotesSearchFilter = e?.Value?.ToString() ?? string.Empty;
        }

        protected void ClearAllFilters()
        {
            SearchFilter = string.Empty;
            NotesSearchFilter = string.Empty;
            SelectedProductId = null;
            ShowAdmittedOnly = null;
            ShowPEOOnly = null;
            _admittedFilter = false;
            _peoFilter = false;
            ApplyFilters();
        }

        protected void ApplyFilters()
        {
            var filteredCarriers = AllCarriers.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchFilter))
            {
                filteredCarriers = filteredCarriers.Where(carrier =>
                    (carrier.CarrierName != null && carrier.CarrierName.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(carrier.CarrierNickname) && carrier.CarrierNickname.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Apply notes search filter
            if (!string.IsNullOrEmpty(NotesSearchFilter))
            {
                filteredCarriers = filteredCarriers.Where(carrier =>
                    !string.IsNullOrEmpty(carrier.Notes) && 
                    carrier.Notes.Contains(NotesSearchFilter, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Apply product filter
            if (SelectedProductId.HasValue)
            {
                filteredCarriers = filteredCarriers.Where(carrier =>
                    carrier.CarrierProducts?.Any(cp => 
                        cp.ProductId == SelectedProductId.Value && 
                        cp.IsActive) == true
                );
            }

            // Apply admitted filter
            if (ShowAdmittedOnly.HasValue)
            {
                filteredCarriers = filteredCarriers.Where(carrier =>
                    carrier.Admitted == ShowAdmittedOnly.Value
                );
            }

            // Apply PEO filter
            if (ShowPEOOnly.HasValue)
            {
                filteredCarriers = filteredCarriers.Where(carrier =>
                    carrier.IsPEO == ShowPEOOnly.Value
                );
            }

            FilteredCarriers = filteredCarriers
                .OrderByDescending(c => c.IsFavorite)
                .ThenBy(c => c.CarrierName)
                .ToList();
            
            // Ensure selectedCarrier is in the filtered list, if not clear selection
            if (SelectedCarrier != null && !FilteredCarriers.Any(c => c.CarrierId == SelectedCarrier.CarrierId))
            {
                SelectedCarrier = null;
            }
            
            StateHasChanged();
        }

        protected async Task ToggleFavorite(Carrier carrier)
        {
            try
            {
                var newValue = !carrier.IsFavorite;
                await CarrierService.UpdateCarrierFavoriteAsync(carrier.CarrierId, newValue);
                // Update local state
                var target = AllCarriers.FirstOrDefault(c => c.CarrierId == carrier.CarrierId);
                if (target != null)
                {
                    target.IsFavorite = newValue;
                }
                if (SelectedCarrier?.CarrierId == carrier.CarrierId)
                {
                    SelectedCarrier.IsFavorite = newValue;
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed updating favorite: {ex.Message}");
            }
        }

        protected bool CarrierHasProduct(Carrier carrier, int productId)
        {
            return carrier.CarrierProducts?.Any(cp => 
                cp.ProductId == productId && 
                cp.IsActive) == true;
        }

        protected bool CarrierIsSpecialtyFor(Carrier carrier, int productId)
        {
            return carrier.CarrierProducts?.Any(cp => 
                cp.ProductId == productId && 
                cp.IsActive && 
                cp.ProductSpecialty) == true;
        }

        protected bool HasActiveFilters()
        {
            return !string.IsNullOrEmpty(SearchFilter) ||
                   !string.IsNullOrEmpty(NotesSearchFilter) ||
                   SelectedProductId.HasValue ||
                   ShowAdmittedOnly.HasValue ||
                   ShowPEOOnly.HasValue;
        }

        protected async Task OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                // Ensure URL has protocol
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                await JSRuntime.InvokeAsync<object>("open", url, "_blank");
            }
        }
    }

    public class SubmissionCreateRequest
    {
        public int? CarrierId { get; set; }
        public int? WholesalerId { get; set; }
    }
} 