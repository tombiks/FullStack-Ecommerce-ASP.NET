using AppBusiness.DTOs.CartDTOs;
using AppBusiness.DTOs.OrderDTOs;
using AppData.Context;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IDataRepository _dbRepo;
        private readonly AppDbContext _db;

        public OrderController(IDataRepository dbRepo, AppDbContext db)
        {
            _dbRepo = dbRepo;
            _db = db;
        }

        [HttpPost("createorder")]
        public async Task<IActionResult> CompletedOrder(List<CartShowDTO> cartList, int userId)
        {
            var completedOrder = new OrderEntity
            {
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
            };

            bool orderEkle = await _dbRepo.Add<OrderEntity>(completedOrder);

            if (orderEkle)
            {
                foreach (var cart in cartList)
                {
                    var orderItem = new OrderItemEntity
                    {
                        OrderId = completedOrder.Id,
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        UnitPrice = cart.ProductPrice,
                        CreatedAt = DateTime.UtcNow
                    };

                    bool orderItemEkle = await _dbRepo.Add<OrderItemEntity>(orderItem);
                }

                return Ok();
            }

            else
            {
                return Conflict("Order oluşturulmadı.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowOrder(int userId)
        {
            var orderList = await _db.Set<OrderEntity>().Where(o => o.UserId == userId)
                .Select(o => new OrderShowDTO
                {
                    OrderId = o.Id,
                    OrderTime = o.CreatedAt,
                    OrderTotalPrice = _db.Set<OrderItemEntity>()
                        .Where(oi => oi.OrderId == o.Id)
                        .Sum(oi => oi.UnitPrice * oi.Quantity),
                    Items = _db.Set<OrderItemEntity>().Where(oi => oi.OrderId == o.Id)
                        .Select(oi => new OrderItemShowDTO
                        {
                            ProductName = oi.Product!.Name,
                            Quantity = oi.Quantity,
                            TotalPrice = oi.UnitPrice * oi.Quantity
                        }).ToList()
                }).ToListAsync();

            return Ok(orderList);
        }

    }
}
