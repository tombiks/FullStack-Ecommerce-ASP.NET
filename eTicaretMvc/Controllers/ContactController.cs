using AppBusiness.DTOs.ContactDTOs;
using AppBusiness.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretMvc.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }
        [HttpPost]
        public async Task<IActionResult> ContactUs([FromForm] NewContactDTO newContact)
        {
            try
            {
                var ekle = await _contactService.ContactUsAdd(newContact);

                if (!ekle)
                {
                    return Conflict("Contact mesajı gönderme başarısız");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewsPaper([FromForm] NewspaperDTO newspaperDto)
        {
            try
            {
                var ekle = await _contactService.NewspaperAdd(newspaperDto);

                if (!ekle)
                {
                    return Conflict("Newspaper isteği  gönderme başarısız");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
