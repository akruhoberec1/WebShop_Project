using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Technoshop.Model;
using Technoshop.Service.Common;
using Technoshop.WebApi.Models;
using Technoshop.Common;
using Technoshop.WebApi.Models.AddressRest;
using System.Collections.Generic;
using Technoshop.WebApi.Models.Order;
using PdfSharp;
using Technoshop.WebApi.Models.Product;

namespace Technoshop.WebApi.Controllers
{

    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> Get(int pageSize = 5, int pageNumber = 1, string searchQuery = "", decimal? minPrice=null, decimal? maxPrice = null, string sortBy = "", bool sortOrder = false)
        {
            Paginate paginate = new Paginate()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            OrderSorting sorting = new OrderSorting()
            {
                SortBy = sortBy,
                OrderBy = sortOrder
            };

            OrderFilter filtering = new OrderFilter()
            {
                SearchQuery = searchQuery,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            PagedList<Order> orders = await _orderService.GetOrdersAsync(paginate, filtering, sorting);


            if(orders != null)
            {
                PagedList<OrderRest> ordersRest = MapOrdersToRest(orders);

                return Request.CreateResponse(HttpStatusCode.OK, ordersRest);

            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Orders not found.");
        }


        /*
         * GET BY ID METHOD
         */
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            if (id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please insert valid Id.");
            }

            Order order = await GetOrderByIdAsync(id);

            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Did not find the order.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, MapOrderToRest(order));

        }


        /******
         * **   UPDATE METHOD
         ******/
        [HttpPut]
        [Route("{id}")]

        public async Task<HttpResponseMessage> Put(Guid id,[FromBody]UpdateOrderRest orderRest)
        {
            if(orderRest == null || id == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Couldn't find the order you want to update, please try again.");
            }

            bool isUpdated = await _orderService.UpdateOrderAsync(id, MapOrderForUpdate(orderRest));
            if (isUpdated == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Order updated successfully!");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong! Please try again.");
        }


        /********
         * DELETE ORDER
         *******/
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> DeleteOrder(Guid id)
        {
            bool isDeleted = await _orderService.DeleteOrderAsync(id);

            if (isDeleted == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Order deleted!");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong, did not delete the order.");
        }


        /*
         * CREATE ORDER
         * */

        [HttpPost]
        [Route("create")]
        public async Task<HttpResponseMessage> Post([FromBody]CreateOrderRest orderRest)
        {
            Order order = await _orderService.CreateOrderAsync(MapOrderFromCreateRest(orderRest));

            return (order != null) ? Request.CreateResponse(HttpStatusCode.OK, $"Your order has been created, check e-mail about details!\n{order}") : Request.CreateResponse(HttpStatusCode.BadRequest,"Sorry, the order could not be placed.");
        }


        /*
         * Only get and return order
         */
        private async Task<Order> GetOrderByIdAsync(Guid id)
        {
            Order order = await _orderService.GetOrderByIdAsync(id);

            return order;
        }

        /*
         * 
         * M A P P I N G   T O   A N D   F R O M   R E S T    M O D E L S
         * 
         * */

        private PagedList<OrderRest> MapOrdersToRest(PagedList<Order> orders)
        {
            if (orders != null)
            {
               PagedList<OrderRest> ordersRest = new PagedList<OrderRest>();
               ordersRest.Results = orders.Results.Select(order => new OrderRest
                {
                    Id = order.Id,
                    Person = new Models.PersonRest.OrderPersonRest
                    {
                        Id = order.PersonId,
                        FirstName = order.Person.FirstName,
                        LastName = order.Person.LastName,
                        Phone = order.Person.Phone,
                        Email = order.Person.Email
                    },
                    ShippingAddress = new OrderAddressRest
                    {
                        Id=order.ShippingAddress.Id,    
                        StreetName = order.ShippingAddress.StreetName,
                        StreetNumber = order.ShippingAddress.StreetNumber,
                        City = order.ShippingAddress.City,
                        Zipcode = order.ShippingAddress.ZipCode
                    },
                    BillingAddress = new OrderAddressRest
                    {
                        Id=order.BillingAddress.Id, 
                        StreetName = order.BillingAddress.StreetName,
                        StreetNumber = order.ShippingAddress.StreetNumber,
                        City = order.BillingAddress.City,
                        Zipcode = order.BillingAddress.ZipCode
                    },
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    ProductsRest = order.Products.Select(product => new ProductsTableListRest
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity
                    }).ToList()
                }).ToList();
                ordersRest.CurrentPage = orders.CurrentPage;
                ordersRest.TotalPages = (int)Math.Ceiling((double)orders.TotalCount / orders.PageSize);
                ordersRest.TotalCount = orders.TotalCount;
                ordersRest.PageSize = orders.PageSize;

                return ordersRest;

            }

