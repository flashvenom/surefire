using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quickfire.Blazor.Domain.Shared.Models
{
    public class Settings
    {
        public int SettingsId { get; set; }
        public string? DbType { get; set; }
        public string? DbConnectionString { get; set; }
        public string? PayLinkStringTemplate { get; set; }
        [MaxLength(64)]
        public string? BlastmailProviderKey { get; set; } = "Graph";
        public string? BlastmailDefaultSenderName { get; set; }
        public string? BlastmailDefaultSender { get; set; }
        public FileStoreType FileStore { get; set; } = FileStoreType.Local;
        public string? FileStorageSettingsJson { get; set; }

        [NotMapped]
        public FileStorageSettings FileStorage
        {
            get => _fileStorage ??= FileStorageSettings.FromJson(FileStorageSettingsJson);
            set
            {
                _fileStorage = (value ?? FileStorageSettings.CreateDefault()).Normalize();
                FileStorageSettingsJson = _fileStorage.ToJson();
            }
        }

        public string? AzureBlobConnectionString { get; set; }
        public string? AzureBlobContainerName { get; set; }
        public string? FileServerMappedPath { get; set; }
        public bool DisablePlugins { get; set; }
        public bool SandbagMode { get; set; }
        public bool FakeyMode { get; set; }
        public string OrganizationTimeZoneId { get; set; } = DefaultOrganizationTimeZoneId;
        public string? CompanyManualAdminUserId { get; set; }

        private FileStorageSettings? _fileStorage;

        private static string DefaultOrganizationTimeZoneId =>
            System.OperatingSystem.IsWindows()
                ? "Pacific Standard Time"
                : "America/Los_Angeles";
    }

    public enum FileStoreType
    {
        AzureBlob,
        FileServer,
        Local
    }
}
