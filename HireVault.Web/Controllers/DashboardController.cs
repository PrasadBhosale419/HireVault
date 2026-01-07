using HireVault.Core.Entities;
using HireVault.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireVault.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly HireVaultDbContext _dbContext;

        public DashboardController(HireVaultDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddShortListCandidates(Candidates candidate)
        {
            var existingCandidate = _dbContext.Candidates.FirstOrDefault(x => x.Email == candidate.Email);

            if (existingCandidate != null)
            {
                return BadRequest("The candidate with given email is already shortlisted");
            }

            candidate.CreatedAt = DateTime.UtcNow;
            candidate.CandidateId = Guid.NewGuid().ToString();
            _dbContext.Candidates.Add(candidate);
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
            var candidates = _dbContext.Candidates
                .Select(c => new Candidates
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email
                })
                .ToList();

            return View(candidates);
        }


    }
}