            return null;
        }


        private OrderRest MapOrderToRest(Order order)
        {

            if (order != null)
            {
                OrderRest orderRest = new OrderRest()
                {
                    Id = order.Id,
                    Person = new Models.PersonRest.OrderPersonRest()
                    {
                        Id = order.PersonId,
                        FirstName = order.Person.FirstName,
                        LastName = order.Person.LastName,   
                        Phone = order.Person.Phone,
                        Email = order.Person.Email
                    },
                    ShippingAddress = new OrderAddressRest()
                    {
                        StreetName = order.ShippingAddress.StreetName,
                        StreetNumber = order.ShippingAddress.StreetNumber,
                        City = order.ShippingAddress.City,
                        Zipcode = order.ShippingAddress.ZipCode
                    },
                    BillingAddress = new OrderAddressRest()
                    {
                        StreetName = order.BillingAddress.StreetName,
                        StreetNumber = order.ShippingAddress.StreetNumber,
                        City = order.BillingAddress.City,
                        Zipcode = order.BillingAddress.ZipCode
                    },
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    ProductsRest = order.Products.Select(product => new ProductsTableListRest
                    {
                       Name = product.Name,
                       Price = product.Price,
                       Quantity = product.Quantity
                    }).ToList(),
                };

                return orderRest;
            }
            return null;
        }

        private Order MapOrderFromRest(OrderRest orderRest)
        {

            if (orderRest != null)
            {
                Order order = new Order()
                {
                    
                    Person = new Person()
                    {
                        Id = orderRest.Person.Id,
                        FirstName = orderRest.Person.FirstName,
                        LastName = orderRest.Person.LastName,   
                        Phone = orderRest.Person.Phone, 
                        Email = orderRest.Person.Email
                    },
                    ShippingAddress = new Address()
                    {
                        Id = orderRest.ShippingAddress.Id,  
                        StreetName = orderRest.ShippingAddress.StreetName,
                        StreetNumber = orderRest.ShippingAddress.StreetNumber,
                        City = orderRest.ShippingAddress.City,  
                        ZipCode = orderRest.ShippingAddress.Zipcode
                    },
                    BillingAddress = new Address()
                    {
                        Id = orderRest.BillingAddress.Id,  
                        StreetName = orderRest.BillingAddress.StreetName,
                        StreetNumber = orderRest.BillingAddress.StreetNumber,
                        City = orderRest.BillingAddress.City,
                        ZipCode = orderRest.BillingAddress.Zipcode
                    },
                    TotalPrice = orderRest.TotalPrice,
                    CreatedAt = orderRest.CreatedAt,
                    Products = orderRest.ProductsRest.Select(product => new Product
                    {
                        Id = product.Id,    
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity

                    }).ToList(),
                };

                return order;
            }
            return null;
        }

        private Order MapOrderForUpdate(UpdateOrderRest orderRest)
        {

            if (orderRest != null)
            {
                Order order = new Order()
                {
                    PersonId = orderRest.PersonId,
                    ShippingAddress = new Address()
                    {
                        Id = orderRest.ShippingAddressId
                    },
                    BillingAddress = new Address()
                    {
                        Id = orderRest.BillingAddressId
                    },
                    TotalPrice = orderRest.TotalPrice,
                    Products = orderRest.Products.Select(product => new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity
                    }).ToList(),

                };

                return order;
            }
            return null;
        }

        private Order MapOrderFromCreateRest(CreateOrderRest orderRest)
        {

            if (orderRest != null)
            {
                Order order = new Order()
                {

                    Person = new Person()
                    {
                        Id = orderRest.PersonId
                    },
                    ShippingAddress = new Address()
                    {
                        Id = orderRest.ShippingAddressId
                    },
                    BillingAddress = new Address()
                    {
                        Id = orderRest.BillingAddressId
                    },
                    TotalPrice = orderRest.TotalPrice,
                    CreatedAt = orderRest.CreatedAt,
                    Products = orderRest.ProductsRest.Select(product => new Product
                    {
                        Id = product.Id,
                        Price = product.Price,
                        Quantity = product.Quantity

                    }).ToList(),
                };

                return order;
            }
            return null;
        }


    }
}
    


