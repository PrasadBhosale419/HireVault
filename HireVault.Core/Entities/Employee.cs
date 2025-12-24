using HireVault.Core.Enums;
using System;
using System.Collections.Generic;

namespace HireVault.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string CognitoUserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public OnboardingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
