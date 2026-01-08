using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.ProductDTOs
{
    public class ProductDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public string? MainImgUrl { get; set; }
        public List<string>? ImagesUrls { get; set; }
        public List<string>? CommentUserName { get; set; }
        public List<string>? CommentText { get; set; }
        public List<byte>? StartCount { get; set; }
    }
}
