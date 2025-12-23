using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models;

namespace Quickfire.Blazor.Domain.Renewals.Services
{
    public class RenewalUpdateService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RenewalUpdateService> _logger;

        public RenewalUpdateService(
            ApplicationDbContext db,
            ILogger<RenewalUpdateService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<RenewalUpdate> EnablePortalAsync(int renewalId, bool showContractorInfo, bool ezRenewal, DateTime? expiresUtc = null)
        {
            var renewal = await _db.Renewals.Include(r => r.Policy).Include(r => r.Product).FirstOrDefaultAsync(r => r.RenewalId == renewalId)
                          ?? throw new InvalidOperationException($"Renewal {renewalId} not found");

            var existing = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId);
            if (existing == null)
            {
                existing = new RenewalUpdate
                {
                    RenewalId = renewal.RenewalId,
                    ClientId = renewal.ClientId,
                    PolicyId = renewal.PolicyId,
                    ProductId = renewal.ProductId,
                };
                _db.RenewalUpdates.Add(existing);
            }

            existing.RenewalHashId = string.IsNullOrWhiteSpace(existing.RenewalHashId) ? GenerateHashId() : existing.RenewalHashId;
            existing.PortalEnabled = true;
            existing.ShowContractorInfo = showContractorInfo;
            existing.EzRenewal = ezRenewal;
            existing.ExpiresUtc = expiresUtc ?? DateTime.UtcNow.AddDays(10);
            existing.DateModified = DateTime.UtcNow;
            existing.RenewalStage = Math.Max(existing.RenewalStage, 1); // 1=sent

            // Set sensible defaults for include flags based on product line
            if (!existing.IncludeGeneralLiability && !existing.IncludeWorkComp && !existing.IncludeCommercialAuto)
            {
                // Product IDs: WC=2, GL=3, CA=4
                switch (renewal.ProductId)
                {
                    case 2:
                        existing.IncludeWorkComp = true;
                        break;
                    case 3:
                        existing.IncludeGeneralLiability = true;
                        break;
                    case 4:
                        existing.IncludeCommercialAuto = true;
                        break;
                }
            }

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<RenewalUpdate> DisablePortalAsync(int renewalId)
        {
            var existing = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId)
                           ?? throw new InvalidOperationException($"Renewal {renewalId} not found");
            existing.PortalEnabled = false;
            existing.DateModified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<RenewalUpdate> UpdateSettingsAsync(int renewalId, bool showContractorInfo, bool ezRenewal, string? notesForClient, DateTime? expiresUtc, bool? includeGL = null, bool? includeWC = null, bool? includeAuto = null)
        {
            var ru = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId)
                     ?? throw new InvalidOperationException($"No RenewalUpdate for renewal {renewalId}");
            ru.ShowContractorInfo = showContractorInfo;
            ru.EzRenewal = ezRenewal;
            ru.NotesForClient = notesForClient;
            ru.ExpiresUtc = expiresUtc;
            if (includeGL.HasValue) ru.IncludeGeneralLiability = includeGL.Value;
            if (includeWC.HasValue) ru.IncludeWorkComp = includeWC.Value;
            if (includeAuto.HasValue) ru.IncludeCommercialAuto = includeAuto.Value;
            ru.DateModified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return ru;
        }

        public async Task<RenewalUpdate> RegenerateLinkAsync(int renewalId)
        {
            var ru = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId)
                     ?? throw new InvalidOperationException($"No RenewalUpdate for renewal {renewalId}");
            ru.RenewalHashId = GenerateHashId();
            ru.DateModified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return ru;
        }

        public async Task<bool> SaveOriginalIfEmptyAsync(int renewalId, string json)
        {
            var ru = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId)
                     ?? throw new InvalidOperationException($"No RenewalUpdate for renewal {renewalId}");
            if (string.IsNullOrEmpty(ru.OriginalJsonData))
            {
                ru.OriginalJsonData = json;
                ru.DateModified = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task RecordPushAsync(int renewalId, string json)
        {
            var ru = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalId == renewalId)
                     ?? throw new InvalidOperationException($"No RenewalUpdate for renewal {renewalId}");
            ru.JsonData = json; // reuse single field for last pushed/submitted
            ru.LastPushedUtc = DateTime.UtcNow;
            ru.DateModified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task HandleExternalSubmissionAsync(string renewalHashId, string submittedJson)
        {
            var ru = await _db.RenewalUpdates.FirstOrDefaultAsync(x => x.RenewalHashId == renewalHashId)
                     ?? throw new InvalidOperationException($"No RenewalUpdate for hash {renewalHashId}");

            // Soft-expire: accept but do not change PortalEnabled; stage → submitted
            ru.JsonData = submittedJson;
            ru.LastSubmittedUtc = DateTime.UtcNow;
            ru.DateModified = DateTime.UtcNow;
            ru.RenewalStage = Math.Max(ru.RenewalStage, 3); // 3=submitted

            await _db.SaveChangesAsync();
        }

        private static string GenerateHashId()
        {
            Span<byte> bytes = stackalloc byte[16];
            RandomNumberGenerator.Fill(bytes);
            // Base32 Crockford for short, URL-friendly token
            const string alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
            var value = new System.Numerics.BigInteger(bytes.ToArray().Append((byte)0).ToArray());
            var sb = new StringBuilder();
            while (value > 0)
            {
                value = System.Numerics.BigInteger.DivRem(value, 32, out var rem);
                sb.Insert(0, alphabet[(int)rem]);
            }
            var token = sb.Length == 0 ? "0" : sb.ToString();
            return token.Length >= 22 ? token[..22] : token.PadLeft(22, '0');
        }
    }
}
