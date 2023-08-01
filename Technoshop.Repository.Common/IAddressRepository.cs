using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Repository.Common
{
    public interface IAddressRepository
    {
        Task<PagedList<Address>> GetAddressAsync(AddressFilter filter, Sorting sorting, Paginate pagination);

        Task<Address> GetAddressByIdAsync(Guid id);

        Task<bool> CreateAddressAsync(Guid id,Address address);

        Task<bool> UpdateAddressAsync(Guid id, Address address);

        Task<bool> DeleteAddressAsync(Guid id);
        Task<List<Address>> GetAddressesByPersonId(Guid id);
        Task<List<Address>> GetBillingAddressesAsync(Guid id);
        Task<List<Address>> GetShippingAddressesAsync(Guid id);
        Task<List<Address>> GetOrderBillingAddressesAsync(Guid id);
        Task<List<Address>> GetOrderShippingAddressesAsync(Guid id);
    }
}
