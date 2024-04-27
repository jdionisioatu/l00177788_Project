using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeScanning.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public bool launchSuccessfull;
        public bool displayMessage;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            displayMessage = false;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
    }
}
