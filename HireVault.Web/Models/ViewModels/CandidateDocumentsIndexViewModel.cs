namespace HireVault.Web.Models.ViewModels
{
    public class CandidateDocumentsIndexViewModel
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
