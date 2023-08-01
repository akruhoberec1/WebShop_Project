using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Service.Common;
using Technoshop.Model;
using Technoshop.Repository.Common;
using System.Runtime.Remoting.Messaging;

namespace Technoshop.Service
{
    public class ProductService : IProductService
    {

        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedList<Product>> GetProductsAsync(ProductFilter filter, Sorting sorting, Paginate pagination)
        {
            PagedList<Product> products = await _productRepository.GetProductsAsync(filter, sorting, pagination);
            return products;
         }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            Product product = await _productRepository.GetProductByIdAsync(id);
            return product;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
               bool isCreated = await _productRepository.CreateProductAsync(product);
            return isCreated;
        }

        public async Task<bool>DeleteProductAsync(Guid id)
        {
            bool isDeleted = await _productRepository.DeleteProductAsync(id);
            return isDeleted;
         }

        public async Task<bool> UpdateProductAsync(Guid id, Product product)
        {
            bool isUpdated = await _productRepository.UpdateProductAsync(id, product);
            return isUpdated;
        }

        //public async Task<List<Product>> GetProductsByOrderIdAsync(Guid orderId)
        //{
        //    List<Product> products = await _productRepository.GetProductsByOrderIdAsync(orderId);
        //    return products == null ? null : products;
        //}

        public async Task<List<Product>> GetSimpleProductsAsync()
        {
            List<Product> products = await _productRepository.GetSimpleProductsAsync();
            return products == null ? null : products;
        }
    }
   
}
