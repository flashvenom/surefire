using Microsoft.AspNetCore.Components;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Clients.Services;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Renewals.Services;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Syncfusion.Blazor.Navigations;
using Quickfire.Blazor.Domain.Carriers.Components;

namespace Quickfire.Blazor.Domain.Renewals.Components
{
    public class SubmissionsRedesignedBase : ComponentBase
    {
        [Parameter] public int RenewalId { get; set; } = 0;
        [Parameter] public int LeadId { get; set; } = 0;
        [Inject] protected RenewalService RenewalService { get; set; }
        [Inject] protected ClientService ClientService { get; set; }
        [Inject] protected StateService _stateService { get; set; }
        [Inject] protected ISubmissionService SubmissionService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected AppJsInterop JsInterop { get; set; }

        // Data properties
        protected Renewal? SelectedRenewal { get; set; }
        protected Lead? SelectedLead { get; set; }
        protected List<Submission> SubmissionsList = new();
        protected List<Carrier> AllCarriers = new();
        protected List<Carrier> AllWholesalers = new();
        // Grouped data properties
        protected Dictionary<Carrier, List<Submission>> WholesalerGroups = new();
        protected List<Submission> DirectAppointments = new();
        protected HashSet<int> ExpandedDeclinedSubmissionIds { get; } = new();
        // Dialog properties
        protected bool AddSubmissionDialogHidden = true;
        protected int NewSubmissionCarrierId = 0;
        protected int NewSubmissionWholesalerId = 0;
        protected bool PremiumEditDialogHidden = true;
        protected Submission? SubmissionForPremiumEdit = null;
        // New submission view properties
        protected string CurrentView = "submissions"; // "submissions" or "new-submission"
        protected Carrier? NewSubmissionCarrier = null;
        protected Carrier? NewSubmissionWholesaler = null;
        protected List<Carrier> AllCarriersAndWholesalers = new();
        protected List<Product> AllProducts = new();

        private int _previousRenewalId = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _previousRenewalId = RenewalId;
            await LoadData();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // If the RenewalId parameter has changed, reload the data
            if (RenewalId != _previousRenewalId && RenewalId != 0)
            {
                _previousRenewalId = RenewalId;
                await LoadData();
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            await LoadSubmissions();
            GroupSubmissions();
        }

        private async Task LoadSubmissions()
        {
            if (RenewalId != 0)
            {
                SelectedRenewal = await RenewalService.GetRenewalByIdAsync(RenewalId);
                SubmissionsList = SelectedRenewal.Submissions.ToList();
            }
            else if (LeadId != 0)
            {
                SelectedLead = await ClientService.GetLeadByIdAsync(LeadId);
                SubmissionsList = SelectedLead.Submissions.ToList();
            }

            AllCarriers = await _stateService.AllCarriers;
            AllWholesalers = await _stateService.AllWholesalers;

            // Load combined list for carrier picker
            AllCarriersAndWholesalers = new List<Carrier>();
            AllCarriersAndWholesalers.AddRange(AllCarriers);
            AllCarriersAndWholesalers.AddRange(AllWholesalers);

            // Load products for carrier picker
            AllProducts = await _stateService.AllProducts;
        }

        private void GroupSubmissions()
        {
            WholesalerGroups.Clear();
            DirectAppointments.Clear();

            if (SubmissionsList == null) return;

            foreach (var submission in SubmissionsList)
            {
                // Skip master submissions (no carrier and no wholesaler)
                if (submission.Carrier == null && submission.Wholesaler == null) continue;

                if (submission.Wholesaler != null)
                {
                    // Group by wholesaler - find existing wholesaler or create new group
                    var existingWholesaler = WholesalerGroups.Keys.FirstOrDefault(w => w.CarrierId == submission.Wholesaler.CarrierId);

                    if (existingWholesaler != null)
                    {
                        // Add to existing wholesaler group
                        WholesalerGroups[existingWholesaler].Add(submission);
                    }
                    else
                    {
                        // Create new wholesaler group
                        WholesalerGroups[submission.Wholesaler] = new List<Submission> { submission };
                    }
                }
                else if (submission.Carrier != null)
                {
                    // Direct appointment - each goes into DirectAppointments list
                    DirectAppointments.Add(submission);
                }
            }
        }

