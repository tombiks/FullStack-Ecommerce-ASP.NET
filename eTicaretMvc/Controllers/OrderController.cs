using AppBusiness.DTOs.CartDTOs;
using AppBusiness.Services;
using eTicaretMvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eTicaretMvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly ClaimHelper _claimHelper;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrderController(ClaimHelper claimHelper, OrderService orderService, CartService cartService)
        {
            _claimHelper = claimHelper;
            _orderService = orderService;
            _cartService = cartService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteOrder()
        {
            var userId = _claimHelper.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest("Invalid user ID");
            }

            // Get cart items from cookie
            var cartListJson = Request.Cookies["cartList"];

            if (string.IsNullOrEmpty(cartListJson))
            {
                return BadRequest("Cart is empty");
            }

            var cartList = JsonSerializer.Deserialize<List<CartShowDTO>>(cartListJson);

            if (cartList == null || !cartList.Any())
            {
                return BadRequest("Cart is empty");
            }

            try
            {
                // Call service to create order
                bool orderCreated = await _orderService.CompleteOrder(cartList, userId);

                if (orderCreated)
                {
                    // Clear cart from database
                    await _cartService.ClearCart(userId);

                    // Clear cart cookie
                    Response.Cookies.Delete("cartList");
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to create order");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

    }
}
