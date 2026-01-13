using System;
using System.Threading.Tasks;
using HireVault.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireVault.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(HireVaultDbContext context)
        {
            // Seed Candidates
            if (!await context.Applicants.AnyAsync())
            {
                var candidates = new[]
                {
                    new Applicants
                    {
                        ApplicantId = 1,
                        FirstName = "John",
                        Email = "john.doe@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Applicants
                    {
                        ApplicantId = 2,
                        FirstName = "Jane",
                        Email = "jane.smith@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Applicants
                    {
                        ApplicantId = 3,
                        FirstName = "Robert",
                        Email = "robert.johnson@example.com",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Applicants.AddRangeAsync(candidates);
                await context.SaveChangesAsync();

                // Seed Candidate Documents
                var documents = new[]
                {
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = 1,
                        DocumentType = DocumentType.Resume,
                        FileName = "JohnDoe_Resume.pdf",
                        S3Key = "HireVault/Candidates/1/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = 1,
                        DocumentType = DocumentType.AadharCard,
                        FileName = "JohnDoe_Aadhar.pdf",
                        S3Key = "HireVault/Candidates/1/Documents/aadhar.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = 2,
                        DocumentType = DocumentType.Resume,
                        FileName = "JaneSmith_Resume.pdf",
                        S3Key = "HireVault/Candidates/2/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = 3,
                        DocumentType = DocumentType.Resume,
                        FileName = "RobertJohnson_Resume.pdf",
                        S3Key = "HireVault/Candidates/3/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = 3,
                        DocumentType = DocumentType.SalarySlip,
                        FileName = "RobertJohnson_SalarySlip.pdf",
                        S3Key = "HireVault/Candidates/3/Documents/salary_slip.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    }
                };

                await context.CandidateDocuments.AddRangeAsync(documents);
                await context.SaveChangesAsync();
            }
        }
    }
}