        // New submission view methods
        protected void ShowNewSubmissionView()
        {
            CurrentView = "new-submission";
            NewSubmissionCarrier = null;
            NewSubmissionWholesaler = null;
            StateHasChanged();
        }

        protected async Task CreateIncumbentSubmission()
        {
            if (SelectedRenewal == null || SelectedRenewal.CarrierId == null)
            {
                // No incumbent carrier information available
                return;
            }

            try
            {
                var newSubmission = await SubmissionService.CreateNewSubmissionAsync(
                    SelectedRenewal.RenewalId,
                    "renewal",
                    SelectedRenewal.CarrierId.Value,
                    SelectedRenewal.WholesalerId);

                // Add the new submission to the list
                SubmissionsList.Add(newSubmission);
                SelectedRenewal.Submissions.Add(newSubmission);

                // Regroup submissions
                GroupSubmissions();

                // Add activity note
                try
                {
                    var user = _stateService.CurrentUser;
                    var carrierName = newSubmission.Carrier?.CarrierName ?? SelectedRenewal.Carrier?.CarrierName ?? "N/A";
                    var wholesalerName = newSubmission.Wholesaler?.CarrierName ?? SelectedRenewal.Wholesaler?.CarrierName;

                    string noteText;
                    if (!string.IsNullOrEmpty(wholesalerName) && wholesalerName != "N/A")
                    {
                        noteText = $"Incumbent submission for {carrierName} / {wholesalerName} was created.";
                    }
                    else
                    {
                        noteText = $"Incumbent submission for {carrierName} (direct appointment) was created.";
                    }

                    var note = new RenewalNote
                    {
                        RenewalId = SelectedRenewal.RenewalId,
                        SubmissionId = newSubmission.SubmissionId,
                        Note = noteText,
                        DateCreated = DateTime.Now,
                        CreatedById = user?.Id,
                        NoteType = RenewalNoteType.SystemLog
                    };
                    await RenewalService.AddRenewalNoteAsync(note);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding activity note: {ex}");
                }

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating incumbent submission: {ex}");
                // Handle error - show message to user
            }
        }

        protected void HandleCarrierSelected(Carrier selectedCarrier)
        {
            NewSubmissionCarrier = selectedCarrier;
            StateHasChanged();
        }

        protected void HandleWholesalerSelected(Carrier selectedWholesaler)
        {
            NewSubmissionWholesaler = selectedWholesaler;
            StateHasChanged();
        }

        protected async Task SaveNewSubmission()
        {
            if (NewSubmissionCarrier == null) return;

            try
            {
                var parentId = RenewalId != 0 ? RenewalId : LeadId;
                var parentType = RenewalId != 0 ? "renewal" : "lead";

                var newSubmission = await SubmissionService.CreateNewSubmissionAsync(
                    parentId,
                    parentType,
                    NewSubmissionCarrier.CarrierId,
                    NewSubmissionWholesaler?.CarrierId);

                // Add the new submission to the list
                SubmissionsList.Add(newSubmission);

                // Add to the appropriate parent collection
                if (RenewalId != 0 && SelectedRenewal != null)
                    SelectedRenewal.Submissions.Add(newSubmission);
                else if (LeadId != 0 && SelectedLead != null)
                    SelectedLead.Submissions.Add(newSubmission);

                // Regroup submissions
                GroupSubmissions();

                // Add activity note
                try
                {
                    var user = _stateService.CurrentUser;
                    var carrierName = newSubmission.Carrier?.CarrierName ?? "N/A";
                    var wholesalerName = newSubmission.Wholesaler?.CarrierName ?? "N/A";
                    var note = new RenewalNote
                    {
                        RenewalId = newSubmission.Renewal?.RenewalId ?? 0,
                        SubmissionId = newSubmission.SubmissionId,
                        Note = $"Submission for {carrierName} / {wholesalerName} was created using carrier picker.",
                        DateCreated = DateTime.Now,
                        CreatedById = user?.Id,
                        NoteType = RenewalNoteType.SystemLog
                    };
                    await RenewalService.AddRenewalNoteAsync(note);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding activity note: {ex}");
                }

                // Return to submissions view
                CurrentView = "submissions";
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating submission: {ex}");
                // Handle error - show message to user
            }
        }

