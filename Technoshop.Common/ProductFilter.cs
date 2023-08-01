using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Common
{
    public class ProductFilter : Filter
    {
       
        public string Title { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }

        public int? Quantity { get; set; }
        
    }
}
