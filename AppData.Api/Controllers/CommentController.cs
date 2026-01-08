using AppBusiness.DTOs.CommentDTOs;
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
    public class CommentController : ControllerBase
    {
        private readonly IDataRepository _dataRepo;
        private readonly AppDbContext _db;

        public CommentController(IDataRepository dataRepo, AppDbContext db)
        {
            _dataRepo = dataRepo;
            _db = db;
        }

        // Check if user has ordered the product
        [HttpGet("cancomment/{userId}/{productId}")]
        public async Task<IActionResult> CanUserComment(int userId, int productId)
        {
            // Debug logging
            Console.WriteLine($"[CanUserComment] Checking - UserId: {userId}, ProductId: {productId}");

            // Check if user has ordered this product
            var orderItems = await _db.Set<OrderItemEntity>()
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == productId && oi.Order!.UserId == userId)
                .ToListAsync();

            var hasOrdered = orderItems.Any();
            Console.WriteLine($"[CanUserComment] HasOrdered: {hasOrdered}, OrderItems count: {orderItems.Count}");

            if (!hasOrdered)
            {
                return Ok(new { canComment = false, reason = "not_ordered", debug = new { userId, productId, orderItemsCount = 0 } });
            }

            // Check if user has already commented on this product
            var existingComments = await _db.Set<ProductCommentEntity>()
                .Where(pc => pc.ProductId == productId && pc.UserId == userId)
                .ToListAsync();

            var hasCommented = existingComments.Any();
            Console.WriteLine($"[CanUserComment] HasCommented: {hasCommented}, Comments count: {existingComments.Count}");

            return Ok(new {
                canComment = !hasCommented,
                reason = hasCommented ? "already_commented" : "can_comment",
                debug = new {
                    userId,
                    productId,
                    orderItemsCount = orderItems.Count,
                    existingCommentsCount = existingComments.Count
                }
            });
        }

        // Add new comment (IsConfirmed = false by default)
        [HttpPost("add")]
        public async Task<IActionResult> AddComment(CommentAddDTO commentDto)
        {
            var comment = new ProductCommentEntity
            {
                ProductId = commentDto.ProductId,
                UserId = commentDto.UserId,
                Text = commentDto.Text,
                StarCount = commentDto.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            bool added = await _dataRepo.Add<ProductCommentEntity>(comment);

            if (!added)
            {
                return Conflict("Yorum eklenemedi");
            }

            return Ok();
        }

        // Get confirmed comments for a product
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductComments(int productId)
        {
            var comments = await _db.Set<ProductCommentEntity>()
                .Include(c => c.User)
                .Where(c => c.ProductId == productId && c.IsConfirmed == true)
                .Select(c => new CommentShowDTO
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    UserId = c.UserId,
                    UserName = c.User!.FirstName + " " + c.User.LastName,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    CreatedAt = c.CreatedAt
                })
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(comments);
        }

        // Get all comments for admin (waiting for approval)
        [HttpGet("admin/waiting")]
        public async Task<IActionResult> GetWaitingComments()
        {
            var comments = await _db.Set<ProductCommentEntity>()
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.IsConfirmed == false)
                .Select(c => new CommentAdminDTO
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product!.Name,
                    UserId = c.UserId,
                    UserName = c.User!.FirstName + " " + c.User.LastName,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    CreatedAt = c.CreatedAt,
                    IsConfirmed = c.IsConfirmed
                })
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(comments);
        }

        
        [HttpGet("admin/waiting/count")]
        public async Task<IActionResult> GetWaitingCommentsCount()
        {
            var count = await _db.Set<ProductCommentEntity>()
                .CountAsync(c => c.IsConfirmed == false);

            return Ok(count);
        }

        // Confirm comment
        [HttpPut("admin/confirm/{commentId}")]
        public async Task<IActionResult> ConfirmComment(int commentId)
        {
            var comment = await _db.Set<ProductCommentEntity>()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound("Yorum bulunamadı");
            }

            comment.IsConfirmed = true;
            bool updated = await _dataRepo.Update<ProductCommentEntity>(comment);

            if (!updated)
            {
                return Conflict("Yorum onaylanamadı");
            }

            return Ok();
        }

        
        [HttpDelete("admin/reject/{commentId}")]
        public async Task<IActionResult> RejectComment(int commentId)
        {
            var comment = await _db.Set<ProductCommentEntity>()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound("Yorum bulunamadı");
            }

            bool deleted = await _dataRepo.Delete<ProductCommentEntity>(comment);

            if (!deleted)
            {
                return Conflict("Yorum silinemedi");
            }

            return Ok();
        }
    }
}