        protected void CancelNewSubmission()
        {
            CurrentView = "submissions";
            NewSubmissionCarrier = null;
            NewSubmissionWholesaler = null;
            StateHasChanged();
        }

        protected async Task HandleCreateSubmission(SubmissionCreateRequest request)
        {
            try
            {
                var parentId = RenewalId != 0 ? RenewalId : LeadId;
                var parentType = RenewalId != 0 ? "renewal" : "lead";

                var newSubmission = await SubmissionService.CreateNewSubmissionAsync(
                    parentId,
                    parentType,
                    request.CarrierId,
                    request.WholesalerId);

                // Add the new submission to the list
                SubmissionsList.Add(newSubmission);

                // Add to the appropriate parent collection
                if (RenewalId != 0 && SelectedRenewal != null)
                    SelectedRenewal.Submissions.Add(newSubmission);
                else if (LeadId != 0 && SelectedLead != null)
                    SelectedLead.Submissions.Add(newSubmission);

                // Regroup submissions
                GroupSubmissions();

                // Add activity note
                try
                {
                    var user = _stateService.CurrentUser;
                    var carrierName = newSubmission.Carrier?.CarrierName ?? "TBD";
                    var wholesalerName = newSubmission.Wholesaler?.CarrierName ?? "None";
                    var note = new RenewalNote
                    {
                        RenewalId = newSubmission.Renewal?.RenewalId ?? 0,
                        SubmissionId = newSubmission.SubmissionId,
                        Note = request.CarrierId.HasValue
                            ? (request.WholesalerId.HasValue
                                ? $"Submission for {carrierName} via {wholesalerName} was created from carrier picker."
                                : $"Direct submission for {carrierName} was created from carrier picker.")
                            : $"Wholesaler submission (carrier TBD) via {wholesalerName} was created from carrier picker.",
                        DateCreated = DateTime.Now,
                        CreatedById = user?.Id,
                        NoteType = RenewalNoteType.SystemLog
                    };
                    await RenewalService.AddRenewalNoteAsync(note);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding activity note: {ex}");
                }

                // Return to submissions view
                CurrentView = "submissions";
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating submission: {ex}");
            }
        }

        protected async Task OnStepperClicked(StepperClickedEventArgs args, int submissionId)
        {
            var submission = SubmissionsList.FirstOrDefault(s => s.SubmissionId == submissionId);
            if (submission == null) return;

            // Store previous status for activity log
            int previousStatus = submission.StatusInt;

            submission.StatusInt = args.ActiveStep;
            submission.Status = StringHelper.GetSubmissionStatus(args.ActiveStep);

            // Only create a new activity note if status has actually changed
            if (previousStatus != args.ActiveStep)
            {
                try
                {
                    // First update the submission status
                    await RenewalService.UpdateNotesAndPremiumAsync(submission);

                    // Then create a new note without referencing the full entity graph
                    var carrierName = submission.Carrier?.CarrierName ?? "N/A";
                    var wholesalerName = submission.Wholesaler?.CarrierName ?? "N/A";
                    var newNote = new RenewalNote
                    {
                        DateCreated = DateTime.Now,
                        Note = $"{(string.IsNullOrWhiteSpace(wholesalerName) || wholesalerName == "N/A" || wholesalerName == "Other/None" ? carrierName : wholesalerName)} submission status changed to {submission.Status}",
                        SubmissionId = submission.SubmissionId,
                        RenewalId = submission.Renewal?.RenewalId ?? RenewalId,
                        CreatedById = _stateService.CurrentUser?.Id,
                        Deleted = false,
                        NoteType = RenewalNoteType.SubmissionLog
                    };

                    await RenewalService.AddRenewalNoteAsync(newNote);

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating submission status: {ex.Message}");
                    // Restore previous status if there was an error
                    submission.StatusInt = previousStatus;
                    submission.Status = StringHelper.GetSubmissionStatus(previousStatus);
                }
            }
        }

