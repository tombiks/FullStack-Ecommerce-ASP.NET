using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.AuthDTOs
{
    public class AuthRegisterDTO
    {
        [Required(ErrorMessage = "Mail adresi girmek zorunludur."), EmailAddress(ErrorMessage = "Geçerli mail adresi giriniz.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "İsim girmek zorunludur."), MaxLength(50, ErrorMessage = "İsim 50 karakteri geçmemeli.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Soyad girmek zorunludur."), MaxLength(50, ErrorMessage = "Soyad 50 karakteri geçmemeli.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Şifre oluşturmak zorunludur."), DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Şifre tekrarı girmek zorunludur."), DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmedPassword { get; set; } = null!;

        [Required(ErrorMessage = "Role seçimi zorunludur.")]
        public string Role { get; set; } = null!;
    }
}
