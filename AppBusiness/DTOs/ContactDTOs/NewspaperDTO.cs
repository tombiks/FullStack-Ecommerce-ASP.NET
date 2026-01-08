using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.ContactDTOs
{
    public class NewspaperDTO
    {
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
    }
}
