using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Renewals.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Quickfire.Blazor.Domain.Shared.Services;
using System.Threading.Tasks;
using System.Linq;

namespace Quickfire.Blazor.Domain.Renewals.Components
{
    /// <summary>
    /// Base class for the ActivityLog component. Handles loading, adding, and displaying activity notes.
    /// </summary>
    public class ActivityLogBase : ComponentBase
    {
        [Parameter] public int RenewalId { get; set; }
        [Parameter] public EventCallback OnActivityAdded { get; set; }
        [Inject] protected RenewalService RenewalService { get; set; }
        [Inject] protected StateService StateService { get; set; }
        protected List<ActivityItemViewModel> ActivityItems { get; private set; } = new();
        protected string NewActivityNote { get; set; } = string.Empty;
        protected bool IsAddingNote { get; private set; } = false;

        /// <inheritdoc />
        protected override async Task OnParametersSetAsync()
        {
            await RefreshLogAsync();
        }

        public async Task RefreshLogAsync()
        {
            List<RenewalNote> notes = new();
            if (RenewalId > 0)
            {
                // Get ALL notes for this renewal, including those with SubmissionId
                var renewalNotes = await RenewalService.GetRenewalNotesAsync(RenewalId);
                if (renewalNotes != null)
                    notes.AddRange(renewalNotes);
            }
            // No additional filtering: show all notes for this renewal
            notes = notes.OrderByDescending(n => n.DateCreated).ToList();
            ActivityItems = notes.Select(n => new ActivityItemViewModel(n)).ToList();
            StateHasChanged();
        }

        protected async Task AddActivityNote()
        {
            if (string.IsNullOrEmpty(StateService.CurrentUser?.Id))
            {
                return;
            }
            IsAddingNote = true;
            try
            {
                if (RenewalId > 0)
                {
                    var note = new RenewalNote
                    {
                        RenewalId = RenewalId,
                        Note = NewActivityNote.Trim(),
                        DateCreated = System.DateTime.Now,
                        CreatedById = StateService.CurrentUser?.Id,
                        Deleted = false,
                        NoteType = RenewalNoteType.UserTaskNote
                    };
                    await RenewalService.AddRenewalNoteAsync(note);
                }
                NewActivityNote = string.Empty;
                await RefreshLogAsync();
                if (OnActivityAdded.HasDelegate)
                    await OnActivityAdded.InvokeAsync(null);
            }
            finally
            {
                IsAddingNote = false;
            }
        }

        protected async Task OnActivityNoteKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await AddActivityNote();
            }
        }

        // Method to refresh from database (replaces local manipulation)
        public async Task RefreshFromDatabase()
        {
            await RefreshLogAsync();
            StateHasChanged();
        }
    }
}
