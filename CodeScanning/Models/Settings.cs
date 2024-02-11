using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeScanning.Models
{
	public class Settings : IEntityWithId
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Github API Type is Required")]
		[AllowedValues("Organization","Individual", ErrorMessage = "Github API Type must be either Individual or Organization")]
		public string gitHubApiType { get; set; }

		[Required(ErrorMessage = "A GitHub Pat is required")]
		[RegularExpression("^(gh[ps]_[a-zA-Z0-9]{36}|github_pat_[a-zA-Z0-9]{22}_[a-zA-Z0-9]{59})$", ErrorMessage="The GitHub token must be a valid GitHub token.")]
		public string gitHubToken { get; set; }

		[Required(ErrorMessage = "Defecdojo API Token is required")]
		public string defectDojoApiToken { get; set; }
	}

}
