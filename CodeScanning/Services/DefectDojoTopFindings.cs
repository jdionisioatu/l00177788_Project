using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeScanning.Services
{
    public class DefectDojoTopFindings
    {
        public Collection<Finding> findings;
        private readonly string defectDojoApiKey;
        private readonly HttpClient _httpClient;

        public DefectDojoTopFindings(HttpClient httpClient, string defectDojoApiKey)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://defectdojo.collegefan.org");
            this.defectDojoApiKey = defectDojoApiKey;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", defectDojoApiKey);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CodeScanningResearch", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            findings = new Collection<Finding>();
        }

        public async Task GetFindingsAsync()
        {
            var uri = new String("api/v2/findings/?limit=50&o=severity");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiresponse = JsonSerializer.Deserialize<ApiResponse>(content);
            if (apiresponse == null || apiresponse.count == 0)
            {
                return;
            }
            foreach (var finding in apiresponse.results)
            {
                var newfinding = new Finding
                {
                    Id = finding.id,
                    Tags = finding.tags,
                    Title = finding.title,
                    Severity = finding.severity,
                    Description = finding.description,
                    LastStatusUpdate = finding.last_status_update,
                    NumericalSeverity = finding.numerical_severity,
                    Line = finding.line,
                    FilePath = finding.file_path,
                    Created = finding.created
                };
                findings.Add(newfinding);
            }
        } 
        
        public class ApiResponse
        {
            public int count { get; set; }
            public string next { get; set; }
            public string previous { get; set; }
            public List<Result> results { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public List<string> tags { get; set; }
            public string title { get; set; }
            public string severity { get; set; }
            public string description { get; set; }
            public DateTime last_status_update { get; set; }
            public string numerical_severity { get; set; }
            public int line { get; set; }
            public string file_path { get; set; }
            public DateTime created { get; set; }
        }
    }
}

