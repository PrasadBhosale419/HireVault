namespace HireVault.Web.Models.ViewModels
{
    public class DocumentListByCandidateIdViewModel
    {
            public string DocumentName { get; set; }   // Aadhaar, PAN, Resume, etc.
            public string DocumentUrl { get; set; }    // Pre-signed S3 URL
            public string ContentType { get; set; }    // application/pdf, image/png
    }
}
