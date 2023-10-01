using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _categoryRepo.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(int.TryParse(obj.Name, out _))
            {
                ModelState.AddModelError("Name", "Category Name cannot be a number");
            }
            if(ModelState.IsValid)
            {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
               TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id ==0) 
            {
                return NotFound();
            }
            Category categoryToEdit = _categoryRepo.Get(category => category.Id == id);
            if (categoryToEdit == null)
            {
                return NotFound();
            }
            return View(categoryToEdit);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (int.TryParse(obj.Name, out _))
            {
                ModelState.AddModelError("Name", "Category Name cannot be a number");
            }
            if (ModelState.IsValid)
            {
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryToDelete = _categoryRepo.Get(category => category.Id == id);
            if (categoryToDelete == null)
            {
                return NotFound();
            }
            return View(categoryToDelete);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category category = _categoryRepo.Get(category => category.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryRepo.Remove(category);
            _categoryRepo.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }

    }
}
