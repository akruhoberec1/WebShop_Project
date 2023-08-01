using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Repository.Common;
using Technoshop.Service.Common;
using Technoshop.Model;
using System.ComponentModel;
using Technoshop.Common;
using SendGrid.Helpers.Mail.Model;
using SendGrid.Helpers.Mail;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Xml.Linq;
using System.IO;
using Npgsql;

namespace Technoshop.Service

{

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMailerService _mailerService; 
        private readonly IProductRepository _productRepository;
        private readonly IPaginateService _paginateService;
        private readonly IConnectionService _connectionService;
        private readonly IPdfService _pdfService;    
        public OrderService(IOrderRepository orderRepository, IMailerService mailerService, IProductRepository productRepository, IPaginateService paginateService, IConnectionService connectionService, IPdfService pdfService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mailerService = mailerService;
            _paginateService = paginateService;
            _connectionService = connectionService;
            _pdfService = pdfService;
        }

        public async Task<PagedList<Order>> GetOrdersAsync(Paginate paginate, OrderFilter filtering, OrderSorting sorting)
        {

            PagedList<Order> orders = await _orderRepository.GetOrdersWithDataAsync(paginate, filtering, sorting);
            if (orders != null)
            {
                PagedList<Order> ordersWithProducts = await _productRepository.GetProductsByOrderIdAsync(orders);
                return ordersWithProducts;
            }
            return null;

        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {   
            Order order = await _orderRepository.GetOrderById(id);
                if (order != null)
                {
                    return order;
                }
                
            return null;
        }

        public async Task<bool> UpdateOrderAsync(Guid id, Order order)
        {
            bool isUpdated = await _orderRepository.UpdateOrderAsync(id, order);

            return (isUpdated == true) ? true : false;   
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            bool isDeleted = await _orderRepository.DeleteOrderAsync(id);

            return (isDeleted == true) ? true : false;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            bool isCreated = await _orderRepository.CreateOrderAsync(order);

            if (isCreated)
            {
                await _mailerService.SendMail(order);

                Order newOrder = await _orderRepository.GetOrderById(order.Id);
                _pdfService.Create(newOrder);

                return newOrder;
            }
            return null;
        }





    }
}
