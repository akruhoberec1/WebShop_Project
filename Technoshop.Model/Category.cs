using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model.Common;

namespace Technoshop.Model
{
   public class Category : ICategory

    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<Product> Product { get; set; }
        public Person PersonCreated { get; set; }
        public Person PersonUpdated { get; set; }

    }
}
