using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models; // For ActivityItemViewModel
using Quickfire.Blazor.Domain.Renewals.Components;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Renewals.Services;
using Quickfire.Blazor.Domain.Renewals.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Syncfusion.Blazor.DropDowns;


namespace Quickfire.Blazor.Domain.Renewals.Components
{
    public class RenewalsBase : ComponentBase, IDisposable
    {
        [Inject] protected RenewalService RenewalService { get; set; }
        [Inject] protected TaskService TaskService { get; set; }
        [Inject] protected NavigationManager RenewalNav { get; set; }
        [Inject] protected StateService _stateService { get; set; }
        [Inject] protected AppJsInterop JsInterop { get; set; }
        [Parameter] public int renewalId { get; set; }
        [CascadingParameter] public Action<string> UpdateHeader { get; set; }

        // Track previous renewal ID to detect changes
        private int _previousRenewalId;
        protected string resultsClass;
        protected string selectedClass;
        protected string htmlMonth = "Disabled";
        protected string htmlUser = "Disabled";
        protected string noteAnchorId = "task-note-0";
        protected string currentTaskDescription = string.Empty;
        protected bool activityLogToggle = true;
        protected bool _noteVis;
        protected bool jumpTo = false;
        protected bool isLoading = true;
        protected bool dialogHidden = true;
        protected bool isInitiated = false;
        protected bool isParamchanged = false;
        protected bool isUserSubmenuOpen = false; //Submenu
        protected int userSubmenuX, userSubmenuY; //Submenu
        protected int userSubmenuTaskId; //Submenu
        protected int htmlYear;
        protected int activeTaskId; // Row Context
        protected int menuX, menuY; // Row Context
        protected bool isMenuOpen; // Row Context
        protected List<_contextMenu.MenuItem> menuItems = new();
        protected List<_contextMenu.ToolbarItem> menuToolbarItems = new();
        protected FluentTabs renewalTabs;
        protected Renewal renewalDetails;
        protected FluentDialog editGoalDateDialog;
        protected List<TaskItemViewModel> taskList;
        protected List<Carrier> AllCarriers = new();
        protected List<Carrier> AllWholesalers = new();
        protected List<ApplicationUser> AllUsers = new();
        protected readonly Dictionary<int, CancellationTokenSource> _checkboxDebounceTokens = new();
        protected readonly Dictionary<int, CancellationTokenSource> _notesDebounceTokens = new();
        protected ActivityLog activityLogComponent;
        protected int EditTrackTaskId { get; set; }
        protected DateTime EditGoalDate { get; set; }
        protected decimal DownPaymentAmount { get; set; }
        protected decimal FullPaymentAmount { get; set; }
        protected List<ContactViewModel> ContactOptions { get; set; }
        protected bool ShowSubtaskDialog { get; set; } = false;
        protected bool CanNavigateToNext => CurrentRenewalIndex < _stateService.RenewalList?.Count - 1; // Prev-Next Btns
        protected bool CanNavigateToPrevious => CurrentRenewalIndex > 0; // Prev-Next Btns
        protected int CurrentRenewalIndex { get; set; } = -1; // Prev-Next Btns
        protected int? SelectedParentTaskId { get; set; }
        protected Dictionary<int, int> SubtaskCounts { get; set; } = new();
        protected Dictionary<int, int> SubtaskCompletedCounts { get; set; } = new();
        protected int? SubtaskDialogParentId { get; set; }
        //
        //
        // Handler for subtask activity log refresh
        protected async Task OnSubtaskActivityAdded()
        {
            if (activityLogComponent != null)
                await activityLogComponent.RefreshLogAsync();
            await LoadSubtaskCounts();
            StateHasChanged();
        }
        protected void ToggleActivityLog()
        {
            activityLogToggle = !activityLogToggle;
            StateHasChanged();
        }
        // Main Load Logic -----------------------------------------------//
        protected override async Task OnInitializedAsync()
        {
            _previousRenewalId = renewalId;
            _stateService.LoadRenewalFromSearch = UpdateDataWithNewRenewalId;

            // Ensure we have a default tab if none is set
            if (string.IsNullOrEmpty(_stateService.HtmlTab))
            {
                _stateService.HtmlTab = "tab-1";
            }

            await LoadDataForCurrentRenewal();
            await LoadSubtaskCounts();
        }

