using AppBusiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly ContactService _contactService;
        private readonly CommentService _commentService;

        public HomeController(UserService userService, ContactService contactService, CommentService commentService)
        {
            _userService = userService;
            _contactService = contactService;
            _commentService = commentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var counts = await _userService.UserCounts();

            ViewBag.sellerSayisi = counts[0];

            ViewBag.buyerSayisi = counts[1];

            ViewBag.waitingSayisi = counts[2];

            var contacts = await _contactService.ContactsViewService();

            ViewBag.contacts = contacts;

            var newspapers = await _contactService.NewspapersViewService();

            ViewBag.newspapers = newspapers;
            
            var waitingCommentsCount = await _commentService.GetWaitingCommentsCount();

            ViewBag.waitingCommentsCount = waitingCommentsCount;

            return View();
        }

      

       
    }
}
