using AppBusiness.DTOs.AuthDTOs;
using AppBusiness.DTOs.ProfileDTOs;
using AppBusiness.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class UserService
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public UserService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> AddUserService(AuthRegisterDTO authRegisterDTO)
        {
            var response = await Client.PostAsJsonAsync("api/user/add", authRegisterDTO);

            return response.IsSuccessStatusCode;
        }

        public async Task<UserCookieDTO> UpdateUserService(int userId, UpdateUserDTO updateUserDTO)
        {
            var response = await Client.PostAsJsonAsync($"api/user/update/{userId}", updateUserDTO);

            var userCookie = await response.Content.ReadFromJsonAsync<UserCookieDTO>();

            return userCookie!;
        }

        public async Task<ProfileDetailDTO> ShowUserService(int userId)
        {

            var response = await Client.GetAsync($"api/user/get/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var userInfo = await response.Content.ReadFromJsonAsync<ProfileDetailDTO>();

            if (userInfo == null)
            {
                throw new Exception("User info deserialization returned null");
            }

            return userInfo;

        }

        public async Task<List<int>> UserCounts()
        {
            var response = await Client.GetAsync($"api/user/counts");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var userCounts = await response.Content.ReadFromJsonAsync<List<int>>();

            if (userCounts == null)
            {
                throw new Exception("User info deserialization returned null");
            }

            return userCounts;

        }

        public async Task<List<UserListDTO>> UserList()
        {
            var response = await Client.GetAsync($"api/user/userlist");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var userList = await response.Content.ReadFromJsonAsync<List<UserListDTO>>();

            if (userList == null)
            {
                throw new Exception("User info deserialization returned null");
            }

            return userList;

        }

        public async Task<bool> DeleteUserService(int userId)
        {
            var response = await Client.PostAsJsonAsync($"api/user/delete", userId);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var sildiMi = await response.Content.ReadFromJsonAsync<bool>();

            if (sildiMi == false)
            {
                throw new Exception("User could not be deleted");
            }

            return sildiMi;
        }

        public async Task<List<UserListDTO>> ApproveList()
        {
            var response = await Client.GetAsync($"api/user/approvelist");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var userList = await response.Content.ReadFromJsonAsync<List<UserListDTO>>();

            if (userList == null)
            {
                throw new Exception("User info deserialization returned null");
            }

            return userList;

        }

        public async Task<bool> ToEnableUserService(int userId)
        {
            var response = await Client.PostAsJsonAsync($"api/user/toenable", userId);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var toEnable = await response.Content.ReadFromJsonAsync<bool>();

            if (toEnable == false)
            {
                throw new Exception("User could not be enabled");
            }

            return toEnable;
        }
    }
}
