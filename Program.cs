using Mantis.Components;
using Mantis.Components.Account;
using Mantis.Data;
using Mantis.Domain.Carriers.Services;
using Mantis.Domain.Clients.Services;
using Mantis.Domain.Renewals.Services;
using Mantis.Domain.Shared.Services;
using Mantis.Domain.User.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

//builder.Services.AddAuthorization();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

bool detailedErrorsEnabled = builder.Configuration.GetValue<bool>("DetailedErrors:Enabled");

builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CarrierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RenewalService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<BreadcrumbService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

//CrmApiService
builder.Services.Configure<CrmApiOptions>(builder.Configuration.GetSection("CrmApi"));
builder.Services.AddHttpClient<CrmApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.***REMOVED***/");
}).ConfigureHttpClient((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var clientId = config["ClientId"];
    var clientSecret = config["ClientSecret"];
    // Initialize the CrmApiService with the credentials
    client.DefaultRequestHeaders.Add("ClientId", clientId);
    client.DefaultRequestHeaders.Add("ClientSecret", clientSecret);
});

var app = builder.Build();

if (detailedErrorsEnabled)
{
    app.UseDeveloperExceptionPage(); // Detailed error page
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Custom error page
}
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQwNzc3NUAzMjM2MmUzMDJlMzBuMkJzRGlPajJxTEIraGFiNDZ1NThtOG9CTkxocmFvWXozNVR3TUZLUjN3PQ==");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapControllers();
app.MapAdditionalIdentityEndpoints();

app.Run();
