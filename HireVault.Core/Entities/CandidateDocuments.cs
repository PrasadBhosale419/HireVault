using System.ComponentModel.DataAnnotations;
namespace HireVault.Core.Entities
{
    public class CandidateDocuments
    {
        [Key]
        public string DocumentId { get; set; }
        public string CandidateId { get; set; }
        public DocumentType DocumentType {get; set;}
        public string S3Key {get; set;}
        public string FileName {get; set;}
        public string UploadedAt {get; set;}
    }
    public enum DocumentType
    {
        AadharCard,
        Resume,
        ResignationLetter,
        SalarySlip
    }
}