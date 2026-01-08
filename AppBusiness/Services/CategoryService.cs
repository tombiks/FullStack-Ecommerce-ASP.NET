using AppBusiness.DTOs.CategoryDTOs;
using System.Net.Http.Json;

namespace AppBusiness.Services
{
    public class CategoryService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public CategoryService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<CategoryDTO>?> CategoryViewService()
        {
            var response = await Client.GetAsync("api/category/getcategories");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();

            if (categories == null)
            {
                throw new Exception("Categories deserialization returned null");
            }

            return categories;
        }

        public async Task<bool> CategoryEdit(CategoryEditDTO categoryEditDTO)
        {
            var response = await Client.PostAsJsonAsync($"api/category/edit", categoryEditDTO);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CategoryDelete(int categoryId)
        {
            var response = await Client.PostAsJsonAsync($"api/category/delete", categoryId);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CategoryAdd(CategoryCreateDTO categoryCreateDTO)
        {
            var response = await Client.PostAsJsonAsync($"api/category/create", categoryCreateDTO);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
