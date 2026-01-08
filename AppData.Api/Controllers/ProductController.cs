using AppBusiness.DTOs.CategoryDTOs;
using AppBusiness.DTOs.ProductDTOs;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbContext _db;
        private readonly IDataRepository _dataRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DbContext db, IDataRepository dataRepository, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _dataRepository = dataRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("featured")]
        public async Task<IActionResult> ProductForFeatured()
        {
            // İlk önce tüm aktif ürünleri ve resimlerini yükle
            var allProducts = await _db.Set<ProductEntity>()
                .Where(p => p.Enabled)
                .Include(p => p.Images)
                .ToListAsync();

            // Sonra bellekte gruplama ve seçim yap
            var products = allProducts
                .GroupBy(p => p.CategoryId)
                .SelectMany(g => g.OrderByDescending(p => p.Id).Take(3))
                .Select(p => new ProductFeaturedDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.Images?.Select(i => i.Url).FirstOrDefault() ?? "default.jpg"
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("categoried/{categoryId}")]
        public async Task<IActionResult> ProductForCategoried([FromRoute] int categoryId)
        {
            var products = await _db.Set<ProductEntity>().Where(p => p.Enabled)
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Images)
                .Select(p => new ProductCategoriedDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.Images!.Select(i => i.Url).FirstOrDefault() ?? "default.jpg"
                }).ToListAsync();

            if (products == null)
            {
                return Conflict("Productlar getirilemedi.");
            }

            return Ok(products);
        }

        [HttpGet("productdetails/{productId}")]
        public async Task<IActionResult> ProductDetails([FromRoute] int productId)
        {
            var productDetails = await _db.Set<ProductEntity>()
                .Include(p => p.Images)
                .Include(p => p.Comments!)
                .Where(p => p.Id == productId)
                .Select(p => new ProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Details = p.Details,
                    MainImgUrl = p.Images != null && p.Images.Any() ? p.Images.Select(i => i.Url).FirstOrDefault() : null,
                    ImagesUrls = p.Images != null ? p.Images.Select(i => i.Url).ToList() : new List<string>(),
                    CommentUserName = p.Comments != null ? p.Comments.Select(u => u.User!.FirstName).ToList() : new List<string>(),
                    CommentText = p.Comments != null ? p.Comments.Select(t => t.Text).ToList() : new List<string>(),
                    StartCount = p.Comments != null ? p.Comments.Select(s => s.StarCount).ToList() : new List<byte>()
                }).SingleOrDefaultAsync();

            if (productDetails == null)
            {
                return Conflict("Productlar getirilemedi.");
            }

            return Ok(productDetails);
        }

        [HttpGet("getproducts")]
        public async Task<IActionResult> ProductsForAdmin()
        {
            var products = await _db.Set<ProductEntity>().Select(p => new ProductListDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductDescription = p.Details,
                SellerId = p.SellerId,
                SellerName = p.Seller!.FirstName! + " " + p.Seller!.LastName!,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name!
            }).ToListAsync();

            if (products == null)
            {
                return Conflict("Categoryler getirilemedi.");
            }

            return Ok(products);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> ProductDelete([FromBody] int productId)
        {
            var product = await _db.Set<ProductEntity>().SingleOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return Conflict("Böyle bir product yok.");
            }

            bool sildimi = await _dataRepository.DeleteById<ProductEntity>(productId);

            return Ok();
        }

        [HttpGet("foredit/{productId}")]
        public async Task<IActionResult> GetProductForEdit([FromRoute] int productId)
        {
            // Önce entity'yi çek
            var productEntity = await _db.Set<ProductEntity>()
                .Include(p => p.Images)
                .Where(p => p.Id == productId)
                .FirstOrDefaultAsync();

            if (productEntity == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            // Sonra memory'de projection yap
            var product = new
            {
                productEntity.Id,
                productEntity.Name,
                productEntity.Price,
                productEntity.Details,
                productEntity.StockAmount,
                productEntity.CategoryId,
                Images = (productEntity.Images ?? new List<ProductImageEntity>()).Select(i => new { i.Id, i.Url }).ToList()
            };

            return Ok(product);
        }

        [HttpPost("add")]
        public async Task<IActionResult> ProductAdd([FromForm] ProductAddDTO productAddDTO, [FromForm] int sellerId)
        {
            // 1. Product entity oluştur
            var newProduct = new ProductEntity
            {
                SellerId = sellerId,
                CategoryId = productAddDTO.CategoryId,
                Name = productAddDTO.Name,
                Price = productAddDTO.Price,
                Details = productAddDTO.Details,
                StockAmount = productAddDTO.StockAmount,
                Enabled = true,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Product'ı veritabanına ekle (Id otomatik oluşur)
            bool added = await _dataRepository.Add(newProduct);
            if (!added)
            {
                return Conflict("Ürün eklenemedi.");
            }

            // 3. Fotoğrafları kaydet
            if (productAddDTO.Images != null && productAddDTO.Images.Any())
            {
                // uploads/products klasörünün yolunu oluştur
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var imageFile in productAddDTO.Images)
                {
                    // Dosya adı oluştur: {ProductId}_{Guid}.{uzantı}
                    var fileName = $"{newProduct.Id}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Dosyayı fiziksel olarak kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                        await stream.FlushAsync();
                        stream.Close();
                    }

                    // Veritabanına ProductImage kaydı ekle
                    var productImage = new ProductImageEntity
                    {
                        ProductId = newProduct.Id,
                        Url = $"/uploads/products/{fileName}",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dataRepository.Add(productImage);
                }
            }

            return Ok();
        }

        [HttpPost("edit")]
        public async Task<IActionResult> ProductEdit([FromForm] ProductEditDTO productEditDTO)
        {
            
            var product = await _db.Set<ProductEntity>().FindAsync(productEditDTO.Id);
            if (product == null)
            {
                return Conflict("Ürün bulunamadı.");
            }

            product.Name = productEditDTO.Name;
            product.Price = productEditDTO.Price;
            product.Details = productEditDTO.Details;
            product.StockAmount = productEditDTO.StockAmount;
            product.CategoryId = productEditDTO.CategoryId;

            await _db.SaveChangesAsync();

            // 2. Eski fotoğrafları sil
            if (productEditDTO.DeletedImageIds != null && productEditDTO.DeletedImageIds.Any())
            {
                foreach (var imageId in productEditDTO.DeletedImageIds)
                {
                    var productImage = await _db.Set<ProductImageEntity>().FindAsync(imageId);
                    if (productImage != null)
                    {
                        // Fiziksel dosyayı sil
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, productImage.Url.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        // Veritabanından sil
                        await _dataRepository.DeleteById<ProductImageEntity>(imageId);
                    }
                }
            }

            // 3. Yeni fotoğrafları ekle
            if (productEditDTO.NewImages != null && productEditDTO.NewImages.Any())
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var imageFile in productEditDTO.NewImages)
                {
                    var fileName = $"{product.Id}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                        await stream.FlushAsync();
                        stream.Close();
                    }

                    var productImage = new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Url = $"/uploads/products/{fileName}",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dataRepository.Add(productImage);
                }
            }

            return Ok();
        }

    }
}
