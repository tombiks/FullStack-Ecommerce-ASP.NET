using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.AuthDTOs
{
    public class AuthLoginDTO
    {
        [Required(ErrorMessage = "Lütfen emailinizi yazınız."), EmailAddress(ErrorMessage = "Lütfen geçerli e-mail adresi giriniz.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen şifrenizi yazınız."), MinLength(4), DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
