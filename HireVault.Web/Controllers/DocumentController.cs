using HireVault.Core.Interfaces;
using HireVault.Infrastructure.Data;
using HireVault.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HireVault.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IS3Service _s3Service;

        public HireVaultDbContext _dbContext { get; }

        public DocumentController(IS3Service s3Service, HireVaultDbContext dbContext)
        {
            _s3Service = s3Service;
            _dbContext = dbContext;
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
                                               UploadedAt = g.Max(x => DateTime.Parse(x.UploadedAt))
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

            return View(docObjects);
        }

        public async Task<IActionResult> ViewDocument(string key)
        {
            var stream = await _s3Service.GetFileAsync(key);

            return File(stream, "application/pdf");
        }

    }
}
