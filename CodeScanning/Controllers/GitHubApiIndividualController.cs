using CodeScanning.Data;
using Microsoft.AspNetCore.Mvc;

namespace CodeScanning.Controllers
{
    public class GitHubApiIndividualController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GitHubApiIndividualController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
