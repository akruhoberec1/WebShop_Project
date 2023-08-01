using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IPaginateService
    {
        PagedList<Order> PaginateOrders(List<Order> ordersWithProducts, Paginate paginate);
    }
}
