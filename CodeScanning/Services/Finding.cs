namespace CodeScanning.Services
{
    public class Finding
    {
        public int Id { get; set; }
        public List<string> ?Tags { get; set; }
        public string? Title { get; set; }
        public string? Severity { get; set; }
        public string? Description { get; set; }
        public DateTime? LastStatusUpdate { get; set; }
        public string? NumericalSeverity { get; set; }
        public int? Line { get; set; }
        public string? FilePath { get; set; }
        public DateTime? Created { get; set; }
        
    }
}
