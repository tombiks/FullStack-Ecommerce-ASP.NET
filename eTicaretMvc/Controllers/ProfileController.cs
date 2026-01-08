using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using AppBusiness.Services;
using AppBusiness.DTOs.UserDTOs;
using AppBusiness.DTOs.OrderDTOs;
using eTicaretMvc.Helpers;

namespace eTicaretMvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserService _userService;
        private readonly ClaimHelper _claimHelper;
        private readonly CategoryService _categoryService;
        private readonly OrderService _orderService;

        public ProfileController(UserService userService, ClaimHelper claimHelper, CategoryService categoryService, OrderService orderService)
        {
            _userService = userService;
            _claimHelper = claimHelper;
            _categoryService = categoryService;
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var userId = id ?? _claimHelper.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest("Geçersiz kullanıcı ID'si");
            }

            var userInfo = await _userService.ShowUserService(userId);

            // Kategorileri ViewBag ile View'a gönder (modal'da kullanılacak)
            ViewBag.Categories = await _categoryService.CategoryViewService();

            // Get orders for buyers
            if (!userInfo.IsSeller)
            {
                try
                {
                    var orders = await _orderService.GetUserOrders(userId);
                    ViewBag.Orders = orders;
                }
                catch
                {
                    ViewBag.Orders = new List<OrderShowDTO>();
                }
            }

            return View(userInfo);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int userId, UpdateUserDTO updateUserDTO)
        {
            var newUserCookie = await _userService.UpdateUserService(userId, updateUserDTO);

            var tokenString = _claimHelper.ClaimsIdendityHelper(newUserCookie);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true, //js ile erişilemesin
                Secure = true, //https kullanabilsin
                SameSite = SameSiteMode.Strict //bu token sadece bu sitede geçerli olsun.
            });

            return RedirectToAction("Details", "Profile");
        }        
    }
}
