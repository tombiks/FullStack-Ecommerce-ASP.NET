using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.CartDTOs
{
    public class CartShowDTO
    {
        public string? ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice {  get; set; }
        public byte Quantity { get; set; }
        public decimal TotalPrice { get; set; }


    }
}
