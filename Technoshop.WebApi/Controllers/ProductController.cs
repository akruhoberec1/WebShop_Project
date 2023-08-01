using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service.Common;
using Technoshop.WebApi.Models;
using Technoshop.WebApi.Models.Product;

namespace Technoshop.WebApi.Controllers
{
    public class ProductController : ApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("api/products/")]
        public async Task<HttpResponseMessage> GetProductsAsync(int pageNumber = 1, int pageSize = 10, string orderBy = "", string sortBy = "",
                                                           string searchQuery = "", string title = "", decimal? minPrice = null, decimal? maxPrice = null, int? quantity = null)
        {
            ProductFilter filter = new ProductFilter();
            filter.SearchQuery = searchQuery;
            filter.Title = title;
            filter.MinPrice = minPrice;
            filter.MaxPrice = maxPrice;
            filter.Quantity = quantity;

            Sorting sorting = new Sorting
            {
                SortBy = string.IsNullOrEmpty(sortBy) ? "Name" : sortBy,
                OrderBy = string.IsNullOrEmpty(orderBy) ? "ASC" : orderBy
            };

            Paginate pagination = new Paginate();
            pagination.PageSize = pageSize;
            pagination.PageNumber = pageNumber;

            PagedList<Product> pagedProducts = await _productService.GetProductsAsync(filter, sorting, pagination);

            if (pagedProducts != null && pagedProducts.Results.Count > 0)
            { var mapedList = MapProductListToRest(pagedProducts);
                return Request.CreateResponse(HttpStatusCode.OK,mapedList ); }
            else
                return Request.CreateResponse(HttpStatusCode.NoContent, "No products found.");
            
        }


        [HttpGet]
        [Route("api/product/{id}")]
        public async Task<HttpResponseMessage> GetProductByIdAsync(Guid id)
        {
            Product product = await _productService.GetProductByIdAsync(id);

            if (product != null)
                return Request.CreateResponse(HttpStatusCode.OK, product);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Product with Id {id} not found.");
        }

        [HttpPut]
        [Route("api/product/update/{id}")]
        public async Task<HttpResponseMessage> UpdateProductAsync(Guid id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid product data.");

            updatedProduct.Id = id;
            bool result = await _productService.UpdateProductAsync(id, updatedProduct);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "Product data updated.");
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Product with Id {id} not found.");
        }


        [HttpPost]
        [Route("api/product")]
        public async Task<HttpResponseMessage> CreateProductAsync([FromBody] Product product)
        {
            if (product == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid product data.")
                };
            }

            bool result = await _productService.CreateProductAsync(product);

            if (result)
            {
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent("Product created successfully")
                };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Failed to create product.")
                };
            }
        }


        [HttpPut]
        [Route("api/product/delete/{id}")]
        public async Task<HttpResponseMessage> DeleteProductAsync(Guid id)
        {
            bool result = await _productService.DeleteProductAsync(id);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, $"Product with Id {id} successfully deleted.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Product with Id {id} not found.");
        }


        private OrderProductRest MapProductToRest(Product product)
        {

            OrderProductRest productRest = new OrderProductRest()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,

                Quantity = product.Quantity
            };

            return productRest;
        }


        private PagedList<OrderProductRest> MapProductListToRest(PagedList<Product> products)
        {
            if (products != null)
            {
                PagedList<OrderProductRest> productRests = new PagedList<OrderProductRest>();
                productRests.Results = products.Results.Select(product => new OrderProductRest

                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    IsActive = product.IsActive,
                    CategoryTitle = product.CategoryTitle,
                    CategoryId = product.CategoryId,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    CreatedBy = product.CreatedBy,
                    UpdatedBy = product.UpdatedBy,
                    Category = product.Category,
                    //Orders= product.Orders,
                    PersonCreated = product.PersonCreated,
                    PersonUpdated = product.PersonUpdated

                }).ToList();
                productRests.CurrentPage = products.CurrentPage;
                productRests.TotalCount = products.TotalCount;
                productRests.PageSize =  products.PageSize;
                return productRests;
            }
            return null;

        }


        private List<ProductsTableListRest> MapProductsToProductListRest(List<Product> products)
        {

            if (products != null)
            {
                List<ProductsTableListRest> productList = new List<ProductsTableListRest>();
                productList = products.Select(product => new ProductsTableListRest
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = (decimal)product.Price,
                    Quantity = (int)product.Quantity
                }).ToList();

                return productList;
            }
            
            return null;
        
        }

       // [HttpGet]
       // [Route("api/products/{orderId}")]
       // public async Task<HttpResponseMessage> GetProductsByOrderIdAsync(Guid orderId)
       //{
       //     if (orderId == null || orderId == Guid.Empty) 
       //     {
       //        return Request.CreateResponse(HttpStatusCode.BadRequest, "No products found by the given OrderId");
       //     }
       //     else
       //     {
       //         List<Product> products = await _productService.GetProductsByOrderIdAsync(orderId);

       //         if(products == null)
       //         {
       //             return Request.CreateResponse(HttpStatusCode.BadRequest, "No products found!");
       //         }

       //         List<ProductsListRest> productList = MapProductsToProductListRest(products);
       //         return Request.CreateResponse(HttpStatusCode.OK, productList);
       //     }
       // }

        [HttpGet]
        [Route("api/simpleproducts")]
        public async Task<HttpResponseMessage> GetSimpleProductsAsync()
        {
            List<Product> products = await _productService.GetSimpleProductsAsync();

            if (products == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No products found!");
            }

            List<ProductsTableListRest> simpleProducts = products.Select(product => new ProductsTableListRest
            {
                Id = product.Id,
                Name = product.Name,
                Price = (decimal)product.Price,
                Quantity = (int)product.Quantity
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, simpleProducts);
        }



        /*End------*/
    }
}
