using AppBusiness.DTOs.CategoryDTOs;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DbContext _db;
        private readonly IDataRepository _dataRepository;

        public CategoryController(DbContext db, IDataRepository dataRepository)
        {
            _db = db;
            _dataRepository = dataRepository;
        }

        [HttpGet("getcategories")]
        public async Task<IActionResult> CategoryView()
        {
            var categories = await _db.Set<CategoryEntity>().Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                IconCssClass = c.IconCssClass,
                Color = c.Color
            }).ToListAsync();

            if (categories == null)
            {
                return Conflict("Categoryler getirilemedi.");
            }

            return Ok(categories);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> CategoryEdit([FromBody] CategoryEditDTO categoryEditDTO)
        {
            var category = await _db.Set<CategoryEntity>().SingleOrDefaultAsync(c => c.Id == categoryEditDTO.Id);

            if (category == null)
            {
                return Conflict("Böyle bir category yok.");
            }

            category.Name = categoryEditDTO.Name;
            category.IconCssClass = categoryEditDTO.IconCssClass;
            category.Color = categoryEditDTO.Color;

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> CategoryDelete([FromBody] int categoryId)
        {
            var category = await _db.Set<CategoryEntity>().SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {

                return Conflict("Böyle bir category yok.");
            }
            
            var hasProducts = await _db.Set<ProductEntity>().AnyAsync(p => p.CategoryId == categoryId);

            if (hasProducts)
            {
                return Conflict("Bu kategori silinemez çünkü bu kategoriye ait ürünler bulunmaktadır. Önce bu kategorideki ürünleri silin veya başka bir kategoriye taşıyın.");
            }

            bool sildimi = await _dataRepository.DeleteById<CategoryEntity>(categoryId);

            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CategoryCreate([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            CategoryEntity newCategory = new CategoryEntity
            {
                Name = categoryCreateDTO.Name,
                Color = categoryCreateDTO.Color,
                IconCssClass = categoryCreateDTO.IconCssClass,
                CreatedAt = DateTime.Now
            };

            bool ekledimi = await _dataRepository.Add<CategoryEntity>(newCategory);

            return Ok();
        }
    }
}



