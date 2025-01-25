using Microsoft.AspNetCore.Components;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Renewals.Services;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Clients.Services;
using Surefire.Domain.Shared.Services;
using Surefire.Domain.Attachments.Components;
using Surefire.Domain.Attachments.Models;
using Syncfusion.Blazor.Navigations;
using Microsoft.AspNetCore.Components.Web;

namespace Surefire.Components.Shared
{
    public class SubmissionsBase : ComponentBase
    {
        [Parameter] public int RenewalId { get; set; } = 0;
        [Parameter] public int LeadId { get; set; } = 0;
        //
        [Inject] protected RenewalService RenewalService { get; set; }
        [Inject] protected ClientService ClientService { get; set; }
        [Inject] protected StateService _stateService { get; set; }
        [Inject] protected ISubmissionService SubmissionService { get; set; }
        //
        protected Renewal? SelectedRenewal { get; set; }
        protected Lead? SelectedLead { get; set; }
        protected Submission? SelectedSubmission { get; set; }
        protected int PremiumSave { get; set; }
        protected int CarrierSaveId { get; set; }
        protected int WholesalerSaveId { get; set; }
        protected string ViewPanelView { get; set; } = "submissions";
        
        protected string NewSubmissionNoteText { get; set; } = "";
        protected string SType = "undefined";
        protected int SId;
        protected List<Submission> SubmissionsList = new();
        protected List<Carrier> AllCarriers = new();
        protected List<Carrier> AllWholesalers = new();
        protected int SelectedCarrierId;
        protected int SelectedWholsalerId;

        protected override async Task OnInitializedAsync()
        {
            if (RenewalId != 0)
            {
                var renewalDetailsTask = RenewalService.GetRenewalByIdAsync(RenewalId);
                SelectedRenewal = await renewalDetailsTask;
                SubmissionsList = SelectedRenewal.Submissions.ToList();
                SType = "renewal";
                SId = SelectedRenewal.RenewalId;
            }
            else if (LeadId != 0)
            {
                var leadDetailsTask = ClientService.GetLeadByIdAsync(LeadId);
                SelectedLead = await leadDetailsTask;
                SubmissionsList = SelectedLead.Submissions.ToList();
                SType = "lead";
                SId = SelectedLead.LeadId;
            }

            AllCarriers = await _stateService.AllCarriers;
            AllWholesalers = await _stateService.AllWholesalers;
        }

        protected async Task LoadSubmission(int submissionId)
        {
            ViewPanelView = "submissions";
            SelectedSubmission = await SubmissionService.GetSubmissionByIdAsync(submissionId);
            //await InvokeAsync(StateHasChanged);
        }
        protected async Task CreateIncumbantSubmission()
        {
            //Don't have to worry about this one, since leads don't have incumbants
            var newSubmission = await SubmissionService.CreateNewSubmissionAsync(SelectedRenewal.RenewalId, "renewal", SelectedRenewal.CarrierId, SelectedRenewal.WholesalerId);

            // Add the new submission to the renewal's submission list
            SelectedRenewal.Submissions.Add(newSubmission);

            // Load the created submission in the view panel
            ViewPanelView = "submissions";
            await LoadSubmission(newSubmission.SubmissionId);
        }
        protected async Task CreateSubmission()
        {
            var newSubmission = await SubmissionService.CreateNewSubmissionAsync(SId, SType, SelectedCarrierId, SelectedWholsalerId);

            // Add the new submission to the appropriate list
            SubmissionsList.Add(newSubmission);

            if (SType == "renewal" && SelectedRenewal != null)
                SelectedRenewal.Submissions.Add(newSubmission);
            else if (SType == "lead" && SelectedLead != null)
                SelectedLead.Submissions.Add(newSubmission);

            await LoadSubmission(newSubmission.SubmissionId);
            ViewPanelView = "submissions";
            SelectedCarrierId = 0;
            SelectedWholsalerId = 0;

            await InvokeAsync(StateHasChanged);
        }
        protected async Task OnStepperClicked(StepperClickedEventArgs args)
        {
            SelectedSubmission.StatusInt = args.ActiveStep;
            SubmissionsList.FirstOrDefault(x => x.SubmissionId == SelectedSubmission.SubmissionId).StatusInt = args.ActiveStep;
            await SubmissionService.UpdateSubmissionAsync(SelectedSubmission);
        }
        protected async Task DeleteSelectedSubmission()
        { 
            
            ViewPanelView = "summary";
            await SubmissionService.DeleteSubmissionAsync(SelectedSubmission.SubmissionId);
            var removeSubmission = SubmissionsList.Where(x => x.SubmissionId == SelectedSubmission.SubmissionId).FirstOrDefault();
            SubmissionsList.Remove(removeSubmission);
            await InvokeAsync(StateHasChanged);

        }
        protected async Task AddSubmissionNote()
        {
            if (!string.IsNullOrWhiteSpace(NewSubmissionNoteText))
            {
                var newNote = new SubmissionNote
                {
                    Note = NewSubmissionNoteText,
                    DateCreated = DateTime.UtcNow,
                    SubmissionId = SelectedSubmission.SubmissionId,
                    Deleted = false
                };

                await RenewalService.AddSubmissionNoteAsync(newNote);

                // Add the new note to the top of the list
                SelectedSubmission.SubmissionNotes.Insert(0, newNote);

                // Clear the input field
                NewSubmissionNoteText = "";

                await InvokeAsync(StateHasChanged);
            }
        }
        protected async Task SavePremium()
        {
            if (PremiumSave > 0 && PremiumSave != SelectedSubmission.Premium)
            {
                await SubmissionService.UpdateSubmissionPremiumAsync(SelectedSubmission.SubmissionId, PremiumSave);
                SelectedSubmission.Premium = PremiumSave;
                SubmissionsList.Where(x => x.SubmissionId == SelectedSubmission.SubmissionId).FirstOrDefault().Premium = PremiumSave;
                PremiumSave = 0; // Reset after saving
            }
        }
        protected async Task SaveCarrier()
        {
            if (CarrierSaveId > 0 && CarrierSaveId != SelectedSubmission.Carrier?.CarrierId)
            {

                SelectedSubmission.Carrier = await SubmissionService.UpdateSubmissionCarrierAsync(SelectedSubmission.SubmissionId, CarrierSaveId);
                CarrierSaveId = 0; // Reset after saving
                                   //Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }
        protected async Task SaveWholesaler()
        {
            if (WholesalerSaveId > 0 && WholesalerSaveId != SelectedSubmission.Wholesaler?.CarrierId)
            {
                SelectedSubmission.Wholesaler = await SubmissionService.UpdateSubmissionWholesalerAsync(SelectedSubmission.SubmissionId, WholesalerSaveId);
                WholesalerSaveId = 0; // Reset after saving
            }
        }
        protected async Task OnKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await AddSubmissionNote();
            }
        }

        //Navigation
        protected void ShowAddScreen()
        {
            ViewPanelView = "add";
            StateHasChanged();
        }
        protected void ShowSummaryScreen()
        {
            SelectedSubmission = null;
            ViewPanelView = "summary";
            StateHasChanged();
        }
    }
}
