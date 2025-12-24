using System;

namespace HireVault.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty; // S3 key
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; }
        public string? VerificationNotes { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; } // User ID of the verifier
        
        // Foreign keys
        public int EmployeeId { get; set; }
        public int DocumentTypeId { get; set; }
        
        // Navigation properties
        public Employee Employee { get; set; } = null!;
        public DocumentType DocumentType { get; set; } = null!;
    }
}
