namespace CodeScanning.Models
{
    public class GitHubIndividualModel : IEntityWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }

    }
}
