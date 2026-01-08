using AdminMvc.Helpers;
using AppBusiness.DTOs.AuthDTOs;
using AppBusiness.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ClaimHelper _claimHelper;

        public AuthController(AuthService authService, ClaimHelper claimHelper)
        {
            _authService = authService;
            _claimHelper = claimHelper;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] AuthLoginDTO AuthLoginDTO)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Error = "Giriş işlemi başarısız";
                return View();
            }

            var user = await _authService.LoginService(AuthLoginDTO);

            if (user is null || user?.Role != "Admin")
            {
                ViewBag.Error = "Hatalı giriş yaptınız, böyle bir admin üyesi yok.";
                return View();
            }

            var tokenString = _claimHelper.ClaimsIdendityHelper(user);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true, //js ile erişilemesin
                Secure = true, //https kullanabilsin
                SameSite = SameSiteMode.Strict //bu token sadece bu sitede geçerli olsun.
            });

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public async Task<IActionResult> Logout() 
        {
            Response.Cookies.Delete("access_token");

            return RedirectToAction("Login", "Auth");
        }
    }
}
