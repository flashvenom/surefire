using System.Data;

using System.Text.RegularExpressions;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Attachments.Models;
using Syncfusion.Blazor.Data;
using Microsoft.AspNetCore.Components;
using Surefire.Domain.Accounting.Models;
using System.Text;

namespace Surefire.Domain.Shared.Helpers
{
    public static class StringHelper
    {
        // Math
        public static int RenewalProgressPercentWeighted(ICollection<TrackTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return 0;
            }

            int totalTasks = tasks.Count;
            double totalWeight = 0;
            double weightedCompleted = 0;

            for (int i = 0; i < totalTasks; i++)
            {
                // Higher weight for earlier tasks
                double weight = (totalTasks - i) / (double)totalTasks;
                totalWeight += weight;

                if (tasks.ElementAt(i).Completed)
                {
                    weightedCompleted += weight;
                }
            }

            // Calculate weighted completion percentage
            return (int)((weightedCompleted / totalWeight) * 100);
        }
        public static int RenewalProgressPercent(ICollection<TrackTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return 0;
            }
            var visibleTasks = tasks.Where(task => !task.Hidden).ToList();

            if (visibleTasks.Count == 0)
            {
                return 0;
            }
            int totalTasks = tasks.Count;
            int completedTasks = tasks.Count(task => task.Completed);

