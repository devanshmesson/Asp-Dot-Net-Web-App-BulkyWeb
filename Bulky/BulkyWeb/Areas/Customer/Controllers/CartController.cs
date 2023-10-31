using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartList = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ApplicationUserId, IncludeProperties: "Product")
            };

            foreach(var shoppingCart in shoppingCartList.ShoppingCartList)
            {
                double price = GetPrizeBasedOnQuantity(shoppingCart);
                shoppingCartList.OrderTotal += price * shoppingCart.Count;
            }
            return View(shoppingCartList);
        }

        private double GetPrizeBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        } 
    }
}
