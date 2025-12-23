using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Quickfire.Blazor.Data;

namespace Quickfire.Blazor.Domain.Shared.Services;

public interface IDataReassignmentService
{
    Task<int> ReassignAllOwnedRecordsAsync(string userId, CancellationToken cancellationToken = default);
}

public sealed class DataReassignmentService : IDataReassignmentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DataReassignmentService> _logger;

    public DataReassignmentService(ApplicationDbContext dbContext, ILogger<DataReassignmentService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> ReassignAllOwnedRecordsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var sqlHelper = _dbContext.GetService<ISqlGenerationHelper>();
        var model = _dbContext.Model;
        var totalUpdates = 0;

        foreach (var entityType in model.GetEntityTypes())
        {
            if (entityType.ClrType == typeof(ApplicationUser))
            {
                continue;
            }

            var clrNamespace = entityType.ClrType?.Namespace;
            if (!string.IsNullOrWhiteSpace(clrNamespace) &&
                clrNamespace.StartsWith("Microsoft.AspNetCore.Identity", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName) ||
                tableName.StartsWith("AspNet", StringComparison.OrdinalIgnoreCase) ||
                tableName.Equals("__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var schema = entityType.GetSchema();
            var storeObject = StoreObjectIdentifier.Table(tableName, schema);

            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                if (foreignKey.PrincipalEntityType.ClrType != typeof(ApplicationUser))
                {
                    continue;
                }

                if (foreignKey.Properties.Count != 1)
                {
                    continue;
                }

                var property = foreignKey.Properties[0];
                if (property.ClrType != typeof(string))
                {
                    continue;
                }

                var columnName = property.GetColumnName(storeObject);
                if (string.IsNullOrWhiteSpace(columnName))
                {
                    continue;
                }

                var tableIdentifier = sqlHelper.DelimitIdentifier(tableName, schema);
                var columnIdentifier = sqlHelper.DelimitIdentifier(columnName);

                var sql = $"UPDATE {tableIdentifier} SET {columnIdentifier} = @p0 WHERE {columnIdentifier} IS NOT NULL AND {columnIdentifier} <> @p0";
                var affected = await _dbContext.Database.ExecuteSqlRawAsync(sql, new object[] { userId }, cancellationToken).ConfigureAwait(false);
                if (affected > 0)
                {
                    totalUpdates += affected;
                    _logger.LogInformation("Reassigned {Count} rows in {Table}.{Column}", affected, tableName, columnName);
                }
            }
        }

        _logger.LogInformation("Finished reassigning ownership references. Total rows updated: {Total}", totalUpdates);
        return totalUpdates;
    }
}
