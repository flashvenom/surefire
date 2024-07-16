//using Folient.Data;
//using Folient.Domain.Renewals.Models;
//using Folient.Domain.Renewals.ViewModels;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Folient.Domain.Renewals.Services
//{
//    public class TaskService
//    {
//        private readonly ApplicationDbContext _context;

//        public TaskService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<ImportantTaskViewModel>> GetHighlightedTasksAsync()
//        {
//            var marketingEntries = await _context.MarketingEntries
//                .Include(me => me.Tasks)
//                .ToListAsync();

//            var highlightedTasks = marketingEntries
//                .SelectMany(me => me.Tasks
//                .Where(t => !t.Completed && t.Highlighted)
//                    .Select(t => new ImportantTaskViewModel
//                    {
//                        InsuredName = me.InsuredName,
//                        InsuredId = me.Id,
//                        PolicyType = me.PolicyType,
//                        TaskName = t.TaskName,
//                        GoalDate = t.GoalDate,
//                        Note = t.Notes,
//                        ExpirationDate = me.ExpirationDate,
//                        DaysLeft = string.Empty,
//                        Type = "Reminder",
//                        CsrAssigned = me.CsrAssigned
//                    }))
//                .ToList();
//            return highlightedTasks;
//        }

//        public async Task<List<ImportantTaskViewModel>> GetPastDueTasksAsync()
//        {
//            var marketingEntries = await _context.MarketingEntries
//                .Include(me => me.Tasks)
//                .ToListAsync();

//            var pastDueTasks = marketingEntries
//                .SelectMany(me => me.Tasks
//                    .Where(t => !t.Completed && t.GoalDate.HasValue && t.GoalDate.Value < DateTime.Now)
//                    .Select(t => new ImportantTaskViewModel
//                    {
//                        InsuredName = me.InsuredName,
//                        InsuredId = me.Id,
//                        PolicyType = me.PolicyType,
//                        TaskName = t.TaskName,
//                        GoalDate = t.GoalDate,
//                        Note = t.Notes,
//                        ExpirationDate = me.ExpirationDate,
//                        DaysLeft = $"{(DateTime.Now.Date - t.GoalDate.Value.Date).Days} Days Past Due",
//                        Type = "Past Due",
//                        CsrAssigned = me.CsrAssigned
//                    }))
//                .OrderBy(t => t.GoalDate)
//                .ToList();
//            return pastDueTasks;
//        }

//        public async Task<List<ImportantTaskViewModel>> GetUpcomingTasksAsync()
//        {
//            var marketingEntries = await _context.MarketingEntries
//                .Include(me => me.Tasks)
//                .ToListAsync();

//            var upcomingTasks = marketingEntries
//                .SelectMany(me => me.Tasks
//                    .Where(t => !t.Completed && t.GoalDate.HasValue && t.GoalDate.Value >= DateTime.Now && t.GoalDate.Value <= DateTime.Now.AddDays(14))
//                .OrderBy(t => t.GoalDate)
//                    .Select(t => new ImportantTaskViewModel
//                    {
//                        InsuredName = me.InsuredName,
//                        InsuredId = me.Id,
//                        PolicyType = me.PolicyType,
//                        TaskName = t.TaskName,
//                        GoalDate = t.GoalDate,
//                        Note = t.Notes,
//                        ExpirationDate = me.ExpirationDate,
//                        DaysLeft = $"{(t.GoalDate.Value.Date - DateTime.Now.Date).Days} Days Left",
//                        Type = "Upcoming",
//                        CsrAssigned = me.CsrAssigned
//                    }))
//                .OrderBy(t => t.GoalDate)
//                .ToList();

//            return upcomingTasks;
//        }
//    }
//}
