using CodeScanning.Data;
using CodeScanning.Models;
using CodeScanning.Services;
using CodeScanning.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static System.Net.WebRequestMethods;


namespace CodeScanning.Controllers
{
    public class TopFindingsBySeverity : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public TopFindingsBySeverity(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            TopFindingsViewModel model = new();
            var settings = _context.Settings.FirstOrDefault();
            if (settings != null)
            {
                model.Settings = settings;
                var httpClient = new HttpClient();
                model.Findings = new DefectDojoTopFindings(httpClient, settings.defectDojoApiToken);
                await model.Findings.GetFindingsAsync();
                if (model.Findings.getApiError())
                {
                    return View("Errors/DDApiError");
                }
            }
            else
            {
                model.SettingsNotFound = true;
            }

            return View("TopFindingsIndex", model);
        }

    }
}
