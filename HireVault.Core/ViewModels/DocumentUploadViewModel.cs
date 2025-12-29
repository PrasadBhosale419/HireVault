using System.ComponentModel.DataAnnotations;
using HireVault.Core.Entities;

namespace HireVault.Core.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file")]
        [Display(Name = "Document")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Please select a document type")]
        [Display(Name = "Document Type")]
        public DocumentType DocumentType { get; set; }
    }
}
