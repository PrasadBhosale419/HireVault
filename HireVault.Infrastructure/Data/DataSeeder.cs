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
            if (!await context.Candidates.AnyAsync())
            {
                var candidates = new[]
                {
                    new Candidates
                    {
                        CandidateId = "CAND001",
                        FirstName = "John",
                        Email = "john.doe@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Candidates
                    {
                        CandidateId = "CAND002",
                        FirstName = "Jane",
                        Email = "jane.smith@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Candidates
                    {
                        CandidateId = "CAND003",
                        FirstName = "Robert",
                        Email = "robert.johnson@example.com",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Candidates.AddRangeAsync(candidates);
                await context.SaveChangesAsync();

                // Seed Candidate Documents
                var documents = new[]
                {
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = "CAND001",
                        DocumentType = DocumentType.Resume,
                        FileName = "JohnDoe_Resume.pdf",
                        S3Key = "HireVault/Candidates/CAND001/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = "CAND001",
                        DocumentType = DocumentType.AadharCard,
                        FileName = "JohnDoe_Aadhar.pdf",
                        S3Key = "HireVault/Candidates/CAND001/Documents/aadhar.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = "CAND002",
                        DocumentType = DocumentType.Resume,
                        FileName = "JaneSmith_Resume.pdf",
                        S3Key = "HireVault/Candidates/CAND002/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = "CAND003",
                        DocumentType = DocumentType.Resume,
                        FileName = "RobertJohnson_Resume.pdf",
                        S3Key = "HireVault/Candidates/CAND003/Documents/resume.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    },
                    new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = "CAND003",
                        DocumentType = DocumentType.SalarySlip,
                        FileName = "RobertJohnson_SalarySlip.pdf",
                        S3Key = "HireVault/Candidates/CAND003/Documents/salary_slip.pdf",
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    }
                };

                await context.CandidateDocuments.AddRangeAsync(documents);
                await context.SaveChangesAsync();
            }
        }
    }
}
