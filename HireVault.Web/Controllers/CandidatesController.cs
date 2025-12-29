using System;
using System.IO;
using System.Threading.Tasks;
using HireVault.Core.Entities;
using HireVault.Core.Interfaces;
using HireVault.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HireVault.Infrastructure.Data;

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

        [AllowAnonymous]
        [HttpGet("candidates/{candidateId}/documents/upload")]
        public IActionResult UploadDocument(string candidateId)
        {
            // Verify candidate exists
            var candidate = _context.Candidates.Find(candidateId);
            if (candidate == null)
            {
                return NotFound();
            }

            ViewBag.CandidateId = candidateId;
            return View(new DocumentUploadViewModel());
        }

        [AllowAnonymous]
        [HttpPost("candidates/{candidateId}/documents/upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(string candidateId, [FromForm] DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CandidateId = candidateId;
                return View(model);
            }

            try
            {
                // Verify candidate exists
                var candidate = await _context.Candidates.FindAsync(candidateId);
                if (candidate == null)
                {
                    return NotFound();
                }

                var file = Request.Form.Files["File"];
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("", "Please select a file to upload.");
                    ViewBag.CandidateId = candidateId;
                    return View(model);
                }

                // Generate S3 key
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var s3Key = $"HireVault/Candidates/{candidateId}/Documents/{fileName}";

                // Upload to S3
                using var fileStream = file.OpenReadStream();
                await _s3Service.UploadFileAsync(fileStream, file.FileName, s3Key, file.ContentType);

                // Save to database
                var document = new CandidateDocuments
                {
                    DocumentId = Guid.NewGuid().ToString(),
                    CandidateId = candidateId,
                    DocumentType = model.DocumentType,
                    FileName = file.FileName,
                    S3Key = s3Key,
                    UploadedAt = DateTime.UtcNow.ToString("o")
                };

                _context.CandidateDocuments.Add(document);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Document uploaded successfully!";
                return RedirectToAction(nameof(UploadDocument), new { candidateId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                ModelState.AddModelError("", "An error occurred while uploading the document. Please try again.");
                ViewBag.CandidateId = candidateId;
                return View(model);
            }
        }
    }
}