using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Quickfire.Blazor.Domain.Renewals.Models
{
    /// <summary>
    /// ViewModel for displaying activity items in the log.
    /// </summary>
    public class ActivityItemViewModel
    {
        public RenewalNote SourceItem { get; }
        public string CssClass { get; }
        public bool IsSystemLog { get; }
        public bool IsUserNote { get; }
        public string FormattedDate { get; }
        public string FormattedDateFull { get; }
        public string DaysAgoText { get; }
        public string DisplaySource { get; }
        public MarkupString DisplayContent { get; }
        public string NoteType { get; }

        public ActivityItemViewModel(RenewalNote note)
        {
            SourceItem = note;
            IsSystemLog = note.NoteType == RenewalNoteType.SystemLog
                || note.NoteType == RenewalNoteType.SubmissionLog
                || note.NoteType == RenewalNoteType.SubmissionUpdate
                || note.NoteType == RenewalNoteType.RenewalUpdate;
            IsUserNote = note.NoteType == RenewalNoteType.UserTaskNote
                || note.NoteType == RenewalNoteType.UserSubtaskNote
                || note.NoteType == RenewalNoteType.UserEntry
                || note.NoteType == RenewalNoteType.SubmissionUserNote;
            
            var localDate = note.DateCreated.ToLocalTime();
            FormattedDate = localDate.ToString("MM/dd/yyyy hh:mm tt");
            FormattedDateFull = localDate.ToString("MMMM dd, yyyy");
            
            // Calculate days ago
            var daysDiff = (DateTime.Now.Date - localDate.Date).Days;
            DaysAgoText = daysDiff switch
            {
                0 => "Today",
                1 => "Yesterday", 
                _ when daysDiff < 7 => $"{daysDiff} days ago",
                _ when daysDiff < 30 => $"{daysDiff / 7} week{(daysDiff / 7 == 1 ? "" : "s")} ago",
                _ => $"{daysDiff / 30} month{(daysDiff / 30 == 1 ? "" : "s")} ago"
            };
            
            DisplaySource = note.CreatedBy != null ? $"{note.CreatedBy.FirstName} {note.CreatedBy.LastName}" : "System";
            DisplayContent = new MarkupString(note.Note);
            var classes = new List<string> { "activity-item" };
            if (IsSystemLog) classes.Add("system-log");
            if (IsUserNote) classes.Add("user-note");
            CssClass = string.Join(" ", classes);
            // Expose NoteType as string, lowercased, default to 'systemlog' if null
            NoteType = (note.NoteType != null ? note.NoteType.ToString() : "SystemLog").ToLower();
        }
    }

    // Add any additional activity-related models or viewmodels here as needed
}
