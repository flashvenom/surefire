using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Shared;


namespace Mantis.Domain.Shared.Helpers
{
    public static class DateHelper
    {
        public static string FormatRenewalStatus(DateTime? goalDate, bool isCompleted, DateTime? completedDate)
        {
            if (isCompleted)
            {
                return $"Done {completedDate?.ToString("M/d 'at' h:mmtt")}";
            }
            else if (!goalDate.HasValue)
            {
                return " ";
            }
            else
            {
                var currentDate = DateTime.Now;
                var daysDifference = (goalDate.Value.Date - currentDate.Date).Days;

                if (daysDifference > 0)
                {
                    return $"<span class='db'>{goalDate?.ToString("M/d")}</span> (In {daysDifference} Days)";
                }
                else if (daysDifference == 0)
                {
                    return "DEADLINE TODAY";
                }
                else
                {
                    return $"<span style='color:red; font-weight:bold;'>{goalDate?.ToString("M/d")} (PAST DUE)</span>";
                }
            }
        }
    }
}
