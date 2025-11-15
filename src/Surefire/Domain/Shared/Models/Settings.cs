namespace Surefire.Domain.Shared.Models
{
    public class Settings
    {
        public int SettingsId { get; set; }
        public string? OpenAiApiKey { get; set; }
        public string? DbType { get; set; }
        public string? DbConnectionString { get; set; }
        public string? PayLinkStringTemplate { get; set; }
        public FileStoreType FileStore { get; set; } = FileStoreType.Local;
        public string? AzureBlobConnectionString { get; set; } 
        public string? AzureBlobContainerName { get; set; }
        public string? FileServerMappedPath { get; set; }
        public bool DisablePlugins{ get; set; }
    }
    public enum FileStoreType
    {
        AzureBlob,
        FileServer,
        Local
    }
}
