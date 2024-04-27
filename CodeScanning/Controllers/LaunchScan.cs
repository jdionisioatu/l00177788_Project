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
            var buildargs = new Dictionary<string, string>
            {
                { "repository_name", Name },
                { "branch", Branch },
                { "defectdojourl", "https://defectdojo.collegefan.org" },
                { "defectdojotoken", settings.defectDojoApiToken },
                { "giturl",  "https://" + settings.gitHubUserNameOrOrgName + ":" +
                    settings.gitHubToken + "@github.com/" + settings.gitHubUserNameOrOrgName + "/" + Name + ".git"}
            };
            var imageBuildParameters = new ImageBuildParameters
            {
                BuildArgs = buildargs,
                Tags = ["code-scanning"]
            };
            string fullPath = _webHostEnvironment.WebRootPath + "/docker";
            using var tarball = CreateTarballForDockerfileDirectory(fullPath);
            using var dockerClient = new DockerClientConfiguration().CreateClient();
            IProgress<JSONMessage> progress1 = new Progress<JSONMessage>();
            IEnumerable<AuthConfig> authConfig = new List<AuthConfig>();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            await dockerClient.Images.BuildImageFromDockerfileAsync(imageBuildParameters, tarball, authConfig, headers, progress1);

            tarball.Dispose();
            return RedirectToAction("Index", "Home");
        }

        private static Stream CreateTarballForDockerfileDirectory(string directory)
        {
            var tarball = new MemoryStream();
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

            using var archive = new TarOutputStream(tarball)
            {
                //Prevent the TarOutputStream from closing the underlying memory stream when done
                IsStreamOwner = false
            };

            foreach (var file in files)
            {
                //Replacing slashes as KyleGobel suggested and removing leading /
                string tarName = file.Substring(directory.Length).Replace('\\', '/').TrimStart('/');

                //Let's create the entry header
                var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateTarEntry(tarName);
                using var fileStream = System.IO.File.OpenRead(file);
                entry.Size = fileStream.Length;
                entry.TarHeader.Mode = Convert.ToInt32("100755", 8); //chmod 755
                archive.PutNextEntry(entry);

                //Now write the bytes of data
                byte[] localBuffer = new byte[32 * 1024];
                while (true)
                {
                    int numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
                    if (numRead <= 0)
                        break;

                    archive.Write(localBuffer, 0, numRead);
                }

                //Nothing more to do with this entry
                archive.CloseEntry();
            }
            archive.Close();

            //Reset the stream and return it, so it can be used by the caller
            tarball.Position = 0;
            return tarball;
        }
    }
}
