using System;
using System.IO;
using System.Threading.Tasks;
using HireVault.Core.Entities;
using HireVault.Core.Interfaces;
using HireVault.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HireVault.Web.Controllers
{
    [Authorize]
    public class CandidatesController : Controller
    {
        private readonly ILogger<CandidatesController> _logger;
        private readonly IS3Service _s3Service;
        private readonly HireVaultDbContext _context;

        public CandidatesController(
            ILogger<CandidatesController> logger,
            IS3Service s3Service,
            HireVaultDbContext context)
        {
            _logger = logger;
            _s3Service = s3Service;
            _context = context;
        }

        // =========================
        // GET UPLOAD FORM
        // =========================
        [AllowAnonymous]
        [HttpGet("candidates/{candidateId}/documents/uploadform")]
        public IActionResult GetUploadDocumentPage(int candidateId)
        {
            var candidate = _context.Applicants.Find(candidateId);
            if (candidate == null)
            {
                return NotFound();
            }

            ViewBag.CandidateId = candidateId;
            return View();
        }

        // =========================
        // POST DOCUMENT UPLOAD
        // =========================
        [AllowAnonymous]
        [HttpPost("candidates/{candidateId}/documents/upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocuments(int candidateId)
        {
            var candidate = await _context.Applicants.FindAsync(candidateId);
            if (candidate == null)
            {
                return NotFound();
            }

            try
            {
                var files = Request.Form.Files;

                if (files == null || files.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please upload required documents.";
                    return Redirect($"/candidates/{candidateId}/documents/uploadform");
                }

                async Task SaveDocumentAsync(IFormFile file, DocumentType documentType)
                {
                    if (file == null || file.Length == 0) return;

                    var extension = Path.GetExtension(file.FileName);
                    var storedFileName = $"{Guid.NewGuid()}{extension}";
                    var s3Key = $"Candidates/{candidateId}/Documents/{storedFileName}";

                    using var stream = file.OpenReadStream();
                     await _s3Service.UploadFileAsync(
                        stream,
                        file.FileName,
                        s3Key,
                        file.ContentType
                    );

                    var document = new CandidateDocuments
                    {
                        DocumentId = Guid.NewGuid().ToString(),
                        CandidateId = candidateId,
                        DocumentType = documentType,
                        FileName = file.FileName,
                        S3Key = s3Key,
                        UploadedAt = DateTime.UtcNow.ToString("o")
                    };

                    _context.CandidateDocuments.Add(document);
                }

                // Aadhar Card
                await SaveDocumentAsync(
                    files["AadharCard"],
                    DocumentType.AadharCard
                );

                // Resume
                await SaveDocumentAsync(
                    files["Resume"],
                    DocumentType.Resume
                );

                // Resignation Letter
                await SaveDocumentAsync(
                    files["ResignationLetter"],
                    DocumentType.ResignationLetter
                );

                // Salary Slips (multiple)
                var salarySlips = files.GetFiles("SalarySlip");
                foreach (var slip in salarySlips)
                {
                    await SaveDocumentAsync(
                        slip,
                        DocumentType.SalarySlip
                    );
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Documents uploaded successfully!";
                return Redirect($"/candidates/{candidateId}/documents/uploadform");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading documents");
                TempData["ErrorMessage"] = "An error occurred while uploading documents.";
                return Redirect($"/candidates/{candidateId}/documents/uploadform");
            }
        }
    }
}
