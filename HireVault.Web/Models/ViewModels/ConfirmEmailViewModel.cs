using System.ComponentModel.DataAnnotations;

namespace HireVault.Web.Models.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Confirmation Code")]
        public string ConfirmationCode { get; set; }
    }
}
