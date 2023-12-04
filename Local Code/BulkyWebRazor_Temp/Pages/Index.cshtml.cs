using BulkyWebRazor_Temp.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RazorApplicationDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, RazorApplicationDbContext db )
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet()
        {

        }
    }
}
