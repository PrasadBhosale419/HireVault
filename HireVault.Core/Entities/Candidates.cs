using System.ComponentModel.DataAnnotations;
namespace HireVault.Core.Entities
{
    public class Candidates
    {
        [Key]
        public string CandidateId { get; set; }
        public string FirstName {get; set;}
        public string Email {get; set;}
        public DateTime CreatedAt {get; set;}
    }
}