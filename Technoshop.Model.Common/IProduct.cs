using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model.Common
{
    public interface IProduct
    {
        Guid Id { get; set; }
        string Name { get; set; }
        decimal? Price { get; set; }
        int? Quantity { get; set; }
        Guid? CategoryId { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool? IsActive { get; set; }
        Guid? CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
    }
}
