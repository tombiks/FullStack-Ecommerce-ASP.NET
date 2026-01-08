using AppBusiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var waitingComments = await _commentService.GetWaitingComments();
                return View(waitingComments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading comments: {ex.Message}";
                return View(new List<AppBusiness.DTOs.CommentDTOs.CommentAdminDTO>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int commentId)
        {
            try
            {
                var result = await _commentService.ConfirmComment(commentId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Comment confirmed successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to confirm comment";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int commentId)
        {
            try
            {
                var result = await _commentService.RejectComment(commentId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Comment rejected successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reject comment";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
