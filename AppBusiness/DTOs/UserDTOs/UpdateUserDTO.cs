using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required(ErrorMessage = "İsim girmek zorunludur."), MaxLength(50, ErrorMessage = "İsim 50 karakteri geçmemeli.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Soyad girmek zorunludur."), MaxLength(50, ErrorMessage = "Soyad 50 karakteri geçmemeli.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen emailinizi yazınız."), EmailAddress(ErrorMessage = "Lütfen geçerli e-mail adresi giriniz.")]
        public string Email { get; set; } = null!;
    }
}