        public async Task UpdateDataWithNewRenewalId(int newRenewalId)
        {
            _stateService.HtmlSubTaskId = 0;
            if (newRenewalId != renewalId)
            {
                _stateService.HtmlRenewalId = newRenewalId;
                renewalId = newRenewalId; // Update renewalId BEFORE loading data
                RenewalNav.NavigateTo($"/Renewals/Details/{newRenewalId}", false);
                await LoadDataForCurrentRenewal();
                await LoadSubtaskCounts(); // Also reload subtask counts for the new renewal
                StateHasChanged(); // Trigger UI update
            }
        }

        private async Task LoadDataForCurrentRenewal()
        {
            //Console.WriteLine($"Loading renewalid: {renewalId}");
            // Set StateService variables if any are in the url
            var uri = RenewalNav?.ToAbsoluteUri(RenewalNav.Uri);
            if (uri != null && QueryHelpers.ParseQuery(uri.Query).Count > 0)
            {
                var queryParams = QueryHelpers.ParseQuery(uri.Query);

                if (queryParams.TryGetValue("month", out var monthParam) && int.TryParse(monthParam, out var month)) { _stateService.HtmlMonth = month; }

                if (queryParams.TryGetValue("year", out var yearParam) && int.TryParse(yearParam, out var year)) { _stateService.HtmlYear = year; }

                if (queryParams.TryGetValue("user", out var userParam))
                {
                    if (string.IsNullOrEmpty(_stateService.HtmlUser))
                    {
                        _stateService.HtmlUser = "Everyone";
                    }
                }
            }

            // Get the renewallist to cycle through using the prev/next buttons
            if (_stateService.RenewalList == null || !_stateService.RenewalList.Any())
            {
                if (_stateService.HtmlMonth == 0)
                {
                    _stateService.HtmlMonth = DateTime.Now.Month; // If no month/year set, use current date
                    _stateService.HtmlYear = DateTime.Now.Year;
                }

                if (string.IsNullOrEmpty(_stateService.HtmlUser)) { _stateService.HtmlUser = "Everyone"; }

                _stateService.RenewalList = await RenewalService.GetFilteredRenewalListAsync(_stateService.HtmlMonth, _stateService.HtmlYear, _stateService.HtmlUser);
            }

            // Get the current renewal's index in the list
            if (_stateService.RenewalList != null && _stateService.RenewalList.Any())
            {
                CurrentRenewalIndex = _stateService.RenewalList.FindIndex(r => r.RenewalId == renewalId);
            }

            // Load data for the current renewal
            isLoading = true;
            StateHasChanged(); // Ensure UI updates to show loading spinner
            var renewalDetailsTask = RenewalService.GetRenewalByIdAsync(renewalId);
            var taskListTask = TaskService.GetTasksAsListForRenewalAsync(renewalId);
            var AllCarriersTask = _stateService.AllCarriers;
            var AllWholesalersTask = _stateService.AllWholesalers;
            var AllUsersTask = _stateService.AllUsers;

            await Task.WhenAll(renewalDetailsTask, taskListTask, AllCarriersTask, AllWholesalersTask, AllUsersTask);

            renewalDetails = await renewalDetailsTask;
            taskList = await taskListTask;
            AllCarriers = await AllCarriersTask;
            AllWholesalers = await AllWholesalersTask;
            AllUsers = await AllUsersTask;

            // Filter to only parent tasks
            if (taskList != null)
            {
                taskList = taskList.Where(t => t.ParentTaskId == null).ToList();
            }

            // Auto-select the first uncompleted parent task
            if (taskList != null && taskList.Any())
            {
                // If no task is currently selected OR the previously selected task doesn't exist in the new list, pick the first un-completed one.
                if (!SelectedParentTaskId.HasValue || !taskList.Any(t => t.TaskItemId == SelectedParentTaskId.Value))
                {
                    var firstUncompletedTask = taskList.FirstOrDefault(t => !t.IsCompleted) ?? taskList.First();
                    SelectedParentTaskId = firstUncompletedTask?.TaskItemId;
                }
            }

            // Initialize ContactOptions
            if (renewalDetails?.Client?.Contacts != null)
            {
                ContactOptions = renewalDetails.Client.Contacts.Select(c => new ContactViewModel
                {
                    ContactId = c.ContactId,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.EmailAddresses.FirstOrDefault(e => e.IsPrimary)?.Email ?? c.EmailAddresses.FirstOrDefault()?.Email ?? string.Empty
                }).ToList();
            }
            else
            {
                ContactOptions = new List<ContactViewModel>();
            }

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
            if (renewalTabs != null) await renewalTabs.GoToTabAsync(_stateService.HtmlTab);
            if (_stateService.HtmlTab == "tab-1" && _stateService.HtmlSubTaskId != 0 && _stateService.HtmlRenewalId == renewalId)
            {
                OnSubtaskCountClicked(_stateService.HtmlSubTaskId);
            }

            //Console.WriteLine($"Renewal data #{renewalId} is done loading...");
            isLoading = false;
            StateHasChanged(); // Ensure UI updates to hide loading spinner
        }


