using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service.Common;

namespace Technoshop.Service
{
    public class PaginateService : IPaginateService
    {

        public PagedList<Order> PaginateOrders(List<Order> ordersWithProducts, Paginate paginate)
        {
            if(ordersWithProducts.Count > 0)
            {
                PagedList<Order> pagedOrders = new PagedList<Order>()
                {
                    Results = ordersWithProducts,
                    CurrentPage = paginate.PageNumber,
                    PageSize = paginate.PageSize,
                    TotalCount = paginate.TotalCount
                };

                return pagedOrders;
            }

            return null;
            
        }

    }
}
