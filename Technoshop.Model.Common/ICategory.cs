using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model.Common
{
    public interface ICategory
    {
         Guid? Id { get; set; }
         string Title { get; set; }
         DateTime? CreatedAt { get; set; }
         DateTime? UpdatedAt { get; set; }
         bool? IsActive { get; set; }
         Guid? CreatedBy { get; set; }
         Guid? UpdatedBy { get; set; }
    }
}
