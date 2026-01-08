using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Approve()
        {
            return View();
        }
    }
}
