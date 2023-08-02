using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IOrderValidationService
    {
        bool CreateAndUpdateOrderValidation(Order order);
        (OrderFilter, OrderSorting) OrderParamsValidation(OrderFilter filter, OrderSorting sorting);
    }
}
