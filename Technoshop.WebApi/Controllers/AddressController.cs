using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service;
using Technoshop.Service.Common;
using Technoshop.WebApi.Models;
using Technoshop.WebApi.Models.Address;
using Technoshop.WebApi.Models.AddressRest;

namespace Technoshop.WebApi.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.RoutePrefix("api/address")]

    public class AddressController : ApiController
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
      

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        public async Task<HttpResponseMessage> GetAddressesAsync(
               int pageNumber = 1,
               int pageSize = 10,
               string orderBy = "",
               string sortBy = "",
               string searchQuery = "",
               bool isActive = true)
        {
            AddressFilter filter = new AddressFilter
            {
                SearchQuery = searchQuery,
                IsActive = isActive
            };

            Sorting sorting = new Sorting
            {
                SortBy = string.IsNullOrEmpty(sortBy) ? "ZipCode" : sortBy,
                OrderBy = string.IsNullOrEmpty(orderBy) ? "ASC" : orderBy
            };

            Paginate pagination = new Paginate
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            PagedList<Address> pagedAddresses = await _addressService.GetAddressAsync(filter, sorting, pagination);
            if (pagedAddresses.Results.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, MapAddressesToRest(pagedAddresses));
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "No address found.");
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public async Task<HttpResponseMessage> GetAddressByIdAsync(Guid id)
        {
            Address address = await _addressService.GetAddressByIdAsync(id);

            if (address != null)
                return Request.CreateResponse(HttpStatusCode.OK, address);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address with Id {id} not found.");
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("billing/{id}")]
        public async Task<HttpResponseMessage> GetBillingAddresses(Guid id)
        {
            List<Address> billingAddresses = await _addressService.GetBillingAddressesAsync(id);

            if (billingAddresses != null)
                return Request.CreateResponse(HttpStatusCode.OK, billingAddresses);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address with UserId {id} not found.");
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("orderbilling/{id}")]
        public async Task<HttpResponseMessage> GetOrderBillingAddresses(Guid id)
        {
            List<Address> billingAddresses = await _addressService.GetOrderBillingAddressesAsync(id);

            if (billingAddresses != null)
                return Request.CreateResponse(HttpStatusCode.OK, billingAddresses);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address with UserId {id} not found.");
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("shipping/{id}")]
        public async Task<HttpResponseMessage> GetShippingAddressesAsync(Guid id)
        {
            List<Address> shippingAddresses = await _addressService.GetShippingAddressesAsync(id);

            if (shippingAddresses != null)
                return Request.CreateResponse(HttpStatusCode.OK, shippingAddresses);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address not found.");
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ordershipping/{id}")]
        public async Task<HttpResponseMessage> GetOrderShippingAddressesAsync(Guid id)
        {
            List<Address> shippingAddresses = await _addressService.GetOrderShippingAddressesAsync(id);

            if (shippingAddresses != null)
                return Request.CreateResponse(HttpStatusCode.OK, shippingAddresses);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address not found.");
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("update/{id}")] 
        public async Task<HttpResponseMessage> UpdateAddressAsync(Guid id, [FromBody] Address updatedAddress)
        {
            if (updatedAddress == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid address data.");

            updatedAddress.Id = id;
            bool result = await _addressService.UpdateAddressAsync(id, updatedAddress);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "Address data updated.");
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Address with Id {id} not found.");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("{id}")]
        public async Task<HttpResponseMessage> Post(Guid id, [FromBody] AddressRest addressRest)
        {
            if (addressRest == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid address data.");

            Address address = MapAddressFromRest(addressRest);

            bool result = await _addressService.CreateAddressAsync(id, address);

            if (result)
                return Request.CreateResponse(HttpStatusCode.Created, "Address created successfully.");
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed to create address.");
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        public async Task<HttpResponseMessage> DeleteAddressAsync(Guid id)
        {
            bool result = await _addressService.DeleteAddressAsync(id);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, $"Address with Id {id} successfully deleted.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Address with Id {id} not found.");
        }



        public OrderAddressRest MapAddressesToRest(Address address)
        {
            if (address != null)
            {
                OrderAddressRest addressRest = new OrderAddressRest()
                {
                    StreetName = address.StreetName,
                    StreetNumber = address.StreetNumber,
                    City = address.City,
                    Zipcode = address.ZipCode
                };
                return addressRest;
            }

            return null;
        }


        public PagedList<OrderAddressRest> MapAddressesToRest(PagedList<Address> addresses)
        {
            if (addresses != null)
            {
                PagedList<OrderAddressRest> addressRest = new PagedList<OrderAddressRest>(


                    )
                {
                Results= addresses.Results.Select(address => new OrderAddressRest
                    {
                       StreetName= address.StreetName,
                       StreetNumber= address.StreetNumber,
                       City=address.City,
                       Zipcode=address.ZipCode
                    }
                    ).ToList(),
                    CurrentPage=addresses.CurrentPage,
                    TotalCount=addresses.TotalCount,
                    PageSize = addresses.PageSize
                };
                return addressRest;

            }
            return null;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]  
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            List<Address> addresses = await _addressService.GetAddressesByPersonId(id);

            if(addresses == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "We couldn't get the addresses");
            }

            return Request.CreateResponse(HttpStatusCode.OK, MapAddressesToRest(addresses));

        }





        private List<OrderAddressRest> MapAddressesToRest(List<Address> addresses)
        {
            if(addresses != null)
            {
                List<OrderAddressRest> addressesRest = addresses.Select(address => new OrderAddressRest
                {
                    Id = address.Id,
                    StreetName = address.StreetName,
                    StreetNumber = address.StreetNumber,
                    City = address.City,
                    Zipcode = address.ZipCode
                }).ToList();

                return addressesRest;
            }
            return null;
        }

        private Address MapAddressFromRest(AddressRest addressRest)
        {
            if (addressRest != null)
            {
                Address address = new Address()
                {
                    StreetName = addressRest.StreetName,
                    StreetNumber = addressRest.StreetNumber,
                    City = addressRest.City,
                    ZipCode = addressRest.ZipCode,
                    IsShipping = addressRest.IsShipping
                };

                return address;
            }
            return null;
        }




    }
}


