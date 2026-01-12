using System;
using HireVault.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireVault.Infrastructure.Data
{
    public class HireVaultDbContext : DbContext
    {
        public HireVaultDbContext(DbContextOptions<HireVaultDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<CandidateDocuments> CandidateDocuments { get; set; }
        public DbSet<Applicants> Applicants { get; set; }
    }
}