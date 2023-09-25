using BulkyWebRazor_Temp.data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly RazorApplicationDbContext _db;
        [BindProperty]
        public Category Category { get; set; }
        public DeleteModel(RazorApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int ?id)
        {
            if(id!=0 && id!=null)
            {
                Category = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            _db.Categories.Remove(Category);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted SuccessFully";
            return RedirectToPage("Index");    
        }
    }
}
