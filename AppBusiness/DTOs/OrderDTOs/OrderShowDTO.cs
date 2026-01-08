using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.OrderDTOs
{
    public class OrderShowDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public ICollection<OrderItemShowDTO>? Items { get; set; }

    }
}
