using Surefire.Data;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Services;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Renewals.Services;
using Surefire.Domain.Renewals.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace Surefire.Components.Renewals
{
    public class RenewalsBase : ComponentBase, IDisposable
    {
        [Inject] protected RenewalService RenewalService { get; set; }
        [Inject] protected NavigationManager? RenewalNav { get; set; }
        [Inject] protected StateService _stateService{ get; set; }
        [Parameter] public int renewalId { get; set; }
        [CascadingParameter] public Action<string> UpdateHeader { get; set; }

        protected int htmlYear;
        protected string resultsClass;
        protected string selectedClass;
        protected string htmlMonth = "Disabled";
        protected string htmlUser = "Disabled";
        protected bool jumpTo = false;
        protected bool isLoading = true;
        protected bool dialogHidden = true;
        protected bool isInitiated = false;
        protected bool isParamchanged = false;
        protected FluentTabs renewalTabs;
        protected Renewal renewalDetails;
        protected FluentDialog editGoalDateDialog;
        protected List<TaskItemViewModel> taskList;
        protected List<Carrier> AllCarriers = new();
        protected List<Carrier> AllWholesalers = new();
        protected List<ApplicationUser> AllUsers = new();
       
        protected int EditTrackTaskId { get; set; }
        protected DateTime EditGoalDate { get; set; }
        protected string DownPaymentLink { get; set; }
        protected string FullPaymentLink { get; set; }
        protected decimal DownPaymentAmount { get; set; }
        protected decimal FullPaymentAmount { get; set; }
        protected List<ContactViewModel> ContactOptions { get; set; }
        protected readonly Dictionary<int, CancellationTokenSource> _checkboxDebounceTokens = new();
        protected readonly Dictionary<int, CancellationTokenSource> _notesDebounceTokens = new();


        protected override async Task OnInitializedAsync()
        {
            UpdateHeader?.Invoke("Renewals");

            var renewalDetailsTask = RenewalService.GetRenewalByIdAsync(renewalId);
            var taskListTask = RenewalService.GetTasksForRenewalAsync(renewalId);
            var AllCarriersTask = _stateService.AllCarriers;
            var AllWholesalersTask = _stateService.AllWholesalers;

            await Task.WhenAll(renewalDetailsTask, taskListTask, AllCarriersTask, AllWholesalersTask);

            renewalDetails = await renewalDetailsTask;
            taskList = await taskListTask;
            AllCarriers = await AllCarriersTask;
            AllWholesalers = await AllWholesalersTask;


            // Initialize ContactOptions
            ContactOptions = renewalDetails.Client.Contacts.Select(c => new ContactViewModel { ContactId = c.ContactId, FullName = $"{c.FirstName} {c.LastName}", Email = c.Email }).ToList();

            // Initialize amounts
            DownPaymentAmount = 0;
            FullPaymentAmount = 0;

            if (renewalDetails != null && renewalDetails.AssignedTo != null)
            {
                htmlUser = renewalDetails.AssignedTo.FirstName + " " + renewalDetails.AssignedTo.LastName;
                htmlMonth = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(renewalDetails.RenewalDate.Month);
                htmlMonth += " " + renewalDetails.RenewalDate.Year.ToString();
            }

            //Open the right tab now
            if(renewalTabs != null) await renewalTabs.GoToTabAsync(_stateService.HtmlTab);
            
        }


        //Methods
        protected async Task DebounceSaveNotesAsync(int taskItemId, string newNotes, CancellationToken token)
        {
            try
            {
                await Task.Delay(2000, token);

                if (!token.IsCancellationRequested)
                {
                    await SaveNotesAsync(taskItemId, newNotes);
                    //StateHasChanged();
                }
            }
            catch (TaskCanceledException)
            {
                // Operation was cancelled; no action needed
            }
            finally
            {
                // Clean up the token source
                if (_notesDebounceTokens.TryGetValue(taskItemId, out var cts) && cts.Token == token)
                {
                    _notesDebounceTokens.Remove(taskItemId);
                    cts.Dispose();
                }
            }
        }
        protected async Task SaveNotesAsync(int taskItemId, string newNotes)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskItemId);
            if (task != null)
            {
                task.Notes = newNotes;
                await RenewalService.UpdateTaskNotesAsync(taskItemId, newNotes);

                await InvokeAsync(StateHasChanged);
            }
        }
        protected async Task SaveGoalDate()
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == EditTrackTaskId);
            if (task != null)
            {
                task.TaskGoalDate = EditGoalDate;
                await RenewalService.UpdateTrackTaskModelAsync(task);

                await InvokeAsync(StateHasChanged);
                CancelDialog();
            }
        }
        protected async Task ClearGoalDate()
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == EditTrackTaskId);
            if (task != null)
            {
                task.TaskGoalDate = null;
                await RenewalService.UpdateTrackTaskModelAsync(task);

                await InvokeAsync(StateHasChanged);
                CancelDialog();
            }
        }
        protected async Task UpdateNotepad()
        {
            await RenewalService.UpdateNotepadAsync(renewalDetails);
        }
        protected void OnNotesInputChanged(int taskItemId, string newNotes)
        {
            // Cancel any existing debounce operation for this task
            if (_notesDebounceTokens.TryGetValue(taskItemId, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }

            // Create a new cancellation token source
            var cancellationTokenSource = new CancellationTokenSource();
            _notesDebounceTokens[taskItemId] = cancellationTokenSource;

            // Start the debounce delay
            _ = DebounceSaveNotesAsync(taskItemId, newNotes, cancellationTokenSource.Token);
        }
        protected void CancelDialog()
        {
            dialogHidden = true;
            editGoalDateDialog.Hide(); //Not working?!
        }


        // Row & Context Menu Methods
        protected int activeTaskId;
        protected int menuX, menuY;
        protected bool isMenuOpen;
        protected List<_contextMenu.MenuItem> menuItems = new();

        protected void OpenContextMenu(MouseEventArgs e, int taskId)
        {
            activeTaskId = taskId;                // Set the current task ID
            menuX = ((int)e.ClientX)-66;          // Get the X-coordinate from the mouse event
            menuY = ((int)e.ClientY)-240;         // Get the Y-coordinate from the mouse event
            isMenuOpen = true;                    // Open the menu

            // Define menu items
            menuItems = new List<_contextMenu.MenuItem>
            {
                new() { Text = "Highlight", IconValue = new Icons.Regular.Size24.Lightbulb(), Action = HighlightRow },
                new() { Text = "Edit Goal Date", IconValue = new Icons.Regular.Size24.CalendarEdit(), Action = ShowGoalDateDialog },
                new() { Text = "Assign To Me", IconValue = new Icons.Regular.Size24.PersonArrowLeft(), Action = AssignToMe },
                new() { Text = "Assign To Sub", IconValue = new Icons.Regular.Size24.PersonSupport(), Action = AssignToSub },
                new() { Text = "Edit", IconValue = new Icons.Regular.Size24.PenSparkle(), Action = EditRow },
                new() { Text = "Hide", IconValue = new Icons.Regular.Size24.EyeOff(), Action = HideRow }
            };
        }
        protected void CloseMenu()
        {
            isMenuOpen = false;
            StateHasChanged();
        }
        protected void EditRow(int taskId)
        {
            if (RenewalNav != null)
            {
                RenewalNav.NavigateTo($"/Renewals/EditTrackTask/{taskId}");
            }
        }
        protected string GetRowClass(TaskItemViewModel task)
        {
            var classes = new List<string>();
            if (task.IsHighlighted)
            {
                classes.Add("highlighted-row");
            }
            if (task.IsHidden)
            {
                classes.Add("hidden-row");
            }
            if (task.IsCompleted)
            {
                classes.Add("completed-row");
            }
            return string.Join(" ", classes);
        }
        protected async void HighlightRow(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.IsHighlighted = !task.IsHighlighted;
                await RenewalService.UpdateTaskHighlight(taskId, task.IsHighlighted);
            }
            CloseMenu();
        }
        protected async void ShowGoalDateDialog(int taskId) {
            EditTrackTaskId = taskId;
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            EditGoalDate = task.TaskGoalDate ?? DateTime.Now;
            editGoalDateDialog.Show();
            isMenuOpen = false;
            dialogHidden = false;
            CloseMenu();

        }
        protected async void AssignToMe(int taskId) {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.AssignedSubUser = await RenewalService.AssignToMe(taskId);
            }
            CloseMenu();
        }
        protected async void AssignToSub(int taskId) {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                if (task.AssignedSubUser != null)
                {
                    task.AssignedSubUser = null;
                    await RenewalService.UnassignSub(taskId);
                }
                else
                {
                    task.AssignedSubUser = await RenewalService.AssignToSub(taskId);
                }   
            }
            CloseMenu();
        }
        protected async void HideRow(int taskId) {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.IsHidden = !task.IsHidden;
                task.IsCompleted = true;
                await RenewalService.UpdateTaskHidden(taskId, task.IsHidden);
            }
            CloseMenu();
        }
        protected async Task OnCompletedChanged(int taskItemId, object isChecked)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskItemId);
            if (task != null)
            {
                task.IsCompleted = (bool)isChecked;
                await RenewalService.UpdateTaskCompleted(taskItemId, task.IsCompleted);
                StateHasChanged();
            }
        }


        // Misc
        protected void NavigateToRenewalCreate()
        {
            if (RenewalNav != null)
            {
                RenewalNav.NavigateTo("/Renewals/Create");
            }
        }
        protected void BackToCalendar()
        {
            if (RenewalNav != null)
            {
                RenewalNav.NavigateTo("/Renewals");
            }
        }
        protected class ContactViewModel
        {
            public int ContactId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }
        public void Dispose()
        {
            foreach (var cts in _notesDebounceTokens.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _notesDebounceTokens.Clear();
        }
    }
}