using CodeScanning.Data;
using CodeScanning.Models;
using CodeScanning.Services;
using CodeScanning.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Formats.Tar;
using ICSharpCode.SharpZipLib.Tar;
using Docker.DotNet.Models;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static System.Net.WebRequestMethods;


namespace CodeScanning.Controllers
{
    public class LaunchScan : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public LaunchScan(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scan(int Id, string Name, string Branch)
        {
            var settings = _context.Settings.FirstOrDefault();
            string giturl = "https://" + settings.gitHubUserNameOrOrgName + ":" +
                    settings.gitHubToken + "@github.com/" + settings.gitHubUserNameOrOrgName + "/" + Name + ".git";
            var buildargs = new Dictionary<string, string>
            {
                { "repository_name", Name },
                { "branch", Branch },
                { "defectdojourl", "https://defectdojo.collegefan.org" },
                { "defectdojotoken", settings.defectDojoApiToken },
                { "giturl",  giturl}
            };

            const string allowedChars = "0123456789abcdefghijklmnopqrstuvwxyz";
            Random rnd = new Random();
            string randomStringImageName = (Name + LaunchScanHelpers.RandomString(rnd, allowedChars, (10, 10))).ToLower(); 
            var imageBuildParameters = new ImageBuildParameters
            {
                BuildArgs = buildargs,
                Tags = [randomStringImageName]
            };
            string fullPath = _webHostEnvironment.WebRootPath + "/docker";
            using var tarball = LaunchScanHelpers.CreateTarballForDockerfileDirectory(fullPath);
            using var dockerClient = new DockerClientConfiguration().CreateClient();
            IProgress<JSONMessage> progress1 = new Progress<JSONMessage>();
            IEnumerable<AuthConfig> authConfig = new List<AuthConfig>();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            await dockerClient.Images.BuildImageFromDockerfileAsync(imageBuildParameters, tarball, authConfig, headers, progress1);
            tarball.Dispose();

            var containerParameters = new CreateContainerParameters
            {
                Name = randomStringImageName + "run",
                ArgsEscaped = true,
                Image = randomStringImageName,
                Env = [ "defectdojourl=https://defectdojo.collegefan.org", "defectdojotoken=" + settings.defectDojoApiToken,
                    "branch=" + Branch, "giturl=\"" + giturl + "\"", "repository_name=" + Name ]
            };
            CreateContainerResponse createdContainer = await dockerClient.Containers.CreateContainerAsync(containerParameters);

            await dockerClient.Containers.StartContainerAsync(createdContainer.ID, new ContainerStartParameters());
            return RedirectToAction("Index", "Home");
        }
    }
}