        protected async Task OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var formattedUrl = StringHelper.FormatUrl(url);
                await JsInterop.OpenWindowAsync(formattedUrl, "_blank");
            }
        }

        protected void OpenEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var subject = BuildEmailSubject();
                var encodedSubject = Uri.EscapeDataString(subject);
                var mailtoUrl = $"mailto:{email}?subject={encodedSubject}";
                NavigationManager.NavigateTo(mailtoUrl, true);
            }
        }

        private string BuildEmailSubject()
        {
            var clientName = SelectedRenewal?.Client?.Name ?? "Client";
            var policyNumber = SelectedRenewal?.ExpiringPolicyNumber ?? "TBD";
            var renewalDate = SelectedRenewal?.RenewalDate.ToString("MM/dd/yyyy") ?? "TBD";
            var renewalId = SelectedRenewal?.RenewalId ?? RenewalId;

            return $"{clientName} - {policyNumber} - {renewalDate} [ME00{renewalId}]";
        }

        protected Contact? GetSelectedContact(List<Contact> contacts, int? selectedContactId)
        {
            if (contacts == null || !contacts.Any()) return null;

            if (selectedContactId.HasValue)
            {
                var contact = contacts.FirstOrDefault(c => c.ContactId == selectedContactId.Value);
                if (contact != null) return contact;
            }

            // If no selected contact or selected contact not found, return first contact
            return contacts.FirstOrDefault();
        }

        protected static bool IsSubmissionClosed(Submission submission) => submission?.RejectedStatus != null;

        protected bool ShouldCollapseSubmission(Submission submission) =>
            IsSubmissionClosed(submission) && !ExpandedDeclinedSubmissionIds.Contains(submission.SubmissionId);

        protected string GetSubmissionRowClass(Submission submission)
        {
            var classes = new List<string> { "carrier-submission-row" };

            if (submission == null)
            {
                return string.Join(" ", classes);
            }

            if (ShouldCollapseSubmission(submission))
            {
                classes.Add("collapsed");
            }
            else if (IsSubmissionClosed(submission))
            {
                classes.Add("expanded");
            }

            return string.Join(" ", classes);
        }

        protected void ToggleSubmissionCollapsed(Submission submission)
        {
            if (!IsSubmissionClosed(submission))
            {
                return;
            }

            if (!ExpandedDeclinedSubmissionIds.Add(submission.SubmissionId))
            {
                ExpandedDeclinedSubmissionIds.Remove(submission.SubmissionId);
            }
        }

        // Helper method to determine if a submission is the incumbent carrier
        protected bool IsIncumbentCarrier(Submission submission)
        {
            if (SelectedRenewal == null || submission == null) return false;
            
            // Check if both carrier and wholesaler match (for wholesaler submissions)
            if (submission.Wholesaler != null)
            {
                return submission.Carrier?.CarrierId == SelectedRenewal.CarrierId &&
                       submission.Wholesaler?.CarrierId == SelectedRenewal.WholesalerId;
            }
            // Check if just the carrier matches (for direct appointments)
            else
            {
                return submission.Carrier?.CarrierId == SelectedRenewal.CarrierId &&
                       SelectedRenewal.WholesalerId == null;
            }
        }

        protected async Task ToggleRejectionStatus(Submission submission, RejectedStatus targetStatus)
        {
            if (submission == null) return;

            try
            {
                var previousStatus = submission.RejectedStatus;

                // Toggle logic: if clicking the same status, set to null; otherwise set to clicked status
                if (submission.RejectedStatus == targetStatus)
                {
                    submission.RejectedStatus = null;
                }
                else
                {
                    submission.RejectedStatus = targetStatus;
                }

                ExpandedDeclinedSubmissionIds.Remove(submission.SubmissionId);

                // Update the submission in the database
                await SubmissionService.UpdateSubmissionAsync(submission);

                // Create activity note
                var user = _stateService.CurrentUser;
                var carrierName = submission.Carrier?.CarrierName ?? "N/A";
                var wholesalerName = submission.Wholesaler?.CarrierName ?? "N/A";

                string noteText;
                if (submission.RejectedStatus == null)
                {
                    noteText = $"{(string.IsNullOrWhiteSpace(wholesalerName) || wholesalerName == "N/A" || wholesalerName == "Other/None" ? carrierName : wholesalerName)} rejection status cleared";
                }
                else
                {
                    noteText = $"{(string.IsNullOrWhiteSpace(wholesalerName) || wholesalerName == "N/A" || wholesalerName == "Other/None" ? carrierName : wholesalerName)} submission marked as {submission.RejectedStatus}";
                }

                var note = new RenewalNote
                {
                    RenewalId = submission.Renewal?.RenewalId ?? RenewalId,
                    SubmissionId = submission.SubmissionId,
                    Note = noteText,
                    DateCreated = DateTime.Now,
                    CreatedById = user?.Id,
                    NoteType = RenewalNoteType.SubmissionLog
                };

                await RenewalService.AddRenewalNoteAsync(note);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling rejection status: {ex.Message}");
                // Could add user notification here
            }
        }

        // Premium Edit Dialog methods
        protected void ShowPremiumEditDialog(Submission submission)
        {
            SubmissionForPremiumEdit = submission;
            PremiumEditDialogHidden = false;
        }

        protected async Task HandlePremiumSaved(dynamic premiumEditModel)
        {
            try
            {
                // Update the submission premium
                await SubmissionService.UpdateSubmissionPremiumAsync(premiumEditModel.SubmissionId, premiumEditModel.Amount);

                // Update the local submission object
                var submission = SubmissionsList.FirstOrDefault(s => s.SubmissionId == premiumEditModel.SubmissionId);
                if (submission != null)
                {
                    submission.Premium = premiumEditModel.Amount;
                }

                // Hide dialog first
                PremiumEditDialogHidden = true;
                SubmissionForPremiumEdit = null;

                // Create a note for the premium change
                var note = new RenewalNote
                {
                    Note = $"Premium updated to {premiumEditModel.Amount:C} by user.",
                    NoteType = RenewalNoteType.SubmissionLog,
                    DateCreated = DateTime.UtcNow,
                    SubmissionId = premiumEditModel.SubmissionId,
                    RenewalId = RenewalId != 0 ? RenewalId : 0
                };

                await RenewalService.AddRenewalNoteAsync(note);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating premium: {ex.Message}");
                // Could add user notification here
            }
        }

        protected async Task HandleSubmissionDeleted(int submissionId)
        {
            try
            {
                // Find the submission to delete
                var submission = SubmissionsList.FirstOrDefault(s => s.SubmissionId == submissionId);
                if (submission != null)
                {
                    // Delete from database
                    await SubmissionService.DeleteSubmissionAsync(submissionId);

                    // Remove from local list
                    SubmissionsList.Remove(submission);

                    // Remove from parent collection
                    if (RenewalId != 0 && SelectedRenewal != null)
                        SelectedRenewal.Submissions.Remove(submission);
                    else if (LeadId != 0 && SelectedLead != null)
                        SelectedLead.Submissions.Remove(submission);

                    // Regroup submissions
                    GroupSubmissions();

                    // Create a note for the deletion
                    var carrierName = submission.Carrier?.CarrierName ?? "N/A";
                    var wholesalerName = submission.Wholesaler?.CarrierName ?? "None";
                    var note = new RenewalNote
                    {
                        Note = $"Submission for {carrierName} / {wholesalerName} was deleted.",
                        NoteType = RenewalNoteType.SystemLog,
                        DateCreated = DateTime.UtcNow,
                        RenewalId = RenewalId != 0 ? RenewalId : 0
                    };

                    await RenewalService.AddRenewalNoteAsync(note);
                }

                // Hide dialog
                //SubmissionEditDialogHidden = true;
                //SubmissionForEdit = null;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting submission: {ex.Message}");
                // Could add user notification here
            }
        }
    }
}
