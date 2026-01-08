using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.CommentDTOs
{
    public class CommentAddDTO
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Comment text is required")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Text { get; set; } = null!;

        [Required(ErrorMessage = "Star rating is required")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5")]
        public byte StarCount { get; set; }
    }
}
