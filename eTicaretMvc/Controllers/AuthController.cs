using AppBusiness.DTOs.AuthDTOs;
using eTicaretMvc.Helpers;
using AppBusiness.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace eTicaretMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ClaimHelper _claimHelper;
        private readonly CartService _cartService;

        public AuthController(AuthService authService, ClaimHelper claimHelper, CartService cartService)
        {
            _authService = authService;
            _claimHelper = claimHelper;
            _cartService = cartService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login ([FromForm]AuthLoginDTO AuthLoginDTO) 
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Giriş işlemi başarısız.";

                return View();
            }

            var user = await _authService.LoginService(AuthLoginDTO);

            if (user == null)
            {
                ViewBag.Error = "Giriş işlemi başarısız.";
                return View();
            }

            var tokenString = _claimHelper.ClaimsIdendityHelper(user);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true, //js ile erişilemesin
                Secure = true, //https kullanabilsin
                SameSite= SameSiteMode.Strict //bu token sadece bu sitede geçerli olsun.
            });

            // Set cart cookie for buyers
            if (user.Role != "Seller")
            {
                try
                {
                    var cartList = await _cartService.CartList(user.Id);
                    string cartListJson = JsonSerializer.Serialize(cartList);
                    Response.Cookies.Append("cartList", cartListJson, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        HttpOnly = true
                    });
                }
                catch
                {
                    // Cart loading failed, continue without error
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register (AuthRegisterDTO authRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Kayıt işlemi başarısız";
                return View();
            }

            bool mahmutBool = await _authService.RegisterService(authRegisterDTO);

            if (!mahmutBool)
            {
                ViewBag.Error = "Kayıt işlemi başarısız";
                return View();
            }           

            return RedirectToAction("Login", "Auth");

        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("cartList");

            return RedirectToAction("Login", "Auth");
        }
    }
}
