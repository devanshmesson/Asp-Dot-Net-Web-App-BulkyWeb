using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ApplicationUserId, IncludeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                double price = GetPrizeBasedOnQuantity(shoppingCart);
                ShoppingCartVM.OrderHeader.OrderTotal += price * shoppingCart.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cardId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == cardId);
            cart.Count += 1;
            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cardId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == cardId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cardId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == cardId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ApplicationUserId, IncludeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == ApplicationUserId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                double price = GetPrizeBasedOnQuantity(shoppingCart);
                ShoppingCartVM.OrderHeader.OrderTotal += price * shoppingCart.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult OrderConfirmation(int Id)
        {
            return View(Id);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var user = (ClaimsIdentity)User.Identity;
            var ApplicationUserId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ApplicationUserId, IncludeProperties: "Product");
            ShoppingCartVM.OrderHeader.ApplicationUserId = ApplicationUserId;
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == ApplicationUserId);
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            foreach (var shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                double price = GetPrizeBasedOnQuantity(shoppingCart);
                ShoppingCartVM.OrderHeader.OrderTotal += price * shoppingCart.Count;
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Customer account
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                //Company account
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = shoppingCart.Count,
                    Price = shoppingCart.Product.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Regular customer
                var domain = "https://localhost:7204/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { Id = ShoppingCartVM.OrderHeader.Id });
        }

        private double GetPrizeBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
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
