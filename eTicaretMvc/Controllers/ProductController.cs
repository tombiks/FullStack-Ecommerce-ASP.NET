using AppBusiness.DTOs.ProductDTOs;
using AppBusiness.Services;
using eTicaretMvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretMvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CommentService _commentService;
        private readonly ClaimHelper _claimHelper;

        public ProductController(ProductService productService, CommentService commentService, ClaimHelper claimHelper)
        {
            _productService = productService;
            _commentService = commentService;
            _claimHelper = claimHelper;
        }
        public async Task<IActionResult> List()
        {
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductAddDTO productAddDTO)
        {
            try
            {
                // Login olan kullanıcının ID'sini al (SellerId)
                var sellerId = _claimHelper.GetUserId(User);

                if (sellerId == 0)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgisi alınamadı.";
                    return RedirectToAction("Details", "Profile");
                }

                // ProductService ile API'ye gönder
                var result = await _productService.ProductAdd(productAddDTO, sellerId);

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ürün eklenemedi: {ex.Message}";
            }

            return RedirectToAction("Details", "Profile");
        }


        [Authorize(Roles = "Seller")]
        [HttpGet]
        public IActionResult Edit(int productId)
        {
            // productId'yi View'a gönder (Edit formunda hidden input olarak kullanılacak)
            ViewBag.ProductId = productId;
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProductEditDTO productEditDTO)
        {
            try
            {
                // ProductService ile API'ye gönder
                var result = await _productService.ProductEdit(productEditDTO);

                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ürün güncellenemedi: {ex.Message}";
            }

            return RedirectToAction("Details", "Profile");
        }


        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int productId)
        {
            try
            {
                var deleteProduct = await _productService.ProductDelete(productId);
                TempData["SuccessMessage"] = "Product silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Profile");
        }

        public IActionResult Comment()
        {
            return View();
        }

        [HttpGet("Detail/{productId}")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var productDetails = await _productService.ProductDetailsService(productId);

            if (productDetails == null)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            // Get confirmed comments for this product
            var comments = await _commentService.GetProductComments(productId);
            ViewBag.Comments = comments;

            // Check if user can comment (only for authenticated buyers)
            bool canComment = false;
            int userId = 0;
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                Console.WriteLine($"[ProductController] User authenticated. Role: {userRole}");

                if (userRole != "Seller")
                {
                    userId = _claimHelper.GetUserId(User);
                    Console.WriteLine($"[ProductController] UserId from claims: {userId}, ProductId: {productId}");

                    if (userId != 0)
                    {
                        canComment = await _commentService.CanUserComment(userId, productId);
                        Console.WriteLine($"[ProductController] CanComment result: {canComment}");
                    }
                    else
                    {
                        Console.WriteLine($"[ProductController] UserId is 0, cannot check comment permission");
                    }
                }
            }
            ViewBag.CanComment = canComment;
            ViewBag.UserId = userId;

            return View(productDetails);
        }
    }
}
