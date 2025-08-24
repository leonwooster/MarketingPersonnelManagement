using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyA.WebUI.Pages
{
    public class PersonnelModel : PageModel
    {
        private readonly ILogger<PersonnelModel> _logger;

        public PersonnelModel(ILogger<PersonnelModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}