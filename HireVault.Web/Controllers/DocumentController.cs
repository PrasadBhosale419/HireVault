using HireVault.Core.Entities;
using HireVault.Core.Interfaces;
using HireVault.Infrastructure.Data;
using HireVault.Web.Models.ViewModels;
using HireVault.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireVault.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IS3Service _s3Service;
        private readonly IEmailService _emailService;

        public HireVaultDbContext _dbContext { get; }

        public DocumentController(IS3Service s3Service, HireVaultDbContext dbContext, IEmailService emailService)
        {
            _s3Service = s3Service;
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            // Step 1: Load CandidateDocuments into memory first
            var documents = _dbContext.CandidateDocuments
                .AsEnumerable()  // switch to client-side
                .GroupBy(cd => cd.CandidateId)
                .ToList();

            // Step 2: Join with applicants and build ViewModel
            var candidatesWithDocuments = (from g in documents
                                           join a in _dbContext.Applicants.AsEnumerable()
                                           on g.Key equals a.ApplicantId
                                           select new CandidateDocumentsIndexViewModel
                                           {
                                               CandidateId = g.Key,
                                               FullName = a.FirstName + " " + a.LastName,
                                               UploadedAt = g.Max(x => DateTime.Parse(x.UploadedAt)),
                                               Status = a.Status
                                           })
                                          .OrderByDescending(x => x.UploadedAt)
                                          .ToList();

            return View(candidatesWithDocuments);
        }

        [HttpGet("document/getdocument/{candidateId}")]
        public async Task<IActionResult> GetDocumentByCandidateId(int candidateId)
        {
            var documents = await _s3Service.GetCandidateDocumentsAsync(candidateId);

            var docObjects = documents.Select(d => new DocumentListByCandidateIdViewModel
            {
                DocumentName = d.DocumentName,
                DocumentUrl = d.DocumentUrl,
                ContentType = d.ContentType,
                DocumentType = d.DocumentType
            }).ToList();

            ViewBag.CandidateId = candidateId;

            return View(docObjects);
        }

        public async Task<IActionResult> ViewDocument(string key)
        {
            var stream = await _s3Service.GetFileAsync(key);

            return File(stream, "application/pdf");
        }

        public async Task<IActionResult> DownloadDocument(string key)
        {
            var stream = await _s3Service.GetFileAsync(key);
            return File(stream, "application/pdf", "document.pdf");

        }

        [HttpPost]
        public async Task<IActionResult> VerifyDocument(int candidateId)
        {
            var applicant = await _dbContext.Applicants
                .FirstOrDefaultAsync(cd => cd.ApplicantId == candidateId);

            if (applicant == null)
                return Json(new { success = false });

            var subject = "HireVault – You have been shortlisted 🎉";
            var body = $@"
            <h3>Congratulations {applicant.FirstName}!</h3>
            <p>Your documents have been <strong>verified sucessfully</strong></p>
            <p>Our HR team will contact you soon regarding the further process.</p>
            <br/>
            <p>Regards,<br/>HireVault Team</p>";

            await _emailService.SendEmailAsyc(applicant.Email, subject, body);

            TempData["Success"] = "Shortlisted email sent successfully";

            applicant.Status = ApplicantStatus.Verified;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RejectDocument(int candidateId)
        {
            var applicant = await _dbContext.Applicants
                .FirstOrDefaultAsync(cd => cd.ApplicantId == candidateId);

            if (applicant == null)
                return Json(new { success = false });

            applicant.Status = ApplicantStatus.Rejected;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> PendingApplicant(int candidateId, string remarks)
        {
            var applicant = await _dbContext.Applicants
                .FirstOrDefaultAsync(cd => cd.ApplicantId == candidateId);

            if (applicant == null)
                return Json(new { success = false });

            var subject = "HireVault – Document Review Update";

            var body = $@"
            <h3>Hello {applicant.FirstName},</h3>
            <p>Your application is currently marked as <strong>Pending</strong>.</p>
            <p><strong>Remarks from Admin:</strong></p>
            <p style='color:red;'>{remarks}</p>
            <br/>
            <p>Please re-upload the required documents at the earliest.</p>
            <br/>
            <p>Regards,<br/>HireVault Team</p>";

            await _emailService.SendEmailAsyc(applicant.Email, subject, body);

            applicant.Status = ApplicantStatus.Pending;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
