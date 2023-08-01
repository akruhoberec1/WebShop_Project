using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IOrderService
    {
        Task<PagedList<Order>> GetOrdersAsync(Paginate paginate, OrderFilter filtering, OrderSorting sorting);
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<bool> DeleteOrderAsync(Guid id);   
        Task<bool> UpdateOrderAsync(Guid id, Order order);
        Task<Order> CreateOrderAsync(Order order);

    }
}
