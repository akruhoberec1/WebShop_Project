using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetProductsAsync(ProductFilter filter, Sorting sorting, Paginate pagination);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<bool> CreateProductAsync(Product product);
        Task<bool>DeleteProductAsync(Guid id);
        Task<bool> UpdateProductAsync(Guid id, Product product);    
        //Task<List<Product>> GetProductsByOrderIdAsync(Guid orderId);
        Task<List<Product>> GetSimpleProductsAsync();
    }
}
