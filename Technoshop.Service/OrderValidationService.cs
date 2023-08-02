using PdfSharp.Pdf.Filters;
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
    public class OrderValidationService : IOrderValidationService
    {
        public bool CreateOrderValidation(Order order)
        {

            if (order.ShippingAddress.Id == Guid.Empty)
                return false;

            if (order.BillingAddress.Id == Guid.Empty)
                return false;

            if (order.TotalPrice <= 0)
                return false;

            if (order.Products == null || !order.Products.Any())
                return false;

            return true;
        }

        public (OrderFilter, OrderSorting) OrderParamsValidation(OrderFilter filter, OrderSorting sorting)
        {
            string[] allowedSort = { "CreatedAt", "TotalPrice", "UpdatedAt"};
            string sortByQuery = sorting.SortBy;
            if (!string.IsNullOrEmpty(sortByQuery))
            {
                if (allowedSort.All(sortByQuery.Contains))
                {
                    sorting.SortBy = allowedSort[0];    
                }
            }

            return (filter, sorting);

            //check searchQuery SQL injection
        }




    }
}
