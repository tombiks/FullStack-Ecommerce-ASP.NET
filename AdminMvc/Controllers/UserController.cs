using AppBusiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userService.UserList();

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                var silindi = await _userService.DeleteUserService(userId);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List", "User");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Approve()
        {
            var users = await _userService.ApproveList();

            return View(users);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Reject(int userId)
        {
            try
            {
                var silindi = await _userService.DeleteUserService(userId);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla reddedildi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Approve", "User");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Confirm(int userId)
        {
            try
            {
                var toenable = await _userService.ToEnableUserService(userId);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla onaylandı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Approve", "User");
        }
    }
}
