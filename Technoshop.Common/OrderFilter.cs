using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;

namespace Technoshop
{
    public class OrderFilter : Filter
    {
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
