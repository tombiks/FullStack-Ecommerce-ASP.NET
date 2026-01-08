using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.ProductDTOs
{
    public class ProductAddDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Details { get; set; } = string.Empty;
        public byte StockAmount { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
