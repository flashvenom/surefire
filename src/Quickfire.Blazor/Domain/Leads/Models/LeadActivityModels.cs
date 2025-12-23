using System;
using Microsoft.AspNetCore.Components;
using Quickfire.Blazor.Domain.Clients.Models;

namespace Quickfire.Blazor.Domain.Leads.Models
{
    /// <summary>
    /// View model used to display Lead activity items in the timeline log.
    /// </summary>
    public class LeadActivityItemViewModel
    {
        private LeadActivityItemViewModel(string entryType,
                                          string displaySource,
                                          string content,
                                          DateTime timestamp,
                                          LeadNote? sourceNote = null)
        {
            EntryType = entryType;
            DisplaySource = displaySource;
            DisplayContent = new MarkupString(content);
            Timestamp = timestamp;
            SourceNote = sourceNote;

            var localDate = timestamp.ToLocalTime();
            FormattedDate = localDate.ToString("MM/dd/yyyy hh:mm tt");
            FormattedDateFull = localDate.ToString("MMMM dd, yyyy");
            DaysAgoText = GetDaysAgoText(localDate);
        }

        public LeadNote? SourceNote { get; }
        public string EntryType { get; }
        public string DisplaySource { get; }
        public MarkupString DisplayContent { get; }
        public DateTime Timestamp { get; }
        public string FormattedDate { get; }
        public string FormattedDateFull { get; }
        public string DaysAgoText { get; }
        public bool IsUserNote => EntryType == "note";

        public static LeadActivityItemViewModel FromLeadNote(LeadNote note)
        {
            return new LeadActivityItemViewModel(
                entryType: "note",
                displaySource: "Logged note",
                content: note.Note,
                timestamp: note.DateCreated,
                sourceNote: note);
        }

        public static LeadActivityItemViewModel CreateSystemEntry(string content, DateTime timestamp, string? source = null, string entryType = "system")
        {
            var displaySource = string.IsNullOrWhiteSpace(source) ? "System" : source!;
            return new LeadActivityItemViewModel(entryType, displaySource, content, timestamp);
        }

        private static string GetDaysAgoText(DateTime localDate)
        {
            var daysDiff = (DateTime.Now.Date - localDate.Date).Days;
            return daysDiff switch
            {
                < 0 => "Today",
                0 => "Today",
                1 => "Yesterday",
                < 7 => $"{daysDiff} days ago",
                < 30 => $"{daysDiff / 7} week{(daysDiff / 7 == 1 ? "" : "s")} ago",
                _ => $"{daysDiff / 30} month{(daysDiff / 30 == 1 ? "" : "s")} ago"
            };
        }
    }
}
