using AppBusiness.DTOs.CartDTOs;
using AppData.Context;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IDataRepository _dataRepo;

        public CartController(AppDbContext db, IDataRepository dataRepo)
        {
            _db = db;
            _dataRepo = dataRepo;
        }

        [HttpPost("addcart")]
        public async Task<IActionResult> AddCartItem(CartAddDTO cartAddDto)
        {
            // Check if item already exists in cart
            var existingCartItem = await _db.Set<CartItemEntity>()
                .FirstOrDefaultAsync(c => c.UserId == cartAddDto.UserId && c.ProductId == cartAddDto.ProductId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity = (byte)(existingCartItem.Quantity + cartAddDto.Quantity);
                bool updated = await _dataRepo.Update<CartItemEntity>(existingCartItem);

                if (updated)
                {
                    return Ok();
                }
                else
                {
                    return Conflict("Cart güncelleme başarısız.");
                }
            }
            else
            {
                // Add new cart item
                var newCartItem = new CartItemEntity
                {
                    ProductId = cartAddDto.ProductId,
                    UserId = cartAddDto.UserId,
                    Quantity = cartAddDto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                bool ekledimi = await _dataRepo.Add<CartItemEntity>(newCartItem);

                if (ekledimi)
                {
                    return Ok();
                }
                else
                {
                    return Conflict("Cart ekleme başarısız.");
                }
            }
        }

        [HttpPut("updatecart")]
        public async Task<IActionResult> UpdateCartItem(CartAddDTO cartAddDto)
        {
            // Find existing cart item
            var existingCartItem = await _db.Set<CartItemEntity>()
                .FirstOrDefaultAsync(c => c.UserId == cartAddDto.UserId && c.ProductId == cartAddDto.ProductId);

            if (existingCartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            // Replace quantity (not add to it)
            existingCartItem.Quantity = cartAddDto.Quantity;
            bool updated = await _dataRepo.Update<CartItemEntity>(existingCartItem);

            if (updated)
            {
                return Ok();
            }
            else
            {
                return Conflict("Cart güncelleme başarısız.");
            }
        }

        [HttpDelete("deletecart/{userId}/{productId}")]
        public async Task<IActionResult> DeleteCartItem(int userId, int productId)
        {
            var cartItem = await _db.Set<CartItemEntity>()
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            bool deleted = await _dataRepo.Delete<CartItemEntity>(cartItem);

            if (deleted)
            {
                return Ok();
            }
            else
            {
                return Conflict("Cart item silme başarısız.");
            }
        }

        [HttpDelete("clearcart/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var cartItems = await _db.Set<CartItemEntity>()
                .Where(c => c.UserId == userId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                await _dataRepo.Delete<CartItemEntity>(item);
            }

            return Ok();
        }

        [HttpGet("cartlist/{userId}")]
        public async Task<IActionResult> CartList(int userId)
        {
            var cartList = await _db.Set<CartItemEntity>().Where(c => c.UserId == userId)
                .Select(c => new CartShowDTO
                {
                    ImageUrl = "https://localhost:7201" + c.Product!.Images!.Select(i => i.Url).FirstOrDefault(),
                    ProductId = c.ProductId,
                    ProductName = c.Product!.Name,
                    ProductPrice = c.Product!.Price,
                    Quantity = c.Quantity,
                    TotalPrice = c.Product!.Price * c.Quantity,
                })
                .ToListAsync();

            return Ok(cartList);
        }
    }
}
