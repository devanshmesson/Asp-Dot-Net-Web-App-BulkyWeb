using BulkyWebRazor_Temp.data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly RazorApplicationDbContext _db;
        
        public Category Category { get; set; }
        public CreateModel(RazorApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "Category Created SuccessFully";
            return RedirectToPage("Index");
        }
    }
}
