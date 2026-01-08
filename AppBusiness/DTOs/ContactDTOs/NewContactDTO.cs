using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.ContactDTOs
{
    public class NewContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;

    }
}
