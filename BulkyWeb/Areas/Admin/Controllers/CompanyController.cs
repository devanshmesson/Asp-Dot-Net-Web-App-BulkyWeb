using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

        public IActionResult Upsert(int id)
        {
           if(id == 0 || id == null)
            {
                return View(new Company());
            }
            else
            {
                var company = _unitOfWork.Company.Get(x => x.Id == id);
                return View(company);   
            }
        }

        [HttpPost]

        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();
                    TempData["Success"] = "Company Details added successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    _unitOfWork.Save();
                    TempData["Success"] = "Company Details added successfully";
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = _unitOfWork.Company.Get(x => x.Id == id);
            if(company != null)
            {
                _unitOfWork.Company.Remove(company);
                _unitOfWork.Save();
            }
            return Json(new {success = true, message = "delete successfull"});  
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data =  companies});
        }
    }
}
