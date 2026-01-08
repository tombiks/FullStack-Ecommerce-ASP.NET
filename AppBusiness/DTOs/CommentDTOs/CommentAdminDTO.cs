using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.CommentDTOs
{
    public class CommentAdminDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
