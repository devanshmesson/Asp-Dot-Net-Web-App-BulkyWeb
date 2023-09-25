using BulkyWebRazor_Temp.data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly RazorApplicationDbContext _db;
        public List<Category> CategoryList { get; set; }
        public IndexModel(RazorApplicationDbContext db)
        {
                _db = db;
        }
        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }
    }
}
