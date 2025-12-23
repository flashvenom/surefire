using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace Quickfire.Blazor.Domain.Utilities
{
    public class UtilityTool
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public Func<Microsoft.FluentUI.AspNetCore.Components.Icon> Icon { get; set; } = () => new Icons.Filled.Size24.Question();
    }
}
