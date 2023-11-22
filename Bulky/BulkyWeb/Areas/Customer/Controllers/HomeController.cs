using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll(IncludeProperties: "Category").ToList();
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new() {
            
                Product = _unitOfWork.Product.Get(x => x.Id == productId, IncludeProperties: "Category"),
                ProductId = productId,
                Count = 1
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = ApplicationUserId;

            var existingCart = _unitOfWork.ShoppingCart.Get(x => x.ApplicationUserId == ApplicationUserId && x.ProductId == cart.ProductId);
            if (existingCart != null)
            {
                existingCart.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(existingCart);
                _unitOfWork.Save();

            }
            else
            {
              _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();

           
            }

            HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitOfWork.ShoppingCart.GetAll(x => cart.ApplicationUserId == ApplicationUserId).Count());


            TempData["success"] = "Cart Updated SuccessFully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
