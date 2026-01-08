using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.CommentDTOs
{
    public class CommentShowDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
