using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Clients.Services;
using Quickfire.Blazor.Domain.Leads.Models;
using Quickfire.Blazor.Domain.Shared.Services;

namespace Quickfire.Blazor.Domain.Leads.Components
{
    public class LeadActivityLogBase : ComponentBase
    {
        [Parameter] public int LeadId { get; set; }
        [Parameter] public Lead? LeadContext { get; set; }
        [Parameter] public EventCallback<LeadNote> OnActivityAdded { get; set; }

        [Inject] protected ClientService ClientService { get; set; }
        [Inject] protected StateService StateService { get; set; }

        protected List<LeadActivityItemViewModel> ActivityItems { get; private set; } = new();
        protected string NewActivityNote { get; set; } = string.Empty;
        protected bool IsAddingNote { get; private set; }
        protected bool IsLoading { get; private set; }
        protected bool IsAddDisabled => IsAddingNote || LeadId <= 0;
        protected bool IsAddIconDisabled => IsAddDisabled || string.IsNullOrWhiteSpace(NewActivityNote);

        protected override async Task OnParametersSetAsync()
        {
            await RefreshLogAsync();
        }

        public async Task RefreshLogAsync()
        {
            if (LeadId <= 0)
            {
                ActivityItems = new();
                StateHasChanged();
                return;
            }

            IsLoading = true;
            try
            {
                var notes = await ClientService.GetLeadNotesAsync(LeadId) ?? new List<LeadNote>();
                var activities = notes.Select(LeadActivityItemViewModel.FromLeadNote).ToList();

                if (LeadContext != null)
                {
                    var createdBy = LeadContext.CreatedBy != null
                        ? $"{LeadContext.CreatedBy.FirstName} {LeadContext.CreatedBy.LastName}".Trim()
                        : "System";
                    var leadName = !string.IsNullOrWhiteSpace(LeadContext.CompanyName)
                        ? LeadContext.CompanyName
                        : (!string.IsNullOrWhiteSpace(LeadContext.ContactName) ? LeadContext.ContactName : "this lead");

                    activities.Add(LeadActivityItemViewModel.CreateSystemEntry(
                        $"Lead created for <strong>{leadName}</strong>.",
                        LeadContext.CreatedDate,
                        createdBy,
                        entryType: "milestone"));
                }

                ActivityItems = activities
                    .OrderByDescending(a => a.Timestamp)
                    .ToList();
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected async Task AddActivityNote()
        {
            if (IsAddIconDisabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(StateService.CurrentUser?.Id))
            {
                return;
            }

            IsAddingNote = true;
            try
            {
                var newNote = new LeadNote
                {
                    LeadId = LeadId,
                    Note = NewActivityNote.Trim(),
                    DateCreated = DateTime.UtcNow,
                    Deleted = false
                };

                await ClientService.AddLeadNoteAsync(newNote);
                NewActivityNote = string.Empty;

                await RefreshLogAsync();

                if (OnActivityAdded.HasDelegate)
                {
                    await OnActivityAdded.InvokeAsync(newNote);
                }
            }
            finally
            {
                IsAddingNote = false;
            }
        }

        protected async Task OnActivityNoteKeyUp(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await AddActivityNote();
            }
        }
    }
}
