using CodeScanning.Data;
using CodeScanning.Models;
using CodeScanning.Services;
using CodeScanning.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;


namespace CodeScanning.Controllers
{
    public class LaunchScan : Controller
    {
        private readonly ApplicationDbContext _context;
       

        public LaunchScan(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            LaunchScanViewModel model = new();
            var settings = _context.Settings.FirstOrDefault();
            if (settings != null)
            {
                model.Settings = settings;
                bool organizational = false;
                if( settings.gitHubApiType == "Organization")
                {
                    organizational = true;
                }
                var httpClient = new HttpClient();
                model.GitHubApiServiceModel = new GitHubApiService(httpClient ,settings.gitHubToken, settings.gitHubUserNameOrOrgName, organizational);
                await model.GitHubApiServiceModel.GetRepositoriesAsync();
                
            } else
            {
                model.SettingsNotFound = true;
            }

            return View("LaunchScanIndex",model);
        }

        public IActionResult IndexWithPage(int pagenumber)
        {
            return View();
        }
    }
}
