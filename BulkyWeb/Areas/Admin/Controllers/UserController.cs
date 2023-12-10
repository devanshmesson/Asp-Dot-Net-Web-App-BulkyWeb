using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        public ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APICalls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = _db.ApplicationUser.Include(x => x.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in users)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id)?.RoleId;
                if (roleId != null)
                {
                    user.Role = roles.FirstOrDefault(x => x.Id == roleId)?.Name;

                }
                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string userId)
        {
            //Lock/Unlock functionality
            var user = _db.ApplicationUser.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return Json(new { Success = false, message = "Error while locking/unlocking" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                //User is currently locked and we need to unlock the user
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddDays(30);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion

    }
}
