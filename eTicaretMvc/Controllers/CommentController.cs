using AppBusiness.DTOs.CommentDTOs;
using AppBusiness.Services;
using eTicaretMvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretMvc.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;
        private readonly ClaimHelper _claimHelper;

        public CommentController(CommentService commentService, ClaimHelper claimHelper)
        {
            _commentService = commentService;
            _claimHelper = claimHelper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentAddDTO commentDto)
        {
            try
            {
                // Get user ID from claims
                var userId = _claimHelper.GetUserId(User);

                if (userId == 0)
                {
                    return BadRequest("Invalid user ID");
                }

                // Check if user is seller
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole == "Seller")
                {
                    return BadRequest("Sellers cannot add comments");
                }

                commentDto.UserId = userId;

                // Check if user can comment
                var canComment = await _commentService.CanUserComment(userId, commentDto.ProductId);
                if (!canComment)
                {
                    return BadRequest("You cannot comment on this product. You either haven't ordered it or have already commented.");
                }

                // Add comment
                var result = await _commentService.AddComment(commentDto);

                if (!result)
                {
                    return Conflict("Failed to add comment");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
