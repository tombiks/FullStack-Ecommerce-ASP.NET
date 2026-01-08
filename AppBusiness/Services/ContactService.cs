using AppBusiness.DTOs.CategoryDTOs;
using AppBusiness.DTOs.ContactDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class ContactService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient client => _clientFactory.CreateClient("data-api");

        public ContactService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> NewspaperAdd(NewspaperDTO newsPaperDto)
        {
            var response = await client.PostAsJsonAsync($"api/contact/addnewspaper", newsPaperDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ContactUsAdd(NewContactDTO contactDto)
        {
            var response = await client.PostAsJsonAsync($"api/contact/addcontactus", contactDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<NewContactDTO>?> ContactsViewService()
        {
            var response = await client.GetAsync("api/contact/allcontacts");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var contacts = await response.Content.ReadFromJsonAsync<List<NewContactDTO>>();

            if (contacts == null)
            {
                throw new Exception("Contacts deserialization returned null");
            }

            return contacts;
        }

        public async Task<List<NewspaperDTO>?> NewspapersViewService()
        {
            var response = await client.GetAsync("api/contact/allnewspapers");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var newspapers = await response.Content.ReadFromJsonAsync<List<NewspaperDTO>>();

            if (newspapers == null)
            {
                throw new Exception("Contacts deserialization returned null");
            }

            return newspapers;
        }
    }
}
