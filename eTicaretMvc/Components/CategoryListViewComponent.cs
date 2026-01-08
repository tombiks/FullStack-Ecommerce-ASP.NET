using AppBusiness.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretMvc.Components
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly CategoryService _categoryService;

        public CategoryListViewComponent(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            var categoryNames = await _categoryService.CategoryViewService();

            return View(viewName, categoryNames);
        }
    }
}
