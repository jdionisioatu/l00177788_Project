using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace CodeScanning.Services
{
    public class GitHubApiService
    {
        public int numTotalPages { get; set; }
        public Collection<RepositoryItem> repositories; //{ get; set; }
        private string gitHubPAT;
        private string gitHubUserOrOrganization;
        private bool organizational;
        private bool apiError = false;

        private readonly HttpClient _httpClient;

        public GitHubApiService(HttpClient httpClient, string GitHubPAT, string GitHubUserOrOrganization, bool Organizational) {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            gitHubPAT = GitHubPAT;
            organizational = Organizational;
            gitHubUserOrOrganization = GitHubUserOrOrganization;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitHubPAT);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CodeScanningResearch", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            repositories = new Collection<RepositoryItem>();
        }

        public async Task GetRepositoriesAsync()
        {
            //var repositories = new Collection<RepositoryItem>();
            var page = 1;

            var uri = new String("");
            while (true)
            {
                if (organizational)
                {
                    uri = new String($"/orgs/{gitHubUserOrOrganization}/repos?per_page=30&page={page}");
                } else
                {
                    uri = new String($"/users/{gitHubUserOrOrganization}/repos?per_page=30&page={page}");
                }
                var response = await _httpClient.GetAsync(uri);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    apiError = true;
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var githubRepositories = JsonSerializer.Deserialize<List<GithubRepository>>(content);

                if (githubRepositories == null || githubRepositories.Count == 0)
                {
                    break;
                }

                //repositories.Add(new Collection<RepositoryItem>());
                foreach (var repo in githubRepositories)
                {
                    var branches = await GetBranchesForRepository(gitHubUserOrOrganization, repo.name);
                    var repositoryItem = new RepositoryItem
                    {
                        Id = repo.id,
                        Name = repo.name,
                        Branches = new List<string>(branches)
                    };

                    this.repositories.Add(repositoryItem);
                }

                page++;
            }
            this.numTotalPages = page - 1;
        }

        private async Task<List<string>> GetBranchesForRepository(string username, string repoName)
        {
            var branches = new List<string>();
            var page = 1;

            while (true)
            {
                var response = await _httpClient.GetAsync($"repos/{username}/{repoName}/branches?per_page=100&page={page}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var githubBranches = JsonSerializer.Deserialize<List<GithubBranch>>(content);

                if (githubBranches == null || githubBranches.Count == 0)
                {
                    break;
                }

                branches.AddRange(githubBranches.ConvertAll(branch => branch.name));
                page++;
            }

            return branches;
        }

        public bool getApiError() { return this.apiError; }

        private class GithubRepository
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        private class GithubBranch
        {
            public string name { get; set; }
        }
    }
}
