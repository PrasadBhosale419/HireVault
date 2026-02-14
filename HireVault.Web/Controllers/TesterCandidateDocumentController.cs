using HireVault.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireVault.Web.Controllers
{
    public class TesterCandidateDocumentController : Controller
    {
        private readonly HireVaultDbContext _dbContext;

        public TesterCandidateDocumentController(HireVaultDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: TesterCandidateDocumentController
        [HttpGet("gettestdocuments")]
        public ActionResult Index()
        {
            //var sql = @"
            //DELETE FROM CandidateDocuments
            //WHERE ISNUMERIC(CandidateId) = 0
            //";

            //_dbContext.Database.ExecuteSqlRaw(sql);
            //dbContext.SaveChanges();


            return Content("Deleted all rows with invalid CandidateId.");
        }

        // GET: TesterCandidateDocumentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TesterCandidateDocumentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TesterCandidateDocumentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TesterCandidateDocumentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TesterCandidateDocumentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: TesterCandidateDocumentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var document = _dbContext.CandidateDocuments.FirstOrDefault(x => x.CandidateId == id);

            if (document == null)
            {
                return NotFound();
            }

            _dbContext.CandidateDocuments.Remove(document);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
