using System.Collections.ObjectModel;

namespace CodeScanning.Services
{
    public class RepositoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<String> Branches { get; set; }
    }
}
