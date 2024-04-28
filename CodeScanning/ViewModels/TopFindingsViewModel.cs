using CodeScanning.Controllers;
using CodeScanning.Models;
using CodeScanning.Services;
using System.Collections.ObjectModel;

namespace CodeScanning.ViewModels
{
    public class TopFindingsViewModel
    {
        public Settings Settings { get; set; }
        public DefectDojoTopFindings Findings { get; set; }
        public bool SettingsNotFound { get; set; }
    }
}