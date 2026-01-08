using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.DTOs.CategoryDTOs
{
    public class CategoryCreateDTO
    {
        public string Name { get; set; } = null!;
        public string IconCssClass { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
