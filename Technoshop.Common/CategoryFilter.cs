using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Common
{
  public  class CategoryFilter : Filter
    {
        public bool? IsActive { get; set; } = true;
    }
}
