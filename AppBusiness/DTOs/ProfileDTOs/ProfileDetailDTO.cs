using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.ProfileDTOs
{
    public class ProfileDetailDTO
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool IsSeller { get; set; }
        public List<int>? ProductIds { get; set; }
        public List<string>? Products { get; set; }
        public List<string>? Categories { get; set; }

    }
}
