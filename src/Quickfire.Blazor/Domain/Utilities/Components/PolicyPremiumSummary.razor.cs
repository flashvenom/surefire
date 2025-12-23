using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickfire.Blazor.Domain.Utilities.Components;

public sealed partial class PolicyPremiumSummary : ComponentBase, IDisposable
{
    [Parameter] public int? ClientId { get; set; }

    [Inject] private ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] private ILogger<PolicyPremiumSummary>? Logger { get; set; }
    [Inject] private AppJsInterop JsInterop { get; set; } = default!;

    private bool isLoading;
    private string? statusMessage;
    private string? errorMessage;
    private DateTime windowStart = DateTime.UtcNow.AddMonths(-12);

    private readonly List<ClientSummaryRow> clientSummaries = new();
    private bool showAllClients;

    private bool ShowAllClients
    {
        get => showAllClients;
        set
        {
            if (showAllClients != value)
            {
                showAllClients = value;
                _ = InvokeAsync(async () => await LoadReportAsync());
            }
        }
    }

    private int ActivePolicyCount { get; set; }
    private decimal TotalPremium { get; set; }
    private decimal AveragePremium => ActivePolicyCount > 0 ? TotalPremium / ActivePolicyCount : 0m;

    private string TotalPremiumFormatted => TotalPremium.ToString("C", CultureInfo.CurrentCulture);
    private string AveragePremiumFormatted => AveragePremium.ToString("C", CultureInfo.CurrentCulture);
    private bool CanRunReport => !isLoading && (ShowAllClients || ClientId.HasValue);

    protected override async Task OnParametersSetAsync()
    {
        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        if (!ShowAllClients && !ClientId.HasValue)
        {
            clientSummaries.Clear();
            ActivePolicyCount = 0;
            TotalPremium = 0m;
            statusMessage = null;
            errorMessage = null;
            windowStart = DateTime.UtcNow.AddMonths(-12);
            await InvokeAsync(StateHasChanged);
            return;
        }

        isLoading = true;
        errorMessage = null;
        statusMessage = "Loading policy summary...";
        windowStart = DateTime.UtcNow.AddMonths(-12);
        StateHasChanged();

        try
        {
            var now = DateTime.UtcNow;
            clientSummaries.Clear();

            var policiesQuery = DbContext.Policies
                .AsNoTracking()
                .Where(policy => policy.EffectiveDate <= now && policy.ExpirationDate >= windowStart);

            if (!ShowAllClients && ClientId.HasValue)
            {
                policiesQuery = policiesQuery.Where(policy => policy.ClientId == ClientId.Value);
            }

            var policyAggregates = await policiesQuery
                .GroupBy(policy => policy.ClientId)
                .Select(group => new
                {
                    ClientId = group.Key,
                    PolicyCount = group.Count(),
                    TotalPremium = group.Sum(p => p.Premium)
                })
                .ToListAsync()
                .ConfigureAwait(false);

            if (ShowAllClients)
            {
                var clients = await DbContext.Clients
                    .AsNoTracking()
                    .Select(client => new
                    {
                        client.ClientId,
                        client.Name,
                        client.LookupCode
                    })
                    .OrderBy(client => client.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var aggregateLookup = policyAggregates.ToDictionary(x => x.ClientId);

                foreach (var client in clients)
                {
                    aggregateLookup.TryGetValue(client.ClientId, out var metrics);
                    var count = metrics?.PolicyCount ?? 0;
                    var total = metrics?.TotalPremium ?? 0m;
                    var average = count > 0 ? total / count : 0m;

                    clientSummaries.Add(new ClientSummaryRow(
                        client.ClientId,
                        client.Name ?? string.Empty,
                        client.LookupCode ?? string.Empty,
                        count,
                        total,
                        average));
                }
            }
            else
            {
                var clientRecord = await DbContext.Clients
                    .AsNoTracking()
                    .Where(c => c.ClientId == ClientId!.Value)
                    .Select(c => new { c.ClientId, c.Name, c.LookupCode })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (clientRecord is null)
                {
                    errorMessage = "Client not found.";
                    ActivePolicyCount = 0;
                    TotalPremium = 0m;
                    clientSummaries.Clear();
                    return;
                }

                var metrics = policyAggregates.FirstOrDefault();
                var count = metrics?.PolicyCount ?? 0;
                var total = metrics?.TotalPremium ?? 0m;
                var average = count > 0 ? total / count : 0m;

                clientSummaries.Add(new ClientSummaryRow(
                    clientRecord.ClientId,
                    clientRecord.Name ?? string.Empty,
                    clientRecord.LookupCode ?? string.Empty,
                    count,
                    total,
                    average));
            }

            ActivePolicyCount = clientSummaries.Sum(row => row.ActivePolicies);
            TotalPremium = clientSummaries.Sum(row => row.TotalPremium);
            statusMessage = $"Loaded {clientSummaries.Count} client(s) at {DateTime.Now:t}";
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load policy premium summary for client {ClientId}", ClientId);
            errorMessage = "Unable to load policy premium data right now.";
            clientSummaries.Clear();
            ActivePolicyCount = 0;
            TotalPremium = 0m;
        }
        finally
        {
            isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task DownloadCsvAsync()
    {
        if (clientSummaries.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine("ClientName,LookupCode,ActivePolicies,TotalPremium,AveragePremium");

        foreach (var row in clientSummaries.OrderBy(summary => summary.ClientName, StringComparer.OrdinalIgnoreCase))
        {
            var line = string.Join(",",
                SanitizeCsvValue(row.ClientName),
                SanitizeCsvValue(row.LookupCode),
                SanitizeCsvValue(row.ActivePolicies.ToString(CultureInfo.InvariantCulture)),
                FormatCurrencyForCsv(row.TotalPremium),
                FormatCurrencyForCsv(row.AveragePremium));

            builder.AppendLine(line);
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        using var stream = new MemoryStream(bytes);
        using var streamRef = new DotNetStreamReference(stream);
        var fileName = $"policy-premium-summary-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        await JsInterop.DownloadFileFromStreamAsync(fileName, streamRef);
    }

    private static string SanitizeCsvValue(string? value) =>
        (value ?? string.Empty).Replace(",", string.Empty);

    private static string FormatCurrencyForCsv(decimal value) =>
        value.ToString("F2", CultureInfo.InvariantCulture).Replace(",", string.Empty);

    public void Dispose()
    {
    }

    private sealed class ClientSummaryRow
    {
        public ClientSummaryRow(int clientId, string clientName, string lookupCode, int activePolicies, decimal totalPremium, decimal averagePremium)
        {
            ClientId = clientId;
            ClientName = clientName;
            LookupCode = lookupCode;
            ActivePolicies = activePolicies;
            TotalPremium = totalPremium;
            AveragePremium = averagePremium;
        }

        public int ClientId { get; }
        public string ClientName { get; }
        public string LookupCode { get; }
        public int ActivePolicies { get; }
        public decimal TotalPremium { get; }
        public decimal AveragePremium { get; }

        public string TotalPremiumDisplay => TotalPremium.ToString("C", CultureInfo.CurrentCulture);
        public string AveragePremiumDisplay => AveragePremium.ToString("C", CultureInfo.CurrentCulture);
    }
}
