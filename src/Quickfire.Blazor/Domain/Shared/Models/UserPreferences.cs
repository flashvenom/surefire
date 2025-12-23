using System.Text.Json;

namespace Quickfire.Blazor.Domain.Shared.Models
{
    public class UserPreferences
    {
        public bool EnableAudio { get; set; }
        public bool EnableAnimations { get; set; }
        public bool EnableSimpleMode { get; set; }
        public HomepageLayout HomepageLayout { get; set; }

        public UserPreferences()
        {
            EnableAudio = false;
            EnableAnimations = false;
            EnableSimpleMode = false;
            HomepageLayout = new HomepageLayout();
        }

        public UserPreferences(bool enableAudio, bool enableAnimations, bool enableSimpleMode, HomepageLayout? homepageLayout = null)
        {
            EnableAudio = enableAudio;
            EnableAnimations = enableAnimations;
            EnableSimpleMode = enableSimpleMode;
            HomepageLayout = homepageLayout ?? new HomepageLayout();
        }

        // Create from ApplicationUser
        public static UserPreferences FromApplicationUser(Data.ApplicationUser user)
        {
            var homepageLayout = new HomepageLayout();
            
            // Parse the JSON if it exists
            if (!string.IsNullOrEmpty(user.HomepageLayoutJSON))
            {
                try
                {
                    homepageLayout = JsonSerializer.Deserialize<HomepageLayout>(user.HomepageLayoutJSON) ?? new HomepageLayout();
                }
                catch
                {
                    // If parsing fails, use default layout
                    homepageLayout = new HomepageLayout();
                }
            }
            
            return new UserPreferences(
                user.EnableAudio ?? false,
                user.EnableAnimations ?? false,
                user.EnableSimpleMode ?? false,
                homepageLayout
            );
        }

        // Apply to ApplicationUser
        public void ApplyToApplicationUser(Data.ApplicationUser user)
        {
            user.EnableAudio = EnableAudio;
            user.EnableAnimations = EnableAnimations;
            user.EnableSimpleMode = EnableSimpleMode;
            
            // Serialize homepage layout to JSON
            try
            {
                var json = JsonSerializer.Serialize(HomepageLayout);
                Console.WriteLine($"Serializing homepage layout to JSON: {json}");
                user.HomepageLayoutJSON = json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing homepage layout: {ex.Message}");
                // If serialization fails, set to default JSON
                user.HomepageLayoutJSON = new HomepageLayout().ToJson();
            }
        }
    }

    public enum HomepageItemVisibility
    {
        Visible = 1,
        Collapsed = 2, 
        Hidden = 0
    }

    public enum InspirationMode
    {
        BibleVerses = 0,
        Quotes = 1,
        Off = 2
    }

    public class HomepageItemSettings
    {
        public HomepageItemVisibility Vis { get; set; } = HomepageItemVisibility.Visible;
        public int Len { get; set; } = 0;
    }

    public class HomepageLayout
    {
        public HomepageItemSettings Proposals { get; set; } = new() { Len = 15 };
        public HomepageItemSettings Certs { get; set; } = new() { Len = 10 };
        public HomepageItemSettings Leads { get; set; } = new() { Len = 20 };
        public HomepageItemSettings Checklist { get; set; } = new();
        public HomepageItemSettings Tasks { get; set; } = new();
        public HomepageItemSettings Quicklinks { get; set; } = new();
        public InspirationMode Inspiration { get; set; } = InspirationMode.BibleVerses;

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static HomepageLayout FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<HomepageLayout>(json) ?? new HomepageLayout();
            }
            catch
            {
                return new HomepageLayout();
            }
        }
    }
}
