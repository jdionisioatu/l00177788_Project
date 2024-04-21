using CodeScanning.Services;
using CodeScanning.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CodeScanning.ViewModels
{
    public class LaunchScanViewModel
    {
        public bool SettingsNotFound { get; set; }
        public Settings Settings { get; set; }
        public GitHubApiService  GitHubApiServiceModel { get; set; }
        
    }
}
