// HireVault.Infrastructure/Services/DocumentService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireVault.Core.Entities;
using HireVault.Core.Interfaces;
using HireVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireVault.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HireVaultDbContext _context;

        public DocumentService(HireVaultDbContext context)
        {
            _context = context;
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Documents
                .Where(d => d.EmployeeId == employeeId)
                .Include(d => d.DocumentType)
                .ToListAsync();
        }

        public async Task<Document> UploadDocumentAsync(Document document)
        {
            document.UploadedAt = DateTime.UtcNow;
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyDocumentAsync(int id, bool isVerified)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            document.IsVerified = isVerified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}