            // Calculate evenly distributed completion percentage
            return (int)((double)completedTasks / totalTasks * 100);
        }
        public static string FormatSize(double? bytes)
        {
            if (bytes == null || bytes == 0)
                return "0 B";

            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024:0.#} KB";
            else if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024 * 1024):0.#} MB";
            else
                return $"{bytes / (1024 * 1024 * 1024):0.#} GB";
        }
        public static decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove formatting like commas and convert to decimal
            if (decimal.TryParse(value.Replace(",", ""), out var result))
                return result;

            return null; // Handle parsing failure gracefully
        }
        public static decimal? ParsePercentage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove percentage sign and convert to decimal
            if (value.EndsWith("%") && decimal.TryParse(value.TrimEnd('%'), out var result))
                return result; // Convert percentage to decimal fraction

            return null;
        }
        public static DateTime? ParseDate(string value)
        {
            if (DateTime.TryParse(value, out var result))
                return result;

            return null; // Handle parsing failure gracefully
        }


        // Builders
        public static string BuildUploadPath(int contactId, string? firstName, string? lastName, string subfolder, string extension)
        {
            var cleanName = SanitizeFileName(firstName, lastName);
            var buildName = $"{contactId}-{cleanName}{extension}";

            return buildName;
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
        public static List<string> GeneratePolicyVariations(string policyNumber)
        {
            List<string> policyVariations = new List<string>();

            // Step 1: Add the original policy number
            policyVariations.Add(policyNumber);

            // Step 2: Remove trailing "-XX" and add the result if different
            string withoutSuffix = Regex.Replace(policyNumber, @"-\d{2}$", "");
            if (!policyVariations.Contains(withoutSuffix))
            {
                policyVariations.Add(withoutSuffix);
            }

            // Step 3: Find and add any part of the policy number longer than 5 characters that are not in #1 or #2
            string alphanumericPart = Regex.Match(policyNumber, @"\w{6,}").Value;
            if (!string.IsNullOrEmpty(alphanumericPart) && !policyVariations.Contains(alphanumericPart))
            {
                policyVariations.Add(alphanumericPart);
            }

            // Step 4: Remove special characters and add the result
            string withoutSpecialChars = Regex.Replace(policyNumber, @"[^a-zA-Z0-9]", "");
            if (!policyVariations.Contains(withoutSpecialChars))
            {
                policyVariations.Add(withoutSpecialChars);
            }

            // Step 5: If #4 result is longer than 10 characters, add the first half
            if (withoutSpecialChars.Length > 10)
            {
                string firstHalf = withoutSpecialChars.Substring(0, withoutSpecialChars.Length / 2);
                if (!policyVariations.Contains(firstHalf))
                {
                    policyVariations.Add(firstHalf);
                }
            }

            return policyVariations;
        }
        public static List<string> GenerateCompanyNameVariations(string companyName)
        {
            List<string> variations = new List<string>();

            // 1. Add the original name to the list
            variations.Add(companyName);

            // 2. Remove common suffixes and add to the list
            string[] suffixes = new string[]
            {
                "Inc.", "Inc", "Corp.", "Corp", "LLC", "LLP", "Co.", "Co", "Ltd.", "Ltd",
                "Incorporated", "Company", "Limited", "PC", "PLC", "Group", "Holdings",
                "Holding", "Partners", "International", "Associates", "Services"
            };

            string nameWithoutSuffixes = RemoveSuffixes(companyName, suffixes);
            if (!variations.Contains(nameWithoutSuffixes))
            {
                variations.Add(nameWithoutSuffixes);
            }

            // 3. Remove special characters (like Quad-B -> QuadB) and add to the list
            string nameWithoutSpecialChars = RemoveAllSpecialCharacters(nameWithoutSuffixes);
            if (!variations.Contains(nameWithoutSpecialChars))
            {
                variations.Add(nameWithoutSpecialChars);
            }

            // 4. Use the first two words and add to the list
            string[] words = nameWithoutSpecialChars.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length >= 2)
            {
                string firstTwoWords = $"{words[0]} {words[1]}";
                if (!variations.Contains(firstTwoWords))
                {
                    variations.Add(firstTwoWords);
                }
            }

            // 5. Handle cases where the first word is longer than 6 characters, in all caps (an acronym), or contains '&'
            string firstWord = words.Length > 0 ? words[0] : string.Empty;

            if (firstWord.Length > 6 || IsAllCaps(firstWord) || firstWord.Contains("&"))
            {
                if (!variations.Contains(firstWord))
                {
                    variations.Add(firstWord);  // Add just the first word
                }
            }

            return variations;
        }
        public static string BuildThumbnailPath(Attachment attachment)
        {
            if (attachment == null || string.IsNullOrEmpty(attachment.LocalPath) || string.IsNullOrEmpty(attachment.HashedFileName))
            {
                throw new ArgumentException("Invalid attachment information provided.");
            }

            // Construct the base path
            string basePath = $"/{attachment.LocalPath}/";

            // Remove the extension from the hashed file name and append "_thumb.jpg"
            string thumbnailPath = $"{basePath}{Path.ChangeExtension(attachment.HashedFileName, null)}_thumb.jpg";

            return thumbnailPath;
        }

        // <summary>
        // Builds a Windows or URL path for an attachment.
        // </summary>
        // <param name="attachment">The attachment to build the path for.</param>
        // <param name="returnFolder">Whether to return the base path or the full path.</param>
        // <param name="returnAbsolute">Whether to return the absolute path of the file on the server.</param>
        // <returns>The Surefire mapped network drive path for the attachment or it's containing folder.</returns>
        public static string BuildWindowsPath(
            Attachment attachment,
            bool returnFolder = false,
            bool returnUrl = false,
            bool returnAbsolute = false
        )
        {

            if (attachment?.LocalPath is null || attachment.HashedFileName is null)
                throw new ArgumentException("Invalid attachment information provided.");

            // Split & normalize
            var segments = attachment.LocalPath
                .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            // For folder/default, drop a leading "uploads"
            bool hasUploads = segments.FirstOrDefault()?.Equals("uploads", StringComparison.OrdinalIgnoreCase) == true;
            var fsSegments = hasUploads ? segments.Skip(1) : segments;
            // For URL and absolute, keep everything
            var urlSegments = segments;
            var absoluteSegments = segments;
#if DEBUG
            const string baseNetwork = @"C:\source\devops\Surefire\Surefire\wwwroot\uploads\";
#else
    const string baseNetwork = @"S:\Surefiles\";
#endif

            const string iisRoot = @"C:\inetpub\surefire\wwwroot\";

            // 1) URL
            if (returnUrl)
            {
                var urlPath = string.Join("/", urlSegments);
                return $"https://surefire.local/{urlPath}/{attachment.HashedFileName}";
            }

            // 2) Absolute server file path under wwwroot
            if (returnAbsolute)
            {
                // C:\inetpub\surefire\wwwroot\uploads\images\2025\05\abcdef123.jpg
                var absPath = Path.Combine(new[] { iisRoot }
                    .Concat(absoluteSegments)
                    .Concat(new[] { attachment.HashedFileName })
                    .ToArray());
                return absPath;
            }

            // 3) Just the folder on the mapped drive
            if (returnFolder)
            {
                // S:\Surefiles\images\2025\05\
                var folder = Path.Combine(new[] { baseNetwork }
                    .Concat(fsSegments)
                    .ToArray()) + Path.DirectorySeparatorChar;
                return folder;
            }

            // 4) Default: full file on the mapped drive
            var fullPath = Path.Combine(new[] { baseNetwork }
                .Concat(fsSegments)
                .Concat(new[] { attachment.HashedFileName })
                .ToArray());
            return fullPath;
        }



        public static string GenerateFiveCharacterHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Combine input with a unique element (timestamp + GUID)
                string uniqueInput = $"{input}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";

                // Generate the hash
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(uniqueInput));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                // Take the first 5 characters of the hash
                return hashString.Substring(0, 5);
            }
        }


        // Stylers
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
        public static MarkupString GetStyledSubmissionStatus(int statusInt)
        {
            string status = GetSubmissionStatus(statusInt);
            string color;

            switch (status)
            {
                case "Declined":
                    color = "red";
                    break;
                case "Underwriting":
                    color = "darkgoldenrod"; // Using a more descriptive color for better browser support
                    break;
                case "Accepted":
                    color = "green";
                    break;
                default:
                    color = "black"; // Default color for other statuses
                    break;
            }

            string htmlString = $"<span style=\"color: {color};\">{status}</span>";
            return new MarkupString(htmlString);
        }
        public static string FormatDateTimeDifference(DateTime? date)
        {
            // Check if the date is null and return "Never"
            if (date == null)
            {
                return "Never";
            }

            var now = DateTime.Now;

            // Ensure the date is not in the future
            if (date > now)
            {
                date = now;
            }

            var timeDifference = now - date.Value;
            var totalSeconds = timeDifference.TotalSeconds;

            if (date.Value.Date == now.Date) // Today
            {
                if (totalSeconds < 3600)
                {
                    return "Just recently";
                }
                else
                {
                    int hoursAgo = (int)(totalSeconds / 3600);
                    return $"{hoursAgo} hours ago";
                }
            }
            else if (date.Value.Date == now.Date.AddDays(-1)) // Yesterday
            {
                return $"Yesterday {date.Value.ToString("h:mmtt")}";
            }
            else // Older dates
            {
                int daysAgo = (int)(now.Date - date.Value.Date).TotalDays;
                return $"{daysAgo} days ago";
            }
        }
        public static string FormatDateDifference(string dateString)
        {
            // Try multiple date formats to handle different input formats
            DateTime date;

            // First try the original strict format
            if (DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss.fffZ", null, System.Globalization.DateTimeStyles.AdjustToUniversal, out date))
            {
                // Success with original format
            }
            // Try standard DateTime.Parse which handles most common formats
            else if (DateTime.TryParse(dateString, out date))
            {
                // Success with standard parsing
            }
            else
            {
                return "(None)";
            }

            return FormatDateDifference(date);
        }

        public static string FormatDateDifference(DateTime date)
        {
            var now = DateTime.Now;
            var timeDifference = now - date;
            var totalSeconds = timeDifference.TotalSeconds;

            if (date.Date == now.Date) // Today
            {
                if (totalSeconds >= 0)
                {
                    if (totalSeconds < 3600)
                    {
                        return "Less than an hour ago.";
                    }
                    else
                    {
                        int hoursAgo = (int)(totalSeconds / 3600);
                        return $"About {hoursAgo} hours ago.";
                    }
                }
                else // Date is in the future today
                {
                    totalSeconds = -totalSeconds;
                    if (totalSeconds < 3600)
                    {
                        return "In <1 hour";
                    }
                    else
                    {
                        int hoursAhead = (int)(totalSeconds / 3600);
                        return $"In {hoursAhead} Hours";
                    }
                }
            }
            else if (date.Date == now.Date.AddDays(-1)) // Yesterday
            {
                return $"Yesterday at {date.ToString("h:mmtt")}";
            }
            else if (date.Date < now.Date) // Older dates
            {
                int daysAgo = (int)(now.Date - date.Date).TotalDays;
                return $"{daysAgo} days ago at {date.ToString("h:mmtt")}";
            }
            else if (date.Date == now.Date.AddDays(1)) // Tomorrow
            {
                return $"Tomorrow at {date.ToString("h:mmtt")}";
            }
            else // Future dates
            {
                int daysAhead = (int)(date.Date - now.Date).TotalDays;
                return $"In {daysAhead} Days at {date.ToString("h:mmtt")}";
            }
        }
        public static string FormatPhoneNumber2(string phoneNumber)
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
        public static string FormatTelDialNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber; // Return the original phone number if it's null or empty
            }

            // Remove any non-numeric characters
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // If the number has 11 digits and starts with '1', keep it as is
            if (digits.Length == 11 && digits.StartsWith("1"))
            {
                return digits; // The number already has the country code
            }

            // If the number has 10 digits, add '1' at the beginning
            if (digits.Length == 10)
            {
                return "1" + digits; // Add the US country code
            }

            // If the number doesn't meet the requirements, return an empty string or handle it differently
            return string.Empty;
        }
        public static string FormatSeconds(long seconds)
        {
            if (seconds < 40)
            {
                return $"{seconds} seconds";
            }
            else if (seconds < 60)
            {
                return "about a minute";
            }
            else if (seconds < 90)
            {
                return "one minute";
            }
            else if (seconds < 120)
            {
                return "a minute and a half";
            }
            else
            {
                long minutes = seconds / 60;
                return $"{minutes} minutes";
            }
        }
        public static string ColorizeJSON(string json)
        {
            var html = "";
            bool isInString = false;
            bool isKey = true; // To differentiate between keys and values
            int indentLevel = 0;

            foreach (var character in json)
            {
                if (character == '\"')
                {
                    if (isInString)
                    {
                        html += "\"</span>"; // Close the string span, applying color to the final quote
                        isInString = false;
                        isKey = false; // Switch to expecting a value after a key
                    }
                    else
                    {
                        isInString = true;
                        // Choose the color based on whether it's a key or value
                        html += isKey ? "<span class='key'>\"" : "<span class='string'>\"";
                    }
                }
                else if (!isInString && character == ':')
                {
                    html += "<span class='colon'>:</span>";
                    isKey = false; // Once we encounter a colon, we are in the value part
                }
                else if (!isInString && (character == '{' || character == '['))
                {
                    indentLevel++;
                    html += $"<span class='brace'>{character}</span><br>" + new string(' ', indentLevel * 4).Replace(" ", "&nbsp;");
                    isKey = true; // After a brace, we expect a key
                }
                else if (!isInString && (character == '}' || character == ']'))
                {
                    indentLevel--;
                    html += "<br>" + new string(' ', indentLevel * 4).Replace(" ", "&nbsp;") + $"<span class='brace'>{character}</span>";
                }
                else if (!isInString && character == ',')
                {
                    html += ",<br>" + new string(' ', indentLevel * 4).Replace(" ", "&nbsp;");
                    isKey = true; // After a comma, we expect another key
                }
                else if (!isInString && char.IsDigit(character))
                {
                    html += $"<span class='number'>{character}</span>";
                }
                else
                {
                    html += character == ' ' ? "&nbsp;" : character.ToString();
                }
            }

            html += "";
            return html;
        }
        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }
        public static string GetFirstName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            try
            {
                var words = fullName
                    .Trim()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (words.Length == 0)
                    return string.Empty;

                return words[0];
            }
            catch
            {
                return string.Empty;
            }
        }


        // Cleaners
        public static string SanitizeFileName(string? firstName, string? lastName)
        {
            // Fallback to "headshot" if both names are null or empty
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                return "headshot";
            }

            // If only one of the names is null or empty, use the other
            var fullName = $"{firstName} {lastName}".Trim();

            // Remove any non-alphabetic characters (allowing only letters)
            fullName = Regex.Replace(fullName, "[^a-zA-Z]", "");

            // If the name is completely sanitized to empty (no letters), fallback to "headshot"
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "headshot";
            }

            return fullName;
        }
        public static string CropCarrierName(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty; // Return an empty string if input is null or empty
            }

            // Split the input into words
            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Get the first two words
            var firstTwoWords = string.Join(" ", words.Take(2));

            // If the resulting first two words are less than 8 characters, return the first 10 characters
            if (firstTwoWords.Length < 8)
            {
                return input.Length <= 10 ? input : input.Substring(0, 10);
            }

            return firstTwoWords;
        }
        public static string RemoveHashedFileName(string? localPath, string? hashedFileName)
        {
            if (string.IsNullOrEmpty(localPath) || string.IsNullOrEmpty(hashedFileName))
                return localPath ?? string.Empty;

            // Remove the hashed filename from the path, including any trailing backslashes if needed
            int index = localPath.LastIndexOf(hashedFileName, StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? localPath.Substring(0, index) : localPath;
        }
        public static string GetDomainName(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            // Prepend "http://" if no scheme is found
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }

            // Attempt to create a Uri instance
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return string.Empty;
            }

            // Extract the host and remove "www." if needed
            string domain = uri.Host;
            if (domain.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                domain = domain.Substring(4);
            }

            return domain;
        }
        public static string FormatUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "#"; // Return a fallback or placeholder URL if empty
            }

            return url.StartsWith("http://") || url.StartsWith("https://")
                ? url
                : $"http://{url}";
        }
        public static string CleanUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            // Remove both "http://" and "https://" from the URL
            url = url.Replace("http://", "").Replace("https://", "");

            return url;
        }
        public static string RemoveSpecialCharacters(string input)
        {
            // This will remove all characters except letters and digits
            return Regex.Replace(input, @"[^a-zA-Z0-9]", string.Empty);
        }
        public static string RemoveAllSpecialCharacters(string input)
        {
            return new string(input.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        }
        public static bool IsAllCaps(string word)
        {
            return word.All(char.IsUpper);
        }
        public static string RemoveSuffixes(string name, string[] suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - suffix.Length).Trim();
                }
            }
            return name;
        }
        /// <summary>
        /// Cleans and formats a phone number string based on a mask.
        /// </summary>
        /// <param name="rawPhoneNumber">The input phone number string, potentially containing non-digit characters.</param>
        /// <param name="mask">raw=return cleaned numbees, standard=return valid phone number, +1=return with +1, 1=return with 1, or use a custom mask like (###) ###-####</param>
        /// <returns>The formatted phone number string, or an empty string if inputs are invalid.</returns>
        ///
        public static string FormatPhoneNumber(string phoneNumber, string? mask = null)
        {
            // Some simple null and default handling
            if (mask == null) mask = "(###) ###-####";
            if (string.IsNullOrWhiteSpace(phoneNumber)) return "(None)";

            // Extract the raw phone number with no country code or symbols
            string digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
            string mainNumber;
            if (digits.Length < 10)
            {
                mainNumber = new string('0', 10);
            }
            else if (digits.Length == 10)
            {
                mainNumber = digits;
            }
            else if (digits.Length == 11 && digits[0] == '1')
            {
                // If there's a country code '1', strip it off.
                mainNumber = digits.Substring(1, 10);
            }
            else if (digits.Length > 11)
            {
                // More than 11 digits (likely due to an extension)
                mainNumber = digits[0] == '1'
                    ? digits.Substring(1, 10)
                    : digits.Substring(0, 10);
            }
            else
            {
                // Halt and Catch Fire
                mainNumber = digits;
            }

            // Process the mask.
            if (mask.Equals("raw", StringComparison.OrdinalIgnoreCase))
            {
                // 'raw' returns the full cleaned digits.
                return digits;
            }
            else if (mask.Equals("standard", StringComparison.OrdinalIgnoreCase))
            {
                // 'standard' returns just the main number.
                return mainNumber;
            }
            else if (mask.Equals("+1", StringComparison.OrdinalIgnoreCase))
            {
                // '+1' returns the main number with a +1 prefix.
                return "+1" + mainNumber;
            }
            else if (mask.Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                // '1' returns the main number with a 1 prefix.
                return "1" + mainNumber;
            }
            else
            {
                // For custom masks, replace each '#' with a digit from the main number.
                var result = new System.Text.StringBuilder();
                int digitIndex = 0;
                foreach (char c in mask)
                {
                    if (c == '#' && digitIndex < mainNumber.Length)
                    {
                        result.Append(mainNumber[digitIndex]);
                        digitIndex++;
                    }
                    else
                    {
                        result.Append(c);
                    }
                }
                if (result.ToString() == "")
                {
                    return String.Empty;
                }
                else
                {
                    return result.ToString();
                }
            }
        }

        public static string CleanPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber; // Return the original phone number if it's null or empty
            }

            // Remove the "+1" or leading "1" prefix if it exists
            if (phoneNumber.StartsWith("+1"))
            {
                phoneNumber = phoneNumber.Substring(2);
            }
            else if (phoneNumber.StartsWith("1"))
            {
                phoneNumber = phoneNumber.Substring(1);
            }

            // Remove any non-numeric characters
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Return the first 10 digits if available, or all if less than 10
            return digits.Length >= 10 ? digits.Substring(0, 10) : digits;
        }
        public static string Del0s(decimal value)
        {
            return value % 1 == 0
                ? value.ToString("C0")  // Format as currency without decimals
                : value.ToString("C2"); // Format as currency with two decimals
        }
        public static string Del0s(decimal? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return Del0s(value.Value);
        }
        public static string DelP(decimal? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return value.Value % 1 == 0
                ? value.Value.ToString("0")  // No decimals for whole numbers
                : value.Value.ToString("0.##");  // Up to two decimals for fractional values
        }
        public static string DelNoZeros(decimal? value)
        {
            //Return no trailing 0s
            if (!value.HasValue)
                return string.Empty;

            return value.Value % 1 == 0
                ? value.Value.ToString("0")        // No decimals for whole numbers
                : value.Value.ToString("0.##");    // Up to two decimals for fractional values
        }

        // Getters
        public static string GetSubmissionStatus(int statusInt)
        {
            if (Enum.IsDefined(typeof(Surefire.Domain.Renewals.Models.SubmissionStatus), statusInt))
            {
                return ((Surefire.Domain.Renewals.Models.SubmissionStatus)statusInt).ToString();
            }
            else
            {
                return "Unknown Status"; // Fallback for undefined status values
            }
        }
        public static string GetSubmissionStatusPillText(int? statusInt)
        {
            if (!statusInt.HasValue)
            {
                return new string("NONE");
            }

            string statusText;

            switch (statusInt.Value)
            {
                case 0:
                    statusText = "CREATED";
                    break;
                case 1:
                    statusText = "STARTED";
                    break;
                case 2:
                    statusText = "SUBM'D";
                    break;
                case 3:
                    statusText = "QUOTED";
                    break;
                case 4:
                    statusText = "PROPSLD";
                    break;
                case 5:
                    statusText = "BOUND";
                    break;
                case 6:
                    statusText = "ISSUED";
                    break;
                default:
                    statusText = "UNKNOWN";
                    break;
            }

            return new string(statusText);
        }

        public static string GetSubmissionStatusPillClass(int? statusInt)
        {
            if (!statusInt.HasValue)
            {
                return new string("status-none");
            }

            string cssClass;

            switch (statusInt.Value)
            {
                case 0:
                    cssClass = "status-created";
                    break;
                case 1:
                    cssClass = "status-started";
                    break;
                case 2:
                    cssClass = "status-submitted";
                    break;
                case 3:
                    cssClass = "status-quoted";
                    break;
                case 4:
                    cssClass = "status-proposed";
                    break;
                case 5:
                    cssClass = "status-bound";
                    break;
                case 6:
                    cssClass = "status-issued";
                    break;
                default:
                    cssClass = "status-unknown";
                    break;
            }

            return new string(cssClass);
        }
        public static MarkupString GetSubmissionStatusPill(int? statusInt)
        {
            if (!statusInt.HasValue)
            {
                return new MarkupString("<span class=\"status-pill status-none\">NONE</span>");
            }

            string statusText;
            string cssClass;

            switch (statusInt.Value)
            {
                case 0:
                    statusText = "CREATED";
                    cssClass = "status-created";
                    break;
                case 1:
                    statusText = "STARTED";
                    cssClass = "status-started";
                    break;
                case 2:
                    statusText = "SUBM'D";
                    cssClass = "status-submitted";
                    break;
                case 3:
                    statusText = "QUOTED";
                    cssClass = "status-quoted";
                    break;
                case 4:
                    statusText = "PROPOSE";
                    cssClass = "status-proposed";
                    break;
                case 5:
                    statusText = "BOUND";
                    cssClass = "status-bound";
                    break;
                case 6:
                    statusText = "ISSUED";
                    cssClass = "status-issued";
                    break;
                default:
                    statusText = "UNKNOWN";
                    cssClass = "status-unknown";
                    break;
            }

            return new MarkupString($"<span class=\"status-pill {cssClass}\">{statusText}</span>");
        }
        public static string GetBestPhoneNumber(Dictionary<string, string> phoneNumbers)
        {
            // Prioritize Direct, Cell/Mobile, Office, and Desk in this order
            if (phoneNumbers.ContainsKey("Direct"))
                return phoneNumbers["Direct"];
            if (phoneNumbers.ContainsKey("Mobile"))
                return phoneNumbers["Mobile"];
            if (phoneNumbers.ContainsKey("Cell"))
                return phoneNumbers["Cell"];
            if (phoneNumbers.ContainsKey("Office"))
                return phoneNumbers["Office"];
            if (phoneNumbers.ContainsKey("Desk"))
                return phoneNumbers["Desk"];

            return phoneNumbers.Values.FirstOrDefault();
        }
        public static string GetBestMobileNumber(Dictionary<string, string> phoneNumbers)
        {
            if (phoneNumbers.ContainsKey("Mobile"))
                return phoneNumbers["Mobile"];
            if (phoneNumbers.ContainsKey("Cell"))
                return phoneNumbers["Cell"];
            if (phoneNumbers.ContainsKey("Direct"))
                return phoneNumbers["Direct"];
            if (phoneNumbers.ContainsKey("Office"))
                return phoneNumbers["Office"];
            if (phoneNumbers.ContainsKey("Desk"))
                return phoneNumbers["Desk"];

            return phoneNumbers.Values.FirstOrDefault();
        }
        /// <summary>
        /// Safely gets a substring of the specified length starting at the specified index.
        /// If the string is null, empty, or shorter than the required length, it returns the default value.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="startIndex">The starting index of the substring.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="defaultValue">The default value to return if the input is null, empty, or too short. Defaults to "???".</param>
        /// <returns>The substring if valid; otherwise, the default value.</returns>
        public static string GetSafeSubstring(string? input, int startIndex, int length, string defaultValue = "???")
        {
            // If the input is null or empty, return the default value
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }

            // If the input is shorter than the required length, return the entire string
            if (input.Length < startIndex + length)
            {
                return input;
            }

            // Return the requested substring
            return input.Substring(startIndex, length);
        }

        /// <summary>
        /// Converts a string representing a number of days (late/early) into a readable due phrase wrapped in a span with a class.
        /// </summary>
        /// <param name="daysString">The string representing the number of days (negative = late, positive = due in future, 0 = today).</param>
        /// <returns>HTML span with readable phrase and class.</returns>
        public static MarkupString FormatDuePhrase(string daysString, bool? futureTense = false)
        {
            if (!int.TryParse(daysString, out int days))
            {
                return new MarkupString(""); // Return empty if invalid
            }
            string tenseWord = futureTense == true ? "In" : "Due in";
            string phrase;
            string cssClass;

            if (days <= -11)
            {
                phrase = $"{Math.Abs(days)} Days Late";
                cssClass = "pricrazy";
            }
            else if (days <= -1)
            {
                phrase = days == -1 ? "1 Day Late" : $"{Math.Abs(days)} Days Late";
                cssClass = "prilate";
            }
            else if (days == 0)
            {
                phrase = "Due Today";
                cssClass = "pritoday";
            }
            else if (days == 1)
            {
                phrase = "Due Tomorrow";
                cssClass = "prinormal";
            }
            else if (days >= 2 && days <= 6)
            {
                phrase = $"{tenseWord} {days} Days";
                cssClass = "prinormal";
            }
            else if (days >= 7 && days <= 16)
            {
                if (days <= 13)
                {
                    phrase = $"{tenseWord} {days} Days";
                }
                else // days is 14, 15, or 16
                {
                    phrase = $"{tenseWord} 2 Weeks";
                }
                cssClass = "prilow";
            }
            else if (days >= 17 && days <= 45)
            {
                phrase = $"{tenseWord} 3+ Weeks";
                cssClass = "prino";
            }
            else if (days > 45)
            {
                phrase = "Next Month";
                cssClass = "prino";
            }
            else
            {
                phrase = "Due";
                cssClass = "prino";
            }

            return new MarkupString($"<span class='{cssClass}'>{phrase}</span>");
        }

    }

}
