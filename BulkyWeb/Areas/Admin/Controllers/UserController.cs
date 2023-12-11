using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        public ApplicationDbContext _db;
        public UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? userId)
        {
            string roleId = _db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
            var user = _db.ApplicationUser.Include(x=> x.Company).FirstOrDefault(x => x.Id == userId);
            var roleList = _db.Roles.ToList();
            var companyList = _db.Companies.ToList();

            RoleManagementVM roleManagementVM = new()
            {
                ApplicationUser = user,
                RoleList = roleList.Select(role => new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                }),
                CompanyList = companyList.Select(company => new SelectListItem
                {
                    Text = company.Name,
                    Value = company.Id.ToString()
                })

            };
            roleManagementVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(x => x.Id == roleId).Name;
            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            var oldRoleId = _db.UserRoles.FirstOrDefault(x => x.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            var oldRoleName = _db.Roles.FirstOrDefault(x => x.Id == oldRoleId).Name;
            var newRoleName = roleManagementVM.ApplicationUser.Role;
            ApplicationUser user = _db.ApplicationUser.FirstOrDefault(x => x.Id == roleManagementVM.ApplicationUser.Id);

            if(oldRoleName != newRoleName)
            {
                if(newRoleName == SD.Role_Company)
                {
                    user.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if(oldRoleName == SD.Role_Company)
                {
                    user.CompanyId = null;
                }
                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(user, oldRoleName).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, newRoleName).GetAwaiter().GetResult();

            }
                return RedirectToAction("Index");


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
