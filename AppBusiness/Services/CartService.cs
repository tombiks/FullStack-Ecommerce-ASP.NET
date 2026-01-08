using AppBusiness.DTOs.CartDTOs;
using AppBusiness.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{    
    public class CartService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public CartService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool>AddCartItem(CartAddDTO cartAddDto) 
        {
            var response = await Client.PostAsJsonAsync($"api/cart/addcart", cartAddDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCartItem(CartAddDTO cartAddDto)
        {
            var response = await Client.PutAsJsonAsync($"api/cart/updatecart", cartAddDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCartItem(int userId, int productId)
        {
            var response = await Client.DeleteAsync($"api/cart/deletecart/{userId}/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ClearCart(int userId)
        {
            var response = await Client.DeleteAsync($"api/cart/clearcart/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<CartShowDTO>> CartList(int userId)
        {
            var response = await Client.GetAsync($"api/cart/cartlist/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var cartList = await response.Content.ReadFromJsonAsync<List<CartShowDTO>>();

            return cartList ?? new List<CartShowDTO>();
        }
    }
}

