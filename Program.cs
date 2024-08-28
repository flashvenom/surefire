using Mantis.Components;
using Mantis.Components.Account;
using Mantis.Data;
using Mantis.Domain.Carriers.Services;
using Mantis.Domain.Policies.Services;
using Mantis.Domain.Clients.Services;
using Mantis.Domain.Renewals.Services;
using Mantis.Domain.Forms.Services;
using Mantis.Domain.Contacts.Services;
using Mantis.Domain.Shared.Services;
using Mantis.Domain.Users.Services;
using Mantis.Domain.Voip;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Syncfusion.Blazor;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

//Normal Blazor stuff ---//
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
bool detailedErrorsEnabled = builder.Configuration.GetValue<bool>("DetailedErrors:Enabled");

//SyncFusion Requirements ---//
builder.Services.AddSyncfusionBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddFluentUIComponents();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQwNzc3NUAzMjM2MmUzMDJlMzBuMkJzRGlPajJxTEIraGFiNDZ1NThtOG9CTkxocmFvWXozNVR3TUZLUjN3PQ==");

//Identity and Authorization ---//
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
//builder.Services.AddAuthorization();


//Database Context ---//
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddQuickGridEntityFrameworkAdapter();
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();



//Surefire Services ---//
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CarrierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RenewalService>();
builder.Services.AddScoped<PolicyService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<SharedService>();
builder.Services.AddScoped<FormService>();
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ITooltipService, TooltipService>();
builder.Services.AddSingleton<NavigationService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<BreadcrumbService>();
builder.Services.AddHttpContextAccessor();

//My API Services
builder.Services.AddHttpClient("CertificateApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7074/"); // Base address of your Blazor app
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


//RingCentral API Service ---//
builder.Services.AddSignalR();
builder.Services.AddScoped<CallAlertService>();
builder.Services.Configure<RingCentralOptions>(builder.Configuration.GetSection("RingCentralApi"));
builder.Services.AddHttpClient<RingCentralService>();
builder.Services.AddScoped<HubConnection>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/callAlertHub"))
        .WithAutomaticReconnect()
        .Build();
});

//CrmApiService ---//
builder.Services.Configure<CrmApiOptions>(builder.Configuration.GetSection("CrmApi"));
builder.Services.AddHttpClient<CrmApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.mycms.com/");
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
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapControllers();
app.MapAdditionalIdentityEndpoints();
app.MapHub<CallAlertHub>("/callAlertHub");

app.Run();
