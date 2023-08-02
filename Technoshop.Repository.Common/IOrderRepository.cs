using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Repository.Common
{
    public interface IOrderRepository
    {
        Task<PagedList<Order>> GetOrdersWithDataAsync(Paginate paginate, OrderFilter filtering, OrderSorting sorting);
        Task<Order> GetOrderById(Guid id);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<bool> UpdateOrderAsync(Guid id, Order order);
        Task<Order> CreateOrderAsync(Order order);   
    }
}
