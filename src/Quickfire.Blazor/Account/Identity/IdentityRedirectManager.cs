using Microsoft.AspNetCore.Components;

namespace Quickfire.Blazor.Components.Account
{
    internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
    {
        public const string StatusCookieName = "Identity.StatusMessage";

        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };

        public void RedirectTo(string? uri)
        {
            var destination = BuildAppRelativeUri(uri);
            navigationManager.NavigateTo(destination, forceLoad: true);
        }

        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var safeUri = BuildAppRelativeUri(uri);
            var uriWithoutQuery = navigationManager.ToAbsoluteUri(safeUri).GetLeftPart(UriPartial.Path);
            var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);

        private string BuildAppRelativeUri(string? uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return "/";
            }

            if (Uri.TryCreate(uri, UriKind.Absolute, out var absoluteUri))
            {
                if (!IsSameApplication(absoluteUri))
                {
                    return "/";
                }

                return BuildPathFromAbsolute(absoluteUri);
            }

            if (Uri.TryCreate(uri, UriKind.Relative, out var relativeUri))
            {
                var sanitized = relativeUri.ToString();

                if (sanitized.StartsWith("//", StringComparison.Ordinal) ||
                    sanitized.StartsWith("\\\\", StringComparison.Ordinal))
                {
                    return "/";
                }

                if (sanitized.StartsWith("~/", StringComparison.Ordinal))
                {
                    sanitized = sanitized[1..];
                }

                return sanitized.StartsWith("/", StringComparison.Ordinal)
                    ? sanitized
                    : "/" + sanitized;
            }

            return "/";
        }

        private bool IsSameApplication(Uri absoluteUri)
        {
            var baseUri = new Uri(navigationManager.BaseUri);
            return string.Equals(baseUri.Scheme, absoluteUri.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(baseUri.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase)
                && baseUri.Port == absoluteUri.Port;
        }

        private static string BuildPathFromAbsolute(Uri absoluteUri)
        {
            var destination = absoluteUri.PathAndQuery;
            if (!string.IsNullOrEmpty(absoluteUri.Fragment))
            {
                destination += absoluteUri.Fragment;
            }

            return string.IsNullOrEmpty(destination) ? "/" : destination;
        }
    }
}
