using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var product = _unitOfWork.OrderHeader.GetAll(IncludeProperties: "ApplicationUser").ToList();
            return Json(new { data = product });

        }
        #endregion
    }
}
