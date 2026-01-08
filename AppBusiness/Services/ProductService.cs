using AppBusiness.DTOs.CategoryDTOs;
using AppBusiness.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class ProductService
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient client => _clientFactory.CreateClient("data-api");

        public ProductService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<ProductFeaturedDTO>?> ProductForFeaturedService()
        {
            var response = await client.GetAsync("api/product/featured");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var productFeaturedList = await response.Content.ReadFromJsonAsync<List<ProductFeaturedDTO>>();

            if (productFeaturedList == null)
            {
                throw new Exception("Product deserialization returned null");
            }

            return productFeaturedList;

        }

        public async Task<List<ProductCategoriedDTO>?> ProductForCategoriedService(int categoryId)
        {

            var response = await client.GetAsync($"api/product/categoried/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var productCategoriedList = await response.Content.ReadFromJsonAsync<List<ProductCategoriedDTO>>();

            if (productCategoriedList == null)
            {
                throw new Exception("Product categoried deserialization returned null");
            }

            return productCategoriedList;
        }

        public async Task<ProductDetailDTO?> ProductDetailsService(int productId)
        {
            var response = await client.GetAsync($"api/product/productdetails/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var productDetails = await response.Content.ReadFromJsonAsync<ProductDetailDTO>();

            return productDetails;

        }

        public async Task<List<ProductListDTO>?> ProductViewService()
        {
            var response = await client.GetAsync("api/product/getproducts");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var products = await response.Content.ReadFromJsonAsync<List<ProductListDTO>>();

            if (products == null)
            {
                throw new Exception("Products deserialization returned null");
            }

            return products;
        }

        public async Task<bool> ProductDelete(int productId)
        {
            var response = await client.PostAsJsonAsync($"api/product/delete", productId);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ProductAdd(ProductAddDTO productAddDTO, int sellerId)
        {
            // MultipartFormDataContent oluştur (fotoğraf yüklemek için gerekli)
            using var formData = new MultipartFormDataContent();

            // Metin verilerini ekle
            formData.Add(new StringContent(productAddDTO.Name), "Name");
            formData.Add(new StringContent(productAddDTO.Price.ToString()), "Price");
            formData.Add(new StringContent(productAddDTO.Details), "Details");
            formData.Add(new StringContent(productAddDTO.StockAmount.ToString()), "StockAmount");
            formData.Add(new StringContent(productAddDTO.CategoryId.ToString()), "CategoryId");
            formData.Add(new StringContent(sellerId.ToString()), "sellerId");

            // Fotoğrafları ekle
            if (productAddDTO.Images != null && productAddDTO.Images.Any())
            {
                foreach (var image in productAddDTO.Images)
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                    formData.Add(streamContent, "Images", image.FileName);
                }
            }

            // API'ye gönder
            var response = await client.PostAsync("api/product/add", formData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<dynamic?> GetProductForEdit(int productId)
        {
            var response = await client.GetAsync($"api/product/foredit/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var productData = await response.Content.ReadFromJsonAsync<dynamic>();

            return productData;
        }

        public async Task<bool> ProductEdit(ProductEditDTO productEditDTO)
        {
            using var formData = new MultipartFormDataContent();

            // Metin verilerini ekle
            formData.Add(new StringContent(productEditDTO.Id.ToString()), "Id");
            formData.Add(new StringContent(productEditDTO.Name), "Name");
            formData.Add(new StringContent(productEditDTO.Price.ToString()), "Price");
            formData.Add(new StringContent(productEditDTO.Details), "Details");
            formData.Add(new StringContent(productEditDTO.StockAmount.ToString()), "StockAmount");
            formData.Add(new StringContent(productEditDTO.CategoryId.ToString()), "CategoryId");

            // Silinecek fotoğrafların ID'lerini ekle
            if (productEditDTO.DeletedImageIds != null && productEditDTO.DeletedImageIds.Any())
            {
                foreach (var imageId in productEditDTO.DeletedImageIds)
                {
                    formData.Add(new StringContent(imageId.ToString()), "DeletedImageIds");
                }
            }

            // Yeni fotoğrafları ekle
            if (productEditDTO.NewImages != null && productEditDTO.NewImages.Any())
            {
                foreach (var image in productEditDTO.NewImages)
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                    formData.Add(streamContent, "NewImages", image.FileName);
                }
            }

            var response = await client.PostAsync("api/product/edit", formData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
