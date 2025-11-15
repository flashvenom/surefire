namespace Surefire.Domain.Shared.Helpers
{
    public static class LogicHelper
    {
        public static bool IsDisabled(string pageNames, string currentPage)
        {
            var disabledPages = pageNames.Split(',');
            return disabledPages.Any(p => string.Equals(p.Trim(), currentPage, StringComparison.OrdinalIgnoreCase));
        }
    }
}
