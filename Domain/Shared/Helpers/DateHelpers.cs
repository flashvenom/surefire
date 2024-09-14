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
using System.Text.RegularExpressions;


namespace Mantis.Domain.Shared.Helpers
{
    public static class DateHelper //Rename to STRING helper
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
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber; // Return the original phone number if it's null or empty
            }

            if (phoneNumber.StartsWith("+1"))
            {
                phoneNumber = phoneNumber.Substring(2); // Remove the "+1" prefix
            }

            // Store the original phone number for fallback
            string originalPhoneNumber = phoneNumber;

            // Remove any non-numeric characters (like spaces, dashes, or parentheses)
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digits.Length == 10)
            {
                return string.Format("({0}) {1}-{2}",
                                     digits.Substring(0, 3),
                                     digits.Substring(3, 3),
                                     digits.Substring(6, 4));
            }

            return originalPhoneNumber;
        }

        public static string GenerateCertificateName(string clientName, string holderName)
        {
            // Step 1: Take the first 10 characters
            string shortClientName = clientName.Length > 10 ? clientName.Substring(0, 10) : clientName;
            string shortHolderName = holderName.Length > 10 ? holderName.Substring(0, 10) : holderName;

            // Step 2: Remove spaces and special characters
            shortClientName = RemoveSpecialCharacters(shortClientName);
            shortHolderName = RemoveSpecialCharacters(shortHolderName);

            // Step 3: Concatenate and append ".pdf"
            string certName = $"{shortClientName}{shortHolderName}.pdf";
            return certName;
        }

        private static string RemoveSpecialCharacters(string input)
        {
            // This will remove all characters except letters and digits
            return Regex.Replace(input, @"[^a-zA-Z0-9]", string.Empty);
        }

    }
}
