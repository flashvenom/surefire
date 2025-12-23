using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Quickfire.Blazor.Data;

namespace Microsoft.AspNetCore.Routing
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        private const string ExternalLoginCallbackAction = "LoginCallback";

        // Endpoints required by the Identity Razor components used under /Account.
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapPost("/PerformExternalLogin", (
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string provider,
                [FromForm] string returnUrl) =>
            {
                IEnumerable<KeyValuePair<string, StringValues>> query =
                [
                    new("ReturnUrl", returnUrl),
                    new("Action", ExternalLoginCallbackAction)
                ];

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/ExternalLogin",
                    QueryString.Create(query));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return TypedResults.Challenge(properties, [provider]);
            });

            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            accountGroup.MapPost("/PasskeyCreationOptions", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] IAntiforgery antiforgery) =>
            {
                await antiforgery.ValidateRequestAsync(context);

                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                var userName = await userManager.GetUserNameAsync(user) ?? "User";
                var optionsJson = await signInManager.MakePasskeyCreationOptionsAsync(new()
                {
                    Id = userId,
                    Name = userName,
                    DisplayName = userName
                });
                return TypedResults.Content(optionsJson, contentType: "application/json");
            });

            accountGroup.MapPost("/PasskeyRequestOptions", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] IAntiforgery antiforgery,
                [FromQuery] string? username) =>
            {
                await antiforgery.ValidateRequestAsync(context);

                var user = string.IsNullOrEmpty(username) ? null : await userManager.FindByNameAsync(username);
                var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);
                return TypedResults.Content(optionsJson, contentType: "application/json");
            });

            return accountGroup;
        }
    }
}

