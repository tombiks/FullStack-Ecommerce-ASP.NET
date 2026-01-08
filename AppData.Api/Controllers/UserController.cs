using AppBusiness.DTOs.AuthDTOs;
using AppBusiness.DTOs.ProfileDTOs;
using AppBusiness.DTOs.UserDTOs;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDataRepository _repo;
        private readonly DbContext _db;

        public UserController(IDataRepository repo, DbContext db)
        {
            _repo = repo;
            _db = db;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser(AuthRegisterDTO authRegisterDTO)
        {
            int roleId;
            bool enabled;

            if (authRegisterDTO.Role == "seller")
            {
                var sellerRole = await _db.Set<RoleEntity>().FirstOrDefaultAsync(r => r.Name == "Seller");
                roleId = sellerRole!.Id;
                enabled = false;
            }

            else
            {
                var buyerRole = await _db.Set<RoleEntity>().FirstOrDefaultAsync(r => r.Name == "Buyer");
                roleId = buyerRole!.Id;
                enabled = true;
            }

            var newUser = new UserEntity
            {
                Email = authRegisterDTO.Email,
                FirstName = authRegisterDTO.FirstName,
                LastName = authRegisterDTO.LastName,
                Password = authRegisterDTO.Password,
                RoleId = roleId,
                Enabled = enabled,
                CreatedAt = DateTime.Now,
            };

            var eklendi = _repo.Add(newUser);

            return Ok();
        }

        [HttpPost("update/{userId}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int userId, UpdateUserDTO updateUserDTO)
        {
            var user = await _db.Set<UserEntity>().Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Conflict("User bulunamadı.");
            }

            user.FirstName = updateUserDTO.Name;
            user.LastName = updateUserDTO.LastName;
            user.Email = updateUserDTO.Email;

            await _db.SaveChangesAsync();

            UserCookieDTO userCookieDTO = new UserCookieDTO
            {
                Id = user.Id,
                Name = user.FirstName,
                Surname = user.LastName,
                Email = user.Email,
                Role = user.Role!.Name,
            };

            return Ok(userCookieDTO);
        }

        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await _db.Set<UserEntity>().Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Conflict("User bulunamadı.");
            }

            bool sellerMi = user.Role!.Id == 2;
            
            ProfileDetailDTO profileDetailDTO = new ProfileDetailDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsSeller = sellerMi,                
            };

            if (sellerMi)
            {
                var products = await _db.Set<ProductEntity>()
                    .Include(p => p.Category)
                    .Where(p => p.SellerId == userId)
                    .ToListAsync();

                profileDetailDTO.ProductIds = products.Select(p => p.Id).ToList();
                profileDetailDTO.Products = products.Select(p => p.Name).ToList();
                profileDetailDTO.Categories = products.Select(p => p.Category!.Name).ToList();
            }

            return Ok(profileDetailDTO);
        }

        [HttpGet("counts")]
        public async Task<IActionResult> Counts()
        {
            List<int> countList = new List<int>();

            var sellerUsers = await _db.Set<UserEntity>().Where(u => u.RoleId == 2).ToListAsync();

            int sellerUserSayisi = sellerUsers.Count;

            var buyerUsers = await _db.Set<UserEntity>().Where(u => u.RoleId == 3).ToListAsync();

            int buyerUserSayisi = buyerUsers.Count;

            var waitingUsers = await _db.Set<UserEntity>().Where(u => !u.Enabled).ToListAsync();

            int waitingUserSayisi = waitingUsers.Count;            

            countList.Add(sellerUserSayisi);
            countList.Add(buyerUserSayisi);
            countList.Add(waitingUserSayisi);

            if (countList == null)
            {
                return Conflict("User bulunamadı.");
            }

            return Ok(countList);
        }

        [HttpGet("userlist")]
        public async Task<IActionResult> Users() 
        {
            var users = await _db.Set<UserEntity>().Include(u => u.Role)
                .Where(u => u.RoleId != 1)
                .Where(u => u.Enabled)
                .Select(u => new UserListDTO
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Role = u.Role!.Name,
                    Email = u.Email,
                    CreatedTime = u.CreatedAt
                }).ToListAsync();

            if (users == null)
            {
                return Conflict("User bulunamadı.");
            }

            return Ok(users);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] int userId)
        {
            var user = await _db.Set<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Conflict("User bulunamadı.");
            }

            // Check if the user has any products (seller with products)
            var hasProducts = await _db.Set<ProductEntity>().AnyAsync(p => p.SellerId == userId);

            if (hasProducts)
            {
                return Conflict("Bu kullanıcı silinemez çünkü bu kullanıcıya ait ürünler bulunmaktadır. Önce bu kullanıcının ürünlerini silin.");
            }

            bool silme = await _repo.DeleteById<UserEntity>(userId);

            return Ok(silme);
        }

        [HttpGet("approvelist")]
        public async Task<IActionResult> ApproveUsers()
        {
            var users = await _db.Set<UserEntity>().Include(u => u.Role)
                .Where(u => u.RoleId != 1)
                .Where(u => !u.Enabled)
                .Select(u => new UserListDTO
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Role = u.Role!.Name,
                    Email = u.Email,
                    CreatedTime = u.CreatedAt
                }).ToListAsync();

            if (users == null)
            {
                return Conflict("User bulunamadı.");
            }

            return Ok(users);
        }

        [HttpPost("toenable")]
        public async Task<IActionResult> ToEnable([FromBody] int userId)
        {
            var user = await _db.Set<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Conflict("User bulunamadı.");
            }

            user.Enabled = true;            

            await _db.SaveChangesAsync();
            
            return Ok(true);
        }




    }
}
