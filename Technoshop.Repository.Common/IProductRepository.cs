using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model.Common;
using Technoshop.Model;
using Npgsql;

namespace Technoshop.Repository.Common
{
    public interface IProductRepository
    {
        Task<PagedList<Product>> GetProductsAsync(ProductFilter filter, Sorting sorting, Paginate pagination);

        Task<Product> GetProductByIdAsync(Guid id);

        Task<bool> CreateProductAsync(Product product);

        Task<bool> UpdateProductAsync(Guid id, Product product);

        Task<bool> DeleteProductAsync(Guid id);
        Task<Dictionary<Guid, List<Product>>> GetProductsByOrderIdAsync(List<Guid> orderIds);
        Task<List<Product>> GetSimpleProductsAsync();
    }
}