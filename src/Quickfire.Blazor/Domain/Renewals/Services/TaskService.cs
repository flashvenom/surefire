using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.ViewModels;
using Quickfire.Blazor.Domain.Home.Models;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Renewals.Models;
using System.Collections.Generic;

namespace Quickfire.Blazor.Domain.Renewals.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public TaskService(StateService stateService, ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _context = context;
            _dbContextFactory = dbContextFactory;
        }

        // HOMEPAGE AND DAILY TASKS ---------------------------------------------------------//
        public async Task<List<HomePageTasksViewModel>> GetIncompleteTasks()
        {
            using var context = _dbContextFactory.CreateDbContext();

            var today = DateTime.Today;
            var cutoffDate = today.AddDays(7);
            var currentUser = _stateService.CurrentUser;

            // Fetch all incomplete tasks assigned to the current user where RenewalDate is today+7 days or older
            var tasks = await context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(s => s.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false) // Only incomplete tasks
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser) // Assigned to current user
                .Where(t => t.Renewal.RenewalDate <= cutoffDate) // RenewalDate is today + 7 days or older
                .OrderBy(t => t.Renewal.RenewalDate) // Order by RenewalDate with oldest at top
                .Take(20) // Limit to 20 tasks
                .ToListAsync();

            // Create the ViewModel list
            var result = tasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.Renewal.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.Renewal.Client.Name,
                ClientId = t.Renewal.Client.ClientId,
                PolicyProduct = t.Renewal.Policy?.Product?.LineCode ?? "Unknown",
                RenewalDate = t.Renewal.RenewalDate,
                Priority = $"Renewal Date: {t.Renewal.RenewalDate.ToShortDateString()}"
            }).ToList();

            return result;
        }
        public async Task<List<DailyTask>> UpdateDailyTaskAsync(DailyTask task)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingTask = await context.DailyTasks.FindAsync(task.Id);
            if (existingTask != null)
            {
                existingTask.Completed = task.Completed;
                if (task.Completed)
                {
                    existingTask.CompletedDate = DateTime.Now;
                }
                await context.SaveChangesAsync();
                
                // Invalidate homepage cache since daily tasks changed
                _stateService.InvalidateHomepageCache();
            }
            return await GetDailyTasksAsync();
        }
        public async Task<List<DailyTask>> AddNewDailyTaskAsync(DailyTask task)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);
            task.AssignedTo = currentUser; // Set the foreign key directly
            task.DateCreated = DateTime.Now;
            task.Order = 100;
            context.DailyTasks.Add(task);
            await context.SaveChangesAsync();
            
            // Invalidate homepage cache since daily tasks changed
            _stateService.InvalidateHomepageCache();
            
            return await GetDailyTasksAsync();
        }
        public async Task<List<DailyTask>> GetDailyTasksAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            var tasks = await context.DailyTasks
                .Where(task => !task.Completed)
                .Where(t => t.AssignedTo == currentUser)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
            return tasks;
        }
        public async Task UpdateDailyTaskOrderAsync(List<DailyTask> tasks)
        {
            using var context = _dbContextFactory.CreateDbContext();
            foreach (var task in tasks)
            {
                context.DailyTasks.Update(task);
            }

            await context.SaveChangesAsync();
            
            // Invalidate homepage cache since daily task order changed
            _stateService.InvalidateHomepageCache();
        }


        // MASTER AND GROUP TASKS ---------------------------------------------------------------------//
        public async Task<List<TaskMaster>> GetAllTaskMastersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TaskMasters.ToListAsync();
        }

        public async Task<List<TaskMaster>> GetTaskMastersForGroupAsync(int taskGroupId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            // Get the TaskMaster IDs for this group first
            var taskMasterIds = await context.TaskGroupTaskMasters
                .Where(tgtm => tgtm.TaskGroupId == taskGroupId)
                .OrderBy(tgtm => tgtm.OrderNumber)
                .Select(tgtm => tgtm.TaskMasterId)
                .ToListAsync();

            // Then get the TaskMasters with their subtasks, preserving the order
            var taskMasters = await context.TaskMasters
                .Where(tm => taskMasterIds.Contains(tm.TaskMasterId))
                .Include(tm => tm.SubTaskLinks)
                    .ThenInclude(stl => stl.SubTaskMaster)
                .ToListAsync();

            // Restore the original order from the TaskGroup
            return taskMasterIds
                .Select(id => taskMasters.First(tm => tm.TaskMasterId == id))
                .ToList();
        }

        public async Task<List<TaskGroup>> GetAllTaskGroupsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Consider including related data if needed downstream, but for a simple list, this is fine.
            // Example: .Include(tg => tg.TaskGroupTaskMasters)
            return await context.TaskGroups.OrderBy(tg => tg.TaskGroupId).ToListAsync();
        }
        public TaskGroup GetTaskGroupById(int taskGroupId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var taskGroup = context.TaskGroups
                .Include(t => t.TaskGroupTaskMasters)
                    .ThenInclude(tgtm => tgtm.TaskMaster)
                .AsNoTracking()
                .FirstOrDefault(t => t.TaskGroupId == taskGroupId);

            return taskGroup;
        }
        public async Task<TaskGroup> GetTaskGroupByIdAsync(int taskGroupId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var taskGroup = await context.TaskGroups
                .Include(t => t.TaskGroupTaskMasters)
                    .ThenInclude(tgtm => tgtm.TaskMaster)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TaskGroupId == taskGroupId);

            if (taskGroup == null) return null;

            var users = await context.Users.ToListAsync();

            return taskGroup;
        }
        public async Task<TaskMaster> GetTaskMasterByIdAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TaskMasters.FindAsync(taskId);
        }
        public async Task<TaskMaster> AddTaskMasterAsync(TaskMaster newTask)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Assign default staff if provided
            if (!string.IsNullOrEmpty(newTask.DefaultAssignedToId))
            {
                newTask.DefaultAssignedTo = await context.Users.FindAsync(newTask.DefaultAssignedToId);
            }

            // Attach MasterSubTasks if any
            if (newTask.MasterSubTasks != null && newTask.MasterSubTasks.Count > 0)
            {
                foreach (var subtask in newTask.MasterSubTasks)
                {
                    subtask.TaskMaster = newTask;
                }
            }

            await context.TaskMasters.AddAsync(newTask);
            await context.SaveChangesAsync();
            return newTask;
        }
        public async Task<TaskGroup> AddTaskGroupAsync(TaskGroup newGroup)
        {
            using var context = _dbContextFactory.CreateDbContext();

            await context.TaskGroups.AddAsync(newGroup);
            await context.SaveChangesAsync();
            return newGroup;
        }
        public async Task<TaskMaster> UpdateTaskMasterAsync(TaskMaster updatedTask)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existingTask = await context.TaskMasters
                .Include(t => t.MasterSubTasks)
                .FirstOrDefaultAsync(t => t.TaskMasterId == updatedTask.TaskMasterId);
            if (existingTask == null)
            {
                throw new InvalidOperationException("TaskMaster not found.");
            }

            existingTask.TaskName = updatedTask.TaskName;
            existingTask.Description = updatedTask.Description;
            existingTask.DaysBeforeExpiration = updatedTask.DaysBeforeExpiration;
            existingTask.ForType = updatedTask.ForType;
            existingTask.Important = updatedTask.Important;
            existingTask.DefaultAssignedToId = updatedTask.DefaultAssignedToId;
            existingTask.DefaultAssignedTo = null;
            if (!string.IsNullOrEmpty(updatedTask.DefaultAssignedToId))
            {
                existingTask.DefaultAssignedTo = await context.Users.FindAsync(updatedTask.DefaultAssignedToId);
            }

            // Update MasterSubTasks: remove old, add new
            context.MasterSubTasks.RemoveRange(existingTask.MasterSubTasks);
            if (updatedTask.MasterSubTasks != null && updatedTask.MasterSubTasks.Count > 0)
            {
                foreach (var subtask in updatedTask.MasterSubTasks)
                {
                    subtask.TaskMasterId = existingTask.TaskMasterId;
                    subtask.TaskMaster = existingTask;
                }
                await context.MasterSubTasks.AddRangeAsync(updatedTask.MasterSubTasks);
            }

            await context.SaveChangesAsync();
            return existingTask;
        }
        public async Task<TaskGroup> UpdateTaskGroupAsync(TaskGroup updatedGroup)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existingGroup = await context.TaskGroups.FindAsync(updatedGroup.TaskGroupId);
            if (existingGroup == null)
            {
                throw new InvalidOperationException("TaskGroup not found.");
            }

            existingGroup.Name = updatedGroup.Name;
            existingGroup.Description = updatedGroup.Description;

            await context.SaveChangesAsync();
            return existingGroup;
        }
        public async Task DeleteTrackTaskAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskId);
            if (task == null)
            {
                throw new InvalidOperationException("Task not found.");
            }
            context.TrackTasks.Remove(task);    
            await context.SaveChangesAsync();
        }

        public async Task DeleteTaskMasterAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var task = await context.TaskMasters.FindAsync(taskId);
            if (task == null)
            {
                throw new InvalidOperationException("TaskMaster not found.");
            }

            context.TaskMasters.Remove(task);
            await context.SaveChangesAsync();
        }
        public async Task DeleteTaskGroupAsync(int taskGroupId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var group = await context.TaskGroups.FindAsync(taskGroupId);
            if (group == null)
            {
                throw new InvalidOperationException("TaskGroup not found.");
            }

            var masterLinks = await context.TaskGroupTaskMasters
                .Where(t => t.TaskGroupId == taskGroupId)
                .ToListAsync();

            var masterIds = masterLinks
                .Select(link => link.TaskMasterId)
                .Distinct()
                .ToList();

            var sharedMasterIds = await context.TaskGroupTaskMasters
                .Where(t => masterIds.Contains(t.TaskMasterId) && t.TaskGroupId != taskGroupId)
                .Select(t => t.TaskMasterId)
                .Distinct()
                .ToListAsync();

            var uniqueMasterIds = masterIds
                .Where(id => !sharedMasterIds.Contains(id))
                .Distinct()
                .ToList();

            List<TaskMasterSubTask> subtaskLinks = new();
            List<int> potentialOrphanSubtaskIds = new();

            if (uniqueMasterIds.Any())
            {
                subtaskLinks = await context.TaskMasterSubTasks
                    .Where(st => uniqueMasterIds.Contains(st.ParentTaskMasterId))
                    .ToListAsync();

                potentialOrphanSubtaskIds = subtaskLinks
                    .Select(st => st.SubTaskMasterId)
                    .Distinct()
                    .ToList();
            }

            if (subtaskLinks.Any())
            {
                context.TaskMasterSubTasks.RemoveRange(subtaskLinks);
            }

            if (masterLinks.Any())
            {
                context.TaskGroupTaskMasters.RemoveRange(masterLinks);
            }

            List<int> subtaskIdsSafeToDelete = new();
            if (potentialOrphanSubtaskIds.Any())
            {
                var stillLinkedSubtasks = await context.TaskMasterSubTasks
                    .Where(st => potentialOrphanSubtaskIds.Contains(st.SubTaskMasterId))
                    .Select(st => st.SubTaskMasterId)
                    .Distinct()
                    .ToListAsync();

                var subtasksReferencedByGroups = await context.TaskGroupTaskMasters
                    .Where(t => potentialOrphanSubtaskIds.Contains(t.TaskMasterId) && t.TaskGroupId != taskGroupId)
                    .Select(t => t.TaskMasterId)
                    .Distinct()
                    .ToListAsync();

                var blockedSubtasks = new HashSet<int>(stillLinkedSubtasks);
                foreach (var id in subtasksReferencedByGroups)
                {
                    blockedSubtasks.Add(id);
                }

                subtaskIdsSafeToDelete = potentialOrphanSubtaskIds
                    .Where(id => !blockedSubtasks.Contains(id))
                    .ToList();
            }

            if (subtaskIdsSafeToDelete.Any())
            {
                var subtaskMasters = await context.TaskMasters
                    .Where(tm => subtaskIdsSafeToDelete.Contains(tm.TaskMasterId))
                    .ToListAsync();
                context.TaskMasters.RemoveRange(subtaskMasters);
            }

            if (uniqueMasterIds.Any())
            {
                var masterTasksToRemove = await context.TaskMasters
                    .Where(tm => uniqueMasterIds.Contains(tm.TaskMasterId))
                    .ToListAsync();
                context.TaskMasters.RemoveRange(masterTasksToRemove);
            }

            context.TaskGroups.Remove(group);
            await context.SaveChangesAsync();
        }


        // TASKS [GET] ----------------------------------------------------------------------//
        public async Task<List<TaskItemViewModel>> GetTasksAsListForRenewalAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var tasks = await context.TrackTasks
                .Where(t => t.Renewal.RenewalId == renewalId)
                .Select(t => new TaskItemViewModel
                {
                    TaskItemId = t.Id,
                    TaskItemName = t.TaskName,
                    IsCompleted = t.Completed,
                    IsHighlighted = t.Highlighted,
                    IsHidden = t.Hidden,
                    Status = t.Status,
                    TaskGoalDate = t.GoalDate,
                    TaskCompletedDate = t.CompletedDate,
                    AssignedSubUser = t.AssignedTo,
                    Notes = t.Notes,
                    ParentTaskId = t.ParentTaskId
                }).ToListAsync();

            return tasks;
        }

        //Get list of tasks for a renewal from a renewal id
        public async Task<List<TrackTask>> GetTasksForRenewalAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TrackTasks.Where(t => t.RenewalId == renewalId).ToListAsync();
        }

        public async Task<TrackTaskEditViewModel> GetTrackTaskByIdAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Renewal)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return null;

            var users = await context.Users.ToListAsync();

            return new TrackTaskEditViewModel
            {
                Id = task.Id,
                TaskName = task.TaskName,
                Status = task.Status,
                Completed = task.Completed,
                Hidden = task.Hidden,
                Highlighted = task.Highlighted,
                Notes = task.Notes,
                GoalDate = task.GoalDate,
                CompletedDate = task.CompletedDate,
                Renewal = task.Renewal,
                UserName = task.AssignedTo?.UserName,
                Users = users
            };
        }
        public async Task<List<TaskMaster>> GetAllTaskMasters()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var tasks = await context.TaskMasters.ToListAsync();
            return tasks;
        }
        public async Task<TrackTask> GetTrackTaskById(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TrackTasks.FindAsync(taskId);
        }

        public async Task<TrackTask> AddTrackTaskAsync(TrackTask mytrackTask)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.TrackTasks.Add(mytrackTask);
            await context.SaveChangesAsync();
            return mytrackTask;
        }

        // TASKS [UPDATE] -------------------------------------------------------------------//
        public async Task UpdateTaskCompleted(int taskItemId, bool isCompleted)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Completed = isCompleted;
                task.CompletedDate = DateTime.Now;
                await context.SaveChangesAsync();
                
                // Invalidate homepage cache since track task completion changed
                _stateService.InvalidateHomepageCache();
            }
        }
        public async Task UpdateTaskHighlight(int taskItemId, bool isHighlighted)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Highlighted = isHighlighted;
                await context.SaveChangesAsync();
                
                // Invalidate homepage cache since task highlighting changed
                _stateService.InvalidateHomepageCache();
            }
        }
        public async Task UpdateTaskHidden(int taskItemId, bool isHidden)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Hidden = isHidden;
                if (isHidden == true)
                {
                    task.Completed = true;
                }
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTaskNotesAsync(int taskItemId, string newNotes)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FirstOrDefaultAsync(t => t.Id == taskItemId);
            if (task != null)
            {
                task.Notes = newNotes;
                context.TrackTasks.Update(task);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTrackTaskAsync(TrackTaskEditViewModel model)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(model.Id);
            if (task != null)
            {
                task.TaskName = model.TaskName;
                task.Status = model.Status;
                task.Completed = model.Completed;
                task.Hidden = model.Hidden;
                task.Highlighted = model.Highlighted;
                task.Notes = model.Notes;
                task.GoalDate = model.GoalDate;
                task.CompletedDate = model.CompletedDate;
                task.DateModified = DateTime.Now;

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                    task.AssignedTo = user;
                }
                else
                {
                    task.AssignedTo = null;
                }

                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTrackTaskModelAsync(TaskItemViewModel model)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(model.TaskItemId);
            if (task != null)
            {
                task.TaskName = model.TaskItemName;
                task.Status = model.Status;
                task.Completed = model.IsCompleted;
                task.Hidden = model.IsHidden;
                task.Highlighted = model.IsHighlighted;
                task.Notes = model.Notes;
                task.GoalDate = model.TaskGoalDate;
                task.CompletedDate = model.TaskCompletedDate;
                task.DateModified = DateTime.Now;

                await context.SaveChangesAsync();
                
                // Invalidate homepage cache since track task was updated
                _stateService.InvalidateHomepageCache();
            }
        }
        public async Task<ApplicationUser> AssignToMe(int taskItemId)
        {
            using var context = _dbContextFactory.CreateDbContext();


            // Fetch the user from this context
            var userId = _stateService.CurrentUser.Id; // Assuming the ID is available here
            var currentUser = await context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                throw new Exception("User not found in the database.");
            }
            var task = await context.TrackTasks.FindAsync(taskItemId);
            task.AssignedTo = currentUser;
            await context.SaveChangesAsync();
            return currentUser;
        }
        public async Task<ApplicationUser> AssignToSub(int taskItemId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            var subuser = await context.Users.FirstOrDefaultAsync(u => u.Id == "db0723c6-1702-4f55-8ff9-f7128ee68631");
            task.AssignedTo = subuser;
            await context.SaveChangesAsync();
            return subuser;
        }
        public async Task<ApplicationUser> AssignToSpecificUser(int taskItemId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);

            if (task == null)
            {
                throw new Exception($"Task with ID {taskItemId} not found.");
            }

            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            task.AssignedTo = user;
            await context.SaveChangesAsync();
            return user;
        }
        public async Task UnassignSub(int taskid)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskid);
            if (task != null)
            {
                task.AssignedTo = null;
                await context.SaveChangesAsync();
            }
        }

        public async Task AddTaskToGroupAsync(TaskGroupTaskMaster taskGroupTaskMaster)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            // Reorder existing items to make space for the new item
            var existingItems = await context.TaskGroupTaskMasters
                .Where(tgtm => tgtm.TaskGroupId == taskGroupTaskMaster.TaskGroupId)
                .Where(tgtm => tgtm.OrderNumber >= taskGroupTaskMaster.OrderNumber)
                .ToListAsync();

            foreach (var item in existingItems)
            {
                item.OrderNumber++;
            }

            await context.TaskGroupTaskMasters.AddAsync(taskGroupTaskMaster);
            await context.SaveChangesAsync();
        }

        public async Task RemoveTaskFromGroupAsync(int taskGroupId, int taskMasterId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var taskGroupTaskMaster = await context.TaskGroupTaskMasters
                .FirstOrDefaultAsync(tgtm => 
                    tgtm.TaskGroupId == taskGroupId && 
                    tgtm.TaskMasterId == taskMasterId);

            if (taskGroupTaskMaster == null) return;

            var orderNumber = taskGroupTaskMaster.OrderNumber;

            // Remove the association
            context.TaskGroupTaskMasters.Remove(taskGroupTaskMaster);

            // Reorder remaining items
            var itemsToReorder = await context.TaskGroupTaskMasters
                .Where(tgtm => tgtm.TaskGroupId == taskGroupId)
                .Where(tgtm => tgtm.OrderNumber > orderNumber)
                .ToListAsync();

            foreach (var item in itemsToReorder)
            {
                item.OrderNumber--;
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateTaskGroupOrderAsync(int taskGroupId, List<int> taskMasterIds)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var taskGroupTaskMasters = await context.TaskGroupTaskMasters
                .Where(tgtm => tgtm.TaskGroupId == taskGroupId)
                .ToListAsync();

            for (int i = 0; i < taskMasterIds.Count; i++)
            {
                var taskMasterId = taskMasterIds[i];
                var taskGroupTaskMaster = taskGroupTaskMasters
                    .FirstOrDefault(tgtm => tgtm.TaskMasterId == taskMasterId);

                if (taskGroupTaskMaster != null)
                {
                    taskGroupTaskMaster.OrderNumber = i;
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<TrackTask>> GetSubtasksForTask(int parentId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TrackTasks
                .Where(t => t.ParentTaskId == parentId)
                .ToListAsync();
        }

        // SUBTASK ASSIGNMENT (TaskMaster <-> TaskMaster)
        public async Task<List<TaskMaster>> GetSubtasksForParentAsync(int parentTaskMasterId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var subtasks = await context.TaskMasterSubTasks
                .Where(x => x.ParentTaskMasterId == parentTaskMasterId)
                .OrderBy(x => x.OrderNumber)
                .Select(x => x.SubTaskMaster)
                .ToListAsync();
            return subtasks;
        }

        public async Task<List<TaskMaster>> GetAvailableSubtasksForParentAsync(int parentTaskMasterId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var assignedSubtaskIds = await context.TaskMasterSubTasks
                .Where(x => x.ParentTaskMasterId == parentTaskMasterId)
                .Select(x => x.SubTaskMasterId)
                .ToListAsync();
            return await context.TaskMasters
                .Where(tm => tm.TaskMasterId != parentTaskMasterId && !assignedSubtaskIds.Contains(tm.TaskMasterId))
                .ToListAsync();
        }

        public async Task AssignSubtaskAsync(int parentTaskMasterId, int subTaskMasterId, int orderNumber)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var exists = await context.TaskMasterSubTasks.AnyAsync(x => x.ParentTaskMasterId == parentTaskMasterId && x.SubTaskMasterId == subTaskMasterId);
            if (!exists)
            {
                var link = new TaskMasterSubTask
                {
                    ParentTaskMasterId = parentTaskMasterId,
                    SubTaskMasterId = subTaskMasterId,
                    OrderNumber = orderNumber
                };
                context.TaskMasterSubTasks.Add(link);
                await context.SaveChangesAsync();
            }
        }

        public async Task UnassignSubtaskAsync(int parentTaskMasterId, int subTaskMasterId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var link = await context.TaskMasterSubTasks.FirstOrDefaultAsync(x => x.ParentTaskMasterId == parentTaskMasterId && x.SubTaskMasterId == subTaskMasterId);
            if (link != null)
            {
                context.TaskMasterSubTasks.Remove(link);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateSubtaskOrderAsync(int parentTaskMasterId, List<int> orderedSubTaskMasterIds)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var links = await context.TaskMasterSubTasks
                .Where(x => x.ParentTaskMasterId == parentTaskMasterId)
                .ToListAsync();
            for (int i = 0; i < orderedSubTaskMasterIds.Count; i++)
            {
                var subId = orderedSubTaskMasterIds[i];
                var link = links.FirstOrDefault(x => x.SubTaskMasterId == subId);
                if (link != null)
                {
                    link.OrderNumber = i;
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task SwapTaskGroupOrderAsync(int taskGroupId, int taskMasterId1, int taskMasterId2)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var taskGroup1 = await context.TaskGroupTaskMasters
                .FirstOrDefaultAsync(tgtm => tgtm.TaskGroupId == taskGroupId && tgtm.TaskMasterId == taskMasterId1);
            
            var taskGroup2 = await context.TaskGroupTaskMasters
                .FirstOrDefaultAsync(tgtm => tgtm.TaskGroupId == taskGroupId && tgtm.TaskMasterId == taskMasterId2);
            
            if (taskGroup1 != null && taskGroup2 != null)
            {
                var tempOrder = taskGroup1.OrderNumber;
                taskGroup1.OrderNumber = taskGroup2.OrderNumber;
                taskGroup2.OrderNumber = tempOrder;
                
                await context.SaveChangesAsync();
            }
        }

        public async Task SwapSubtaskOrderAsync(int parentTaskMasterId, int subTaskMasterId1, int subTaskMasterId2)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var subtask1 = await context.TaskMasterSubTasks
                .FirstOrDefaultAsync(tmst => tmst.ParentTaskMasterId == parentTaskMasterId && tmst.SubTaskMasterId == subTaskMasterId1);
            
            var subtask2 = await context.TaskMasterSubTasks
                .FirstOrDefaultAsync(tmst => tmst.ParentTaskMasterId == parentTaskMasterId && tmst.SubTaskMasterId == subTaskMasterId2);
            
            if (subtask1 != null && subtask2 != null)
            {
                var tempOrder = subtask1.OrderNumber;
                subtask1.OrderNumber = subtask2.OrderNumber;
                subtask2.OrderNumber = tempOrder;
                
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTrackTaskDailyCheckOffAsync(int trackTaskId, DateTime? dailyCheckOff)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(trackTaskId);
            if (task != null)
            {
                task.DailyCheckOff = dailyCheckOff;
                task.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Updates a subtask entity (TrackTask) directly. Only updates completion and completion date fields.
        /// </summary>
        public async Task UpdateSubtaskEntityAsync(TrackTask subtask)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var dbSubtask = await context.TrackTasks.FindAsync(subtask.Id);
            if (dbSubtask != null)
            {
                dbSubtask.Completed = subtask.Completed;
                dbSubtask.CompletedDate = subtask.CompletedDate;
                // Optionally update other fields if needed
                dbSubtask.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }

        public async Task<MasterSubTask> AddMasterSubTaskAsync(MasterSubTask newSubTask)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.MasterSubTasks.Add(newSubTask);
            await context.SaveChangesAsync();
            return newSubTask;
        }

        public async Task UpdateMasterSubTaskAsync(MasterSubTask updatedSubTask)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingSubTask = await context.MasterSubTasks.FindAsync(updatedSubTask.MasterSubTaskId);
            if (existingSubTask == null)
            {
                throw new InvalidOperationException("MasterSubTask not found.");
            }

            existingSubTask.TaskName = updatedSubTask.TaskName;
            existingSubTask.Description = updatedSubTask.Description;
            existingSubTask.OrderNumber = updatedSubTask.OrderNumber;

            await context.SaveChangesAsync();
        }

        public async Task DeleteMasterSubTaskAsync(int subTaskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var subTask = await context.MasterSubTasks.FindAsync(subTaskId);
            if (subTask == null)
            {
                throw new InvalidOperationException("MasterSubTask not found.");
            }

            context.MasterSubTasks.Remove(subTask);
            await context.SaveChangesAsync();
        }

        // Optimized helper to fetch sub-task counts -------------------------------------------------//
        /// <summary>
        /// Returns a dictionary keyed by ParentTaskId with a tuple of (Total, Completed) sub-task counts.
        /// </summary>
        public async Task<Dictionary<int, (int Total, int Completed)>> GetSubtaskCountsAsync(IEnumerable<int> parentTaskIds)
        {
            if (parentTaskIds == null || !parentTaskIds.Any())
                return new();

            using var context = _dbContextFactory.CreateDbContext();

            var aggregated = await context.TrackTasks
                .Where(t => t.ParentTaskId.HasValue && parentTaskIds.Contains(t.ParentTaskId.Value))
                .GroupBy(t => t.ParentTaskId.Value)
                .Select(g => new {
                    ParentId = g.Key,
                    Total = g.Count(),
                    Completed = g.Count(t => t.Completed)
                })
                .ToListAsync();

            return aggregated.ToDictionary(a => a.ParentId, a => (a.Total, a.Completed));
        }
    }
}
