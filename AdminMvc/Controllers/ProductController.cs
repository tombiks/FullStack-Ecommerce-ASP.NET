using AppBusiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var products = await _productService.ProductViewService();

            return View(products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int productId)
        {
            try
            {
                var deleteProduct = await _productService.ProductDelete(productId);
                TempData["SuccessMessage"] = "Product başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List", "Product");            
        }
    }
}
