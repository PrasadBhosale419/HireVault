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
        public DbSet<DocumentType> DocumentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Document>()
                .HasOne(d => d.DocumentType)
                .WithMany()
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Documents)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}