        // Save Methods
        protected async Task SaveGoalDate()
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == EditTrackTaskId);
            if (task != null)
            {
                task.TaskGoalDate = EditGoalDate;
                await TaskService.UpdateTrackTaskModelAsync(task);
                // Add activity note
                var user = _stateService.CurrentUser;
                var userName = user != null ? user.FirstName : "System";
                var note = new RenewalNote
                {
                    RenewalId = renewalId,
                    Note = $"Goal date for <strong>{task.TaskItemName}</strong> set to {EditGoalDate:MM/dd/yyyy}.",
                    DateCreated = DateTime.Now,
                    CreatedById = user?.Id ?? "System",
                    NoteType = RenewalNoteType.SystemLog
                };
                await RenewalService.AddRenewalNoteAsync(note);
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
                await TaskService.UpdateTrackTaskModelAsync(task);
                // Add activity note
                var user = _stateService.CurrentUser;
                var userName = user != null ? user.FirstName : "System";
                var note = new RenewalNote
                {
                    RenewalId = renewalId,
                    Note = $"Goal date for <strong>{task.TaskItemName}</strong> was cleared.",
                    DateCreated = DateTime.Now,
                    CreatedById = user?.Id ?? "System",
                    NoteType = RenewalNoteType.SystemLog
                };
                await RenewalService.AddRenewalNoteAsync(note);
                await InvokeAsync(StateHasChanged);
                CancelDialog();
            }
        }
        protected async Task UpdateNotepad()
        {
            await RenewalService.UpdateNotepadAsync(renewalDetails);
        }
        protected void CancelDialog()
        {
            dialogHidden = true;
            editGoalDateDialog.Hide(); //Not working?!
        }


        // Row & Context Menu Methods
        protected void OpenContextMenu(MouseEventArgs e, int taskId)
        {
            activeTaskId = taskId;                // Set the current task ID
            menuX = ((int)e.ClientX) - 66;          // Get the X-coordinate from the mouse event
            menuY = ((int)e.ClientY) - 240;         // Get the Y-coordinate from the mouse event
            isMenuOpen = true;                    // Open the menu
            isUserSubmenuOpen = false;            // Close user submenu if open

            // Define toolbar items for quick goal date actions
            menuToolbarItems = new List<_contextMenu.ToolbarItem>
            {
                new() { Text = "Clear Goal Date", IconValue = new Icons.Regular.Size24.CalendarEmpty(), Action = ClearTaskGoalDate },
                new() { Text = "Set Goal Date to Tomorrow", IconValue = new Icons.Regular.Size24.CalendarWeekNumbers(), Action = SetTaskGoalDateTomorrow },
                new() { Text = "Set Goal Date to A Week from Today", IconValue = new Icons.Regular.Size24.CalendarWeekStart(), Action = SetTaskGoalDateNextWeek },
                new() { Text = "Set Goal Date to Two Weeks from Today", IconValue = new Icons.Regular.Size24.CalendarArrowCounterclockwise(), Action = SetTaskGoalDateTwoWeeks }
            };

            // Define menu items
            menuItems = new List<_contextMenu.MenuItem>
            {
                new() { Text = "Highlight", IconValue = new Icons.Regular.Size24.Lightbulb(), Action = HighlightRow },
                new() { Text = "Edit Goal Date", IconValue = new Icons.Regular.Size24.CalendarEdit(), Action = ShowGoalDateDialog },
                new() { Text = "Assign To Me", IconValue = new Icons.Regular.Size24.PersonArrowLeft(), Action = AssignToMe },
                new() { Text = "Assign To...", IconValue = new Icons.Regular.Size24.PersonSupport(), Action = ShowUserSubmenu },
                new() { Text = "Edit", IconValue = new Icons.Regular.Size24.PenSparkle(), Action = EditRow },
                new() { Text = "Hide", IconValue = new Icons.Regular.Size24.EyeOff(), Action = HideRow }
            };
        }
        protected void ShowUserSubmenu(int taskId)
        {
            userSubmenuTaskId = taskId;

            // Place it next to the main menu, aligned with the "Assign To..." option (4th item)
            userSubmenuX = menuX + 140;
            userSubmenuY = menuY + 110; // Approximately where the 4th item would be

            isUserSubmenuOpen = true;

            // Don't close the main menu yet
            StateHasChanged();
        }
        protected async Task AssignToSpecificUser(string userId, int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                try
                {
                    task.AssignedSubUser = await TaskService.AssignToSpecificUser(taskId, userId);
                    // Add activity note
                    var user = _stateService.CurrentUser;
                    var userName = user != null ? user.FirstName : "System";
                    var assignedUser = task.AssignedSubUser != null ? $"{task.AssignedSubUser.FirstName} {task.AssignedSubUser.LastName}" : userId;
                    var note = new RenewalNote
                    {
                        RenewalId = renewalId,
                        Note = $"<strong>{task.TaskItemName}</strong> assigned to {assignedUser}.",
                        DateCreated = DateTime.Now,
                        CreatedById = user?.Id ?? "System",
                        NoteType = RenewalNoteType.SystemLog
                    };
                    await RenewalService.AddRenewalNoteAsync(note);
                    if (activityLogComponent != null)
                        await activityLogComponent.RefreshLogAsync();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error assigning user: {ex.Message}");
                }
            }
            CloseAllMenus();
        }
        protected async Task HandleUserSelected(_userSubmenu.UserAssignmentInfo info)
        {
            if (info != null)
            {
                await AssignToSpecificUser(info.UserId, info.TaskId);
            }
        }
        protected void CloseAllMenus()
        {
            isMenuOpen = false;
            isUserSubmenuOpen = false;
            StateHasChanged();
        }
        protected void CloseMenu()
        {
            isMenuOpen = false;
            isUserSubmenuOpen = false;
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
        protected string GetCellClass(TaskItemViewModel task, bool? connector = false, bool? rounder = false)
        {
            var classes = new List<string>();
            if (connector == false)
            {
                if (task.IsHighlighted)
                {
                    classes.Add("highlighted-cell");
                }
                if (task.TaskItemId == SelectedParentTaskId)
                {
                    classes.Add("selected-cell");
                }

                if (classes.Count == 0 && rounder == true)
                {
                    classes.Add("blank-round-row");
                }
                else if (classes.Count == 0)
                {
                    classes.Add("blank-row");
                }
            }
            else
            {
                //Special handling for the connector so that it can tie in the UI subtask component visuallys
                if (task.TaskItemId == SelectedParentTaskId)
                {
                    classes.Add("selected-cell-connector");
                }
            }

            return string.Join(" ", classes);
        }
        protected async void HighlightRow(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.IsHighlighted = !task.IsHighlighted;
                await TaskService.UpdateTaskHighlight(taskId, task.IsHighlighted);
            }
            CloseMenu();
        }
        protected async void ShowGoalDateDialog(int taskId)
        {
            EditTrackTaskId = taskId;
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            EditGoalDate = task.TaskGoalDate ?? DateTime.Now;
            editGoalDateDialog.Show();
            isMenuOpen = false;
            dialogHidden = false;
            CloseMenu();

        }
        protected async void AssignToMe(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.AssignedSubUser = await TaskService.AssignToMe(taskId);
            }
            CloseMenu();
        }
        protected async void HideRow(int taskId)
        {
            // parent VM (only parent tasks call this method)
            var parent = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (parent == null || parent.IsHidden) { CloseMenu(); return; }

            var now = DateTime.Now;

            // ── 1. parent ────────────────────────────────────────────────
            parent.IsHidden = true;
            parent.IsCompleted = true;
            parent.TaskCompletedDate = now;
            await TaskService.UpdateTrackTaskModelAsync(parent);

            // ── 2. subtasks (if any) ─────────────────────────────────────
            var children = await TaskService.GetSubtasksForTask(taskId);   // entity list
            foreach (var sub in children)
            {
                sub.Hidden = true;
                sub.Completed = true;
                sub.CompletedDate = now;
                await TaskService.UpdateSubtaskEntityAsync(sub);

                // keep any loaded VM in sync
                var vm = taskList.FirstOrDefault(v => v.TaskItemId == sub.Id);
                if (vm != null)
                {
                    vm.IsHidden = true;
                    vm.IsCompleted = true;
                    vm.TaskCompletedDate = now;
                }
            }

            // ── 3. activity log ──────────────────────────────────────────
            var user = _stateService.CurrentUser;
            await RenewalService.AddRenewalNoteAsync(new RenewalNote
            {
                RenewalId = renewalId,
                Note = $"<strong>{parent.TaskItemName}</strong> and {children.Count} sub-tasks were archived (hidden & completed).",
                DateCreated = now,
                CreatedById = user?.Id ?? "System",
                NoteType = RenewalNoteType.SystemLog
            });

            // ── 4. select a new parent for the subtask panel ────────────────
            if (SelectedParentTaskId == parent.TaskItemId)
            {
                // First try: the first **incomplete** visible parent task
                var nextParent = taskList
                    .Where(t => t.ParentTaskId == null && !t.IsHidden && !t.IsCompleted)
                    .OrderBy(t => t.TaskItemId)        // or any ordering you prefer
                    .FirstOrDefault();

                // Fallback: just the first visible parent task
                nextParent ??= taskList
                    .Where(t => t.ParentTaskId == null && !t.IsHidden)
                    .OrderBy(t => t.TaskItemId)
                    .FirstOrDefault();

                SelectedParentTaskId = nextParent?.TaskItemId;
                _stateService.HtmlSubTaskId = SelectedParentTaskId ?? 0;   // keep deep-linking in sync
            }

            await LoadSubtaskCounts();   // refresh badges / indeterminate states
            StateHasChanged();
            CloseMenu();
        }

        // Quick Goal Date Action Methods for Context Menu Toolbar
        protected async void ClearTaskGoalDate(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.TaskGoalDate = null;
                await UpdateTaskGoalDate(task, "cleared");
            }
            CloseMenu();
        }
        protected async void SetTaskGoalDateTomorrow(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.TaskGoalDate = DateTime.Today.AddDays(1);
                await UpdateTaskGoalDate(task, DateTime.Today.AddDays(1).ToString("MM/dd/yyyy"));
            }
            CloseMenu();
        }
        protected async void SetTaskGoalDateNextWeek(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.TaskGoalDate = DateTime.Today.AddDays(7);
                await UpdateTaskGoalDate(task, DateTime.Today.AddDays(7).ToString("MM/dd/yyyy"));
            }
            CloseMenu();
        }
        protected async void SetTaskGoalDateTwoWeeks(int taskId)
        {
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskId);
            if (task != null)
            {
                task.TaskGoalDate = DateTime.Today.AddDays(14);
                await UpdateTaskGoalDate(task, DateTime.Today.AddDays(14).ToString("MM/dd/yyyy"));
            }
            CloseMenu();
        }
        protected async Task UpdateTaskGoalDate(TaskItemViewModel task, string dateDescription)
        {
            await TaskService.UpdateTrackTaskModelAsync(task);

            // Add activity note
            var user = _stateService.CurrentUser;
            var userName = user != null ? user.FirstName : "System";
            var noteText = dateDescription == "cleared"
                ? $"Goal date for <strong>{task.TaskItemName}</strong> was cleared."
                : $"Goal date for <strong>{task.TaskItemName}</strong> set to {dateDescription}.";

            var note = new RenewalNote
            {
                RenewalId = renewalId,
                Note = noteText,
                DateCreated = DateTime.Now,
                CreatedById = user?.Id ?? "System",
                NoteType = RenewalNoteType.SystemLog
            };
            await RenewalService.AddRenewalNoteAsync(note);

            if (activityLogComponent != null)
                await activityLogComponent.RefreshLogAsync();
            StateHasChanged();
        }

        protected async Task OnCompletedChanged(int taskItemId, object isChecked)
        {
            // Find the main task
            var task = taskList.FirstOrDefault(t => t.TaskItemId == taskItemId);
            if (task == null) return;

            bool completed = (bool)isChecked;
            task.IsCompleted = completed;
            var user = _stateService.CurrentUser;
            var userFirstName = user != null ? user.FirstName : "System";
            var now = DateTime.Now;

            // Update completed date
            task.TaskCompletedDate = completed ? now : (DateTime?)null;

            // Update the main task in the database
            await TaskService.UpdateTrackTaskModelAsync(task);

            // Mark all subtasks as complete without unchecking subtasks if marking incomplete
            if (completed)
            {
                var subtasks = await TaskService.GetSubtasksForTask(task.TaskItemId);
                foreach (var subtask in subtasks)
                {
                    if (!subtask.Completed)
                    {
                        subtask.Completed = true;
                        subtask.CompletedDate = now;
                        await TaskService.UpdateSubtaskEntityAsync(subtask);

                        // Also update in local taskList if present
                        var subtaskVm = taskList.FirstOrDefault(t => t.TaskItemId == subtask.Id);
                        if (subtaskVm != null)
                        {
                            subtaskVm.IsCompleted = true;
                            subtaskVm.TaskCompletedDate = now;
                        }
                    }
                }
            }

            // Add a single activity note for the main task change
            string noteText = completed
                ? $"<strong>{task.TaskItemName}</strong> was completed."
                : $"<strong>{task.TaskItemName}</strong> was marked incomplete.";
            await RenewalService.AddRenewalNoteForTrackTaskAsync(renewalId, taskItemId, noteText, RenewalNoteType.SystemLog);

            // Enhancement #3: Auto update renewed status when certain subtasks are checked off
            if (completed)
            {
                await CheckForRenewalStatusUpdate(task);
            }

            // Reload subtask counts to update indeterminate states
            await LoadSubtaskCounts();
            StateHasChanged();
        }

        // Subtask Logic
        protected async Task LoadSubtaskCounts()
        {
            if (taskList == null || taskList.Count == 0) return;

            var parentIds = taskList.Select(t => t.TaskItemId).ToList();

            var counts = await TaskService.GetSubtaskCountsAsync(parentIds);

            foreach (var id in parentIds)
            {
                if (counts.TryGetValue(id, out var tuple))
                {
                    SubtaskCounts[id] = tuple.Total;
                    SubtaskCompletedCounts[id] = tuple.Completed;
                }
                else
                {
                    // Ensure dictionary has an entry even if no subtasks exist
                    SubtaskCounts[id] = 0;
                    SubtaskCompletedCounts[id] = 0;
                }
            }

            // Trigger state change to update indeterminate checkboxes
            StateHasChanged();
        }
        protected int GetSubtaskCount(int taskId)
        {
            return SubtaskCounts.TryGetValue(taskId, out var count) ? count : 0;
        }
        protected int GetSubtaskCompletedCount(int taskId)
        {
            return SubtaskCompletedCounts.TryGetValue(taskId, out var count) ? count : 0;
        }
        protected bool IsTaskIndeterminate(TaskItemViewModel task)
        {
            if (task.IsCompleted) return false; // If task is completed, not indeterminate

            var totalSubtasks = GetSubtaskCount(task.TaskItemId);
            var completedSubtasks = GetSubtaskCompletedCount(task.TaskItemId);

            // Indeterminate if there are subtasks and some (but not all) are completed
            return totalSubtasks > 0 && completedSubtasks > 0 && completedSubtasks < totalSubtasks;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            // Update checkbox indeterminate states after render
            if (taskList != null)
            {
                foreach (var task in taskList)
                {
                    var isIndeterminate = IsTaskIndeterminate(task);
                    var totalSubtasks = GetSubtaskCount(task.TaskItemId);
                    var completedSubtasks = GetSubtaskCompletedCount(task.TaskItemId);

                    // Determine if master task should be checked (all subtasks completed)
                    bool shouldBeChecked = totalSubtasks == 0 ? task.IsCompleted : completedSubtasks == totalSubtasks;

                    // Update the task's IsCompleted property in the database if it doesn't match the subtask completion state
                    if (task.IsCompleted != shouldBeChecked)
                    {
                        task.IsCompleted = shouldBeChecked;
                        task.TaskCompletedDate = shouldBeChecked ? DateTime.Now : (DateTime?)null;

                        // Update in database without waiting to avoid blocking the UI
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await TaskService.UpdateTrackTaskModelAsync(task);
                            }
                            catch
                            {
                                // Ignore database update errors in background
                            }
                        });
                    }

                    try
                    {
                        await JsInterop.SetCheckboxStateAsync(task.TaskItemId, isIndeterminate, shouldBeChecked);
                    }
                    catch
                    {
                        // Ignore JS errors if element not found
                    }
                }
            }
        }
        protected void OnSubtaskCountClicked(int taskId)
        {
            _stateService.HtmlSubTaskId = taskId;
            SelectedParentTaskId = taskId;
            StateHasChanged();
        }
        protected async Task OnSubtasksChanged()
        {
            await LoadSubtaskCounts();
            StateHasChanged();
        }
        protected void HandleNewSubtask(int parentTaskId)
        {
            SubtaskDialogParentId = parentTaskId;
            ShowSubtaskDialog = true;
            StateHasChanged();
        }
        protected async Task OnSubtaskCreatedFromMenu()
        {
            ShowSubtaskDialog = false;
            await LoadSubtaskCounts();
            StateHasChanged();
        }
        protected void OnSubtaskDialogCancel()
        {
            ShowSubtaskDialog = false;
            StateHasChanged();
        }

        // Main UI and Navigation
        protected Task HandleOnMenuChanged(MenuChangeEventArgs args)
        {
            switch (args.Id)
            {
                case "NewMasterTaskBtn":
                    RenewalNav.NavigateTo("/Renewals/MasterTasks");
                    break;
                case "NewTaskGroupBtn":
                    RenewalNav.NavigateTo("/Renewals/TaskGroupSorter/1");
                    break;
                case "NewSubmissionBtn":
                    HandleNewSubmission();
                    break;
            }
            return Task.CompletedTask;
        }

        public void HandleNewSubmission()
        {
            _stateService.HtmlTab = "tab-2";
            _stateService.HtmlTabId = 3;
            StateHasChanged();
        }
        protected class ContactViewModel
        {
            public int ContactId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }
        protected void BackToCalendar()
        {
            if (RenewalNav != null)
            {
                // Return to the main renewals page with the current filters preserved
                RenewalNav.NavigateTo($"/Renewals?month={_stateService.HtmlMonth}&year={_stateService.HtmlYear}&user={_stateService.HtmlUser}");
            }
        }
        protected void NavigateToNextRenewal()
        {
            if (_stateService.RenewalList == null || !_stateService.RenewalList.Any())
            {
                // If the list doesn't exist or is empty, load it first
                RenewalNav?.NavigateTo("/Renewals");
                return;
            }

            // Find the current index again to ensure it's accurate
            CurrentRenewalIndex = _stateService.RenewalList.FindIndex(r => r.RenewalId == renewalId);

            if (CurrentRenewalIndex == -1)
            {
                // If the current renewal isn't in the list, just return to the renewals page
                RenewalNav?.NavigateTo("/Renewals");
                return;
            }

            if (CurrentRenewalIndex < _stateService.RenewalList.Count - 1)
            {
                var nextRenewal = _stateService.RenewalList[CurrentRenewalIndex + 1];
                _stateService.HtmlRenewalId = nextRenewal.RenewalId;
                _stateService.HtmlView = "details";
                UpdateDataWithNewRenewalId(nextRenewal.RenewalId);
            }
        }
        protected void NavigateToPreviousRenewal()
        {
            if (_stateService.RenewalList == null || !_stateService.RenewalList.Any())
            {
                // If the list doesn't exist or is empty, load it first
                RenewalNav?.NavigateTo("/Renewals");
                return;
            }

            // Find the current index again to ensure it's accurate
            CurrentRenewalIndex = _stateService.RenewalList.FindIndex(r => r.RenewalId == renewalId);

            if (CurrentRenewalIndex == -1)
            {
                // If the current renewal isn't in the list, just return to the renewals page
                RenewalNav?.NavigateTo("/Renewals");
                return;
            }

            if (CurrentRenewalIndex > 0)
            {
                var prevRenewal = _stateService.RenewalList[CurrentRenewalIndex - 1];
                _stateService.HtmlRenewalId = prevRenewal.RenewalId;
                _stateService.HtmlView = "details";
                UpdateDataWithNewRenewalId(prevRenewal.RenewalId);
            }
        }

        // ActivityLog
        protected async Task OnActivityAdded()
        {
            // Optionally refresh other components or perform additional actions
            StateHasChanged();
        }

        // Component reference for the SubmissionsRedesigned component
        protected Submissions submissionsRedesignedComponent;

        // Renewal Status Options
        protected List<KeyValuePair<string, string>> RenewalStatusOptions { get; set; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("", "Pending"),
            new KeyValuePair<string, string>("Unneeded", "Unneeded"),
            new KeyValuePair<string, string>("Defected", "Defected"),
            new KeyValuePair<string, string>("Ghosted", "Ghosted"),
            new KeyValuePair<string, string>("Unplaceable", "Unplaceable"),
            new KeyValuePair<string, string>("Purple", "Purple"),
            new KeyValuePair<string, string>("Yellow", "Yellow"),
            new KeyValuePair<string, string>("Green", "Green")
        };

        protected async Task OnRenewalStatusChanged()
        {
            Console.WriteLine("--Ren Status Change");
            if (renewalDetails != null)
            {
                try
                {
                    await RenewalService.UpdateRenewalAsync(renewalDetails);
                    var user = _stateService.CurrentUser;
                    // Add a system note for the status change
                    var note = new RenewalNote
                    {
                        RenewalId = renewalDetails.RenewalId,
                        Note = $"Renewal status changed to '{renewalDetails.RenewalStatus}' by System",
                        DateCreated = DateTime.Now,
                        CreatedById = user?.Id ?? "System",
                        NoteType = RenewalNoteType.RenewalUpdate,
                        Deleted = false
                    };
                    await RenewalService.AddRenewalNoteAsync(note);

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating renewal status: {ex.Message}");
                }
            }
        }

        protected async Task AddRenewalStatusColors(PopupEventArgs args)
        {
            Console.WriteLine("AddRenewalStatusColors called - About to invoke JS function");
            Console.WriteLine($"RenewalStatusOptions count: {RenewalStatusOptions?.Count}");
            await JsInterop.AddRenewalStatusColorsAsync(RenewalStatusOptions, "renewalStatusDropdown");
            Console.WriteLine("JS function invoked successfully");
        }

        protected string GetRenewalStatusContainerClass(string renewalStatus)
        {
            return renewalStatus switch
            {
                "Renewed" => "status-container-renewed",
                "Unneeded" => "status-container-unneeded",
                "Defected" or "Ghosted" or "Unplaceable" => "status-container-defected",
                _ => "status-container-default" // For Pending, Other, and null/empty
            };
        }

        private async Task CheckForRenewalStatusUpdate(TaskItemViewModel task)
        {
            try
            {
                // Check if the task name contains any of the specified strings
                var taskName = task.TaskItemName?.ToLower() ?? "";
                var renewalKeywords = new[] { "renew in epic", "renewed in epic", "epic renewal" };

                if (renewalKeywords.Any(keyword => taskName.Contains(keyword)))
                {
                    // Update renewal status to "Renewed"
                    if (renewalDetails != null && renewalDetails.RenewalStatus != "Renewed")
                    {
                        renewalDetails.RenewalStatus = "Renewed";
                        await RenewalService.UpdateRenewalAsync(renewalDetails);
                        var user = _stateService.CurrentUser;
                        // Add a system note for the automatic status change
                        var note = new RenewalNote
                        {
                            RenewalId = renewalDetails.RenewalId,
                            TrackTaskId = task.TaskItemId,
                            Note = $"Renewal status automatically set to 'Renewed' when task '{task.TaskItemName}' was completed",
                            DateCreated = DateTime.Now,
                            CreatedById = user?.Id ?? "System",
                            NoteType = RenewalNoteType.RenewalUpdate,
                            Deleted = false
                        };
                        await RenewalService.AddRenewalNoteAsync(note);

                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for renewal status update: {ex.Message}");
            }
        }

        // Debug method to toggle loading state
        protected void ToggleLoadingDebug()
        {
            isLoading = !isLoading;
            StateHasChanged();
        }
        protected Task SetRenewalsDetailsTab(FluentTab e)
        {
            _stateService.HtmlTabId = e.Index;
            _stateService.HtmlTab = "tab-" + (e.Index + 1);
            if (_stateService.HtmlTab == "tab-1" && _stateService.HtmlSubTaskId != 0 && _stateService.HtmlRenewalId == renewalId)
            {
                SelectedParentTaskId = _stateService.HtmlSubTaskId;
            }
            return Task.CompletedTask;
        }

        // Cleanup
        public void Dispose()
        {
            foreach (var token in _checkboxDebounceTokens.Values)
            {
                token.Cancel();
                token.Dispose();
            }
            _checkboxDebounceTokens.Clear();

            foreach (var token in _notesDebounceTokens.Values)
            {
                token.Cancel();
                token.Dispose();
            }
            _notesDebounceTokens.Clear();
        }
    }
}
