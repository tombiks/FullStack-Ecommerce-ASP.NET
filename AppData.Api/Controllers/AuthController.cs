using AppBusiness.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppData.Repositories;
using AppData.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbContext _db;
        private readonly IDataRepository _dataRepository;

        public AuthController(DbContext db, IDataRepository dataRepository)
        {
            _db = db;
            _dataRepository = dataRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginDTO authLoginDTO)
        {
            var user = await _db.Set<UserEntity>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == authLoginDTO.Email && u.Password == authLoginDTO.Password && u.Enabled);

            if (user == null)
            {
                return Conflict("Giriş işlemi yapılamadı");
            }

            UserCookieDTO userCookie = new UserCookieDTO { Id = user.Id, Name = user.FirstName, Surname = user.LastName, Email = user.Email, Role = user.Role!.Name };

            return Ok(userCookie);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthRegisterDTO authRegisterDTO)
        {

            var user = await _db.Set<UserEntity>().FirstOrDefaultAsync(u => u.Email == authRegisterDTO.Email);

            if (user != null)
            {
                return Conflict("Kayıt işlemi başarısız");
            }

            if (authRegisterDTO.Role == "buyer")
            {
                var newUser = new UserEntity
                {
                    Email = authRegisterDTO.Email,
                    FirstName = authRegisterDTO.FirstName,
                    LastName = authRegisterDTO.LastName,
                    Password = authRegisterDTO.Password,
                    RoleId = 3,
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow
                };

                bool ekledimi = await _dataRepository.Add<UserEntity>(newUser);

                return Ok();
            }

            else
            {
                var newUser = new UserEntity
                {
                    Email = authRegisterDTO.Email,
                    FirstName = authRegisterDTO.FirstName,
                    LastName = authRegisterDTO.LastName,
                    Password = authRegisterDTO.Password,
                    RoleId = 2,
                    Enabled = false,
                    CreatedAt = DateTime.UtcNow
                };

                bool ekledimi = await _dataRepository.Add<UserEntity>(newUser);

                return Ok();
            }          


        }
    }
}
