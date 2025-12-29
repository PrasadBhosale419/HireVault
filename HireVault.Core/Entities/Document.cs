using System;

namespace HireVault.Core.Entities
{
    public class Document
{
    public int Id { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; }
    public string? VerificationNotes { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    
    // Foreign keys
    public int EmployeeId { get; set; }
    public int DocumentTypeId { get; set; }
    
    // Navigation properties
    public required Employee Employee { get; set; }
}
}
