namespace Mantis.Domain.Shared.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; }
        public byte[]? FileData { get; set; }
        public string? StoreType { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }

    }
}
