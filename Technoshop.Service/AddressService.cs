using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Repository.Common;
using Technoshop.Service.Common;

namespace Technoshop.Service
{
    public class AddressService:IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<PagedList<Address>> GetAddressAsync(AddressFilter filter, Sorting sorting, Paginate pagination)
        {
            PagedList<Address> address = await _addressRepository.GetAddressAsync(filter,sorting,pagination);
            return address;
        }

        public async Task<Address> GetAddressByIdAsync(Guid id)
        {
            Address address = await _addressRepository.GetAddressByIdAsync(id);
            return address;
        }

        public async Task<bool> CreateAddressAsync(Guid id, Address address)
        {
            bool isCreated = await _addressRepository.CreateAddressAsync(id, address);
            return isCreated;
        }

        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            bool isDeleted = await _addressRepository.DeleteAddressAsync(id);
            return isDeleted;
        }

        public async Task<bool> UpdateAddressAsync(Guid id, Address address)
        {
            bool isUpdated = await _addressRepository.UpdateAddressAsync(id, address);
            return isUpdated;
        }

        public async Task<List<Address>> GetAddressesByPersonId(Guid id)
        {
            List<Address> addresses = await _addressRepository.GetAddressesByPersonId(id);

            if(addresses != null)
            {
                return addresses;   
            }
            return null;
        }

        public async Task<List<Address>> GetBillingAddressesAsync(Guid id)
        {
            List<Address> addresses = await _addressRepository.GetBillingAddressesAsync(id);

            if (addresses != null)
            {
                return addresses;
            }
            return null;
        }

        public async Task<List<Address>> GetOrderBillingAddressesAsync(Guid id)
        {
            List<Address> addresses = await _addressRepository.GetOrderBillingAddressesAsync(id);

            if (addresses != null)
            {
                return addresses;
            }
            return null;
        }

        public async Task<List<Address>> GetShippingAddressesAsync(Guid id)
        {
            List<Address> addresses = await _addressRepository.GetShippingAddressesAsync(id);

            if (addresses != null)
            {
                return addresses;
            }
            return null;
        }

        public async Task<List<Address>> GetOrderShippingAddressesAsync(Guid id)
        {
            List<Address> addresses = await _addressRepository.GetOrderShippingAddressesAsync(id);

            if (addresses != null)
            {
                return addresses;
            }
            return null;
        }

    }
}

