using AppBusiness.DTOs.CartDTOs;
using AppBusiness.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class OrderService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public OrderService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> CompleteOrder(List<CartShowDTO> cartList, int userId)
        {
            var response = await Client.PostAsJsonAsync($"api/Order/createorder?userId={userId}", cartList);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderShowDTO>> GetUserOrders(int userId)
        {
            var response = await Client.GetAsync($"api/Order?userId={userId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var orders = await response.Content.ReadFromJsonAsync<List<OrderShowDTO>>();

            return orders ?? new List<OrderShowDTO>();
        }
    }
}
