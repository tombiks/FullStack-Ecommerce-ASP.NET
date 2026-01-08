using AppBusiness.DTOs.ContactDTOs;
using AppData.Context;
using AppData.Entities;
using AppData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IDataRepository _dataRepo;
        private readonly AppDbContext _db;

        public ContactController(IDataRepository dataRepo, AppDbContext db)
        {
            _dataRepo = dataRepo;
            _db = db;
        }

        [HttpPost("addnewspaper")]
        public async Task<IActionResult> AddNewspaper(NewspaperDTO newspaperDto) 
        {
            var newNewspaperEntity = new NewspaperEntity 
            {
                Email = newspaperDto.Email,                
            };

            bool ekle = await _dataRepo.Add<NewspaperEntity>(newNewspaperEntity);

            if (!ekle)
            {
                return Conflict("Mail kaydedilemedi");
            }

            return Ok();
        }

        [HttpPost("addcontactus")]
        public async Task<IActionResult> AddContactUs(NewContactDTO newContactDto)
        {
            var ContactEntity = new ContactEntity
            {
                Email = newContactDto.Email,
                Message = newContactDto.Message,
                Name = newContactDto.Name,
            };

            bool ekle = await _dataRepo.Add<ContactEntity>(ContactEntity);

            if (!ekle) 
            {
                return Conflict("Mesaj kaydedilemedi");
            }

            return Ok();
        }

        [HttpGet("allcontacts")]
        public async Task<IActionResult> GetAllContact()
        {
            var contacts = await _db.Set<ContactEntity>().Select(c => new NewContactDTO 
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Message = c.Message
            }).ToListAsync();

            return Ok(contacts);
        }

        [HttpGet("allnewspapers")]
        public async Task<IActionResult> GetAllNewspaper()
        {
            var newspapers = await _db.Set<NewspaperEntity>().Select(n => new NewspaperDTO
            {  
                Id = n.Id,
                Email = n.Email,                
            }).ToListAsync();

            return Ok(newspapers);
        }
    }
}
