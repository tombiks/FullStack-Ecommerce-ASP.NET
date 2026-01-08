using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.OrderDTOs
{
    public class OrderItemShowDTO
    {
        public string? ProductName { get; set; }
        public byte Quantity { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
