using System.Diagnostics;
using AppBusiness.Services;
using eTicaretMvc.Helpers;
using Microsoft.AspNetCore.Mvc;



namespace eTicaretMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;

        public HomeController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.ProductForFeaturedService();

            if (featuredProducts == null)
            {
                featuredProducts = new List<AppBusiness.DTOs.ProductDTOs.ProductFeaturedDTO>();
            }

            return View(featuredProducts);
        }

        [HttpGet("category/{id}")]
        public async Task<IActionResult> Listing([FromRoute] int id)
        {
            var categoriedProducts = await _productService.ProductForCategoriedService(id);

            if (categoriedProducts == null)
            {
                categoriedProducts = new List<AppBusiness.DTOs.ProductDTOs.ProductCategoriedDTO>();
            }

            return View(categoriedProducts);
        }   

        public IActionResult AboutUs() 
        {
            return View();
        }        

        public IActionResult Contact() 
        {
            return RedirectToAction("AboutUs", "Home");
        }


    }
}
