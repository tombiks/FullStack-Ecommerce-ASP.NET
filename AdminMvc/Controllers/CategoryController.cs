using AppBusiness.DTOs.CategoryDTOs;
using AppBusiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminMvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var categorys = await _categoryService.CategoryViewService();
            return View(categorys);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CategoryEditDTO categoryEditDTO)
        {
            try
            {
                var editCategory = await _categoryService.CategoryEdit(categoryEditDTO);
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List", "Category");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm]int categoryId)
        {
            try
            {
                var deleteCategory = await _categoryService.CategoryDelete(categoryId);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List", "Category");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                var createCategory = await _categoryService.CategoryAdd(categoryCreateDTO);
                TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List", "Category");
        }
    }
}
