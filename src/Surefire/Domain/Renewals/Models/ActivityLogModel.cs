using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Surefire.Domain.Renewals.Models
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
            FormattedDate = note.DateCreated.ToLocalTime().ToString("MM/dd/yyyy hh:mm tt");
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
}
