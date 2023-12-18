using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.DependencyResolver;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var product = _unitOfWork.Product.GetAll(IncludeProperties: "Category").ToList();
            return View(product);
        }

        /* public IActionResult Delete(int? id)
         {
             if (id != null || id > 0)
             {
                 IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                 {
                     Text = x.Name,
                     Value = x.Id.ToString()
                 });
                 ProductVM productVM = new()
                 {
                     Product = _unitOfWork.Product.Get(x => x.Id == id),
                     CategoryList = CategoryList
                 };

                 return View(productVM);
             }
             return NotFound();
         }*/

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Product.Get(x => x.Id == id);

            var wwwrootPath = _webHostEnvironment.WebRootPath;
            // var imagePath = product.ImageUrl.TrimStart('\\');
            //var fullImagePath = Path.Combine(wwwrootPath, imagePath);

            /* if(System.IO.File.Exists(fullImagePath))
             {
                 System.IO.File.Delete(fullImagePath);
             }*/
            if (product != null)
            {
                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();
            }
            return Json(new { success = true, message = "Delete Successful" });
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = CategoryList
            };

            if (id == null || id <= 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(x => x.Id == id);
                return View(productVM);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, List<IFormFile>? files)
        {
            if (int.TryParse(obj.Product.Title, out _))
            {
                ModelState.AddModelError("Title", "Title cannot be null");
            }
            if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save();
                if (files != null)
                {
                    var wwwrootPath = _webHostEnvironment.WebRootPath;
                    foreach (var file in files)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var productPath = @"images/products/product-" + obj.Product.Id;
                        var finalPath = Path.Combine(wwwrootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);


                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        };

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" +  fileName,
                            ProductId = obj.Product.Id,
                        };

                        if(obj.Product.ProductImages == null)
                        {
                            obj.Product.ProductImages = new List<ProductImage>();
                        }

                        obj.Product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                }

                TempData["success"] = "Product details created/updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var product = _unitOfWork.Product.GetAll(IncludeProperties: "Category").ToList();
            return Json(new { data = product });

        }
        #endregion

    }
}