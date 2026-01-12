using HireVault.Core.Entities;
using HireVault.Infrastructure.Data;
using HireVault.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireVault.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HireVaultDbContext _dbContext;
        private readonly IEmailService _emailService;

        public DashboardController(HireVaultDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddShortListCandidates(Applicants applicant)
        {
            var existingCandidate = _dbContext.Applicants.FirstOrDefault(x => x.Email == applicant.Email);

            if (existingCandidate != null)
            {
                return BadRequest("The candidate with given email is already shortlisted");
            }

            applicant.CreatedAt = DateTime.UtcNow;
            _dbContext.Applicants.Add(applicant);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Shortlist()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllCandidates()
        {
            var candidates = _dbContext.Applicants
                .Select(a => new Applicants
                {
                    ApplicantId = a.ApplicantId,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            return View(candidates);
        }

        [HttpPost("Dashboard/NotifyCandidate/{candidateId}")]
        [AllowAnonymous]
        public async Task<IActionResult> NotifyCandidate([FromRoute] int candidateId)
        {
            var candidate = _dbContext.Applicants.FirstOrDefault(c => c.ApplicantId == candidateId);
            if (candidate == null)
                return NotFound("Candidate not found");

            var subject = "HireVault – You have been shortlisted 🎉";
            var body = $@"
            <h3>Congratulations {candidate.FirstName}!</h3>
            <p>You have been <strong>shortlisted</strong> for the next stage of the hiring process.</p>
            <p>Our HR team will contact you soon.</p>
            <br/>
            <p>Regards,<br/>HireVault Team</p>";

            await _emailService.SendEmailAsyc(candidate.Email, subject, body);

            TempData["Success"] = "Shortlisted email sent successfully";
            return Ok(); // return OK for fetch
        }
    }
}
