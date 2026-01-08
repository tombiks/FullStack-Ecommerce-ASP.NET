using AppBusiness.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserService _userService;

        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public AuthService(IHttpClientFactory clientFactory, UserService userService)
        {
            _clientFactory = clientFactory;
            _userService = userService;

        }

        public async Task<UserCookieDTO?> LoginService(AuthLoginDTO AuthLoginDTO) 
        {
            var response = await Client.PostAsJsonAsync("/api/auth/login", AuthLoginDTO);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var userCookie = await response.Content.ReadFromJsonAsync<UserCookieDTO>();

            return userCookie;
        }

        public async Task<bool> RegisterService(AuthRegisterDTO AuthRegisterDTO)
        {
            var response = await Client.PostAsJsonAsync("/api/auth/register", AuthRegisterDTO);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}
