using AppBusiness.DTOs.CartDTOs;
using AppBusiness.Services;
using eTicaretMvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Text.Json;

namespace eTicaretMvc.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ClaimHelper _claimHelper;

        public CartController(CartService cartService, ClaimHelper claimHelper)
        {
            _cartService = cartService;
            _claimHelper = claimHelper;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _claimHelper.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest("Invalid user ID");
            }

            var cartList = await _cartService.CartList(userId);
                        
            string cartListJson = JsonSerializer.Serialize(cartList);
            Response.Cookies.Append("cartList", cartListJson, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true
            });

            return View(cartList);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCardItem([FromBody] CartAddDTO cartAddDto)
        {
            // Check if user is seller
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == "Seller")
            {
                return BadRequest("Sellers cannot add items to cart");
            }

            var userId = _claimHelper.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest("Invalid user ID");
            }

            cartAddDto.UserId = userId;

            var cartItemResponse = await _cartService.AddCartItem(cartAddDto);

            // Update cart cookie
            var cartList = await _cartService.CartList(userId);
            string cartListJson = JsonSerializer.Serialize(cartList);
            Response.Cookies.Append("cartList", cartListJson, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true
            });

            // Return cart info for dynamic update
            return Ok(new
            {
                count = cartList.Count,
                total = cartList.Sum(c => c.TotalPrice)
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartAddDTO cartAddDto)
        {
            try
            {
                var userId = _claimHelper.GetUserId(User);

                if (userId == 0)
                {
                    return BadRequest("Invalid user ID");
                }

                cartAddDto.UserId = userId;

                var cartItemResponse = await _cartService.UpdateCartItem(cartAddDto);

                if (cartItemResponse)
                {
                    // Update cart cookie
                    var cartList = await _cartService.CartList(userId);
                    string cartListJson = JsonSerializer.Serialize(cartList);
                    Response.Cookies.Append("cartList", cartListJson, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        HttpOnly = true
                    });

                    
                    return Ok(new
                    {
                        count = cartList.Count,
                        total = cartList.Sum(c => c.TotalPrice)
                    });
                }
                else
                {
                    return BadRequest("Failed to update cart item");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem([FromBody] int productId)
        {
            try
            {
                var userId = _claimHelper.GetUserId(User);

                if (userId == 0)
                {
                    return BadRequest("Invalid user ID");
                }

                var deleted = await _cartService.DeleteCartItem(userId, productId);

                if (deleted)
                {
                    // Update cart cookie
                    var cartList = await _cartService.CartList(userId);
                    string cartListJson = JsonSerializer.Serialize(cartList);
                    Response.Cookies.Append("cartList", cartListJson, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        HttpOnly = true
                    });

                    // Return cart info for dynamic update
                    return Ok(new
                    {
                        count = cartList.Count,
                        total = cartList.Sum(c => c.TotalPrice)
                    });
                }
                else
                {
                    return BadRequest("Failed to delete cart item");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
