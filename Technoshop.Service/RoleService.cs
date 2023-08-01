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

   

    
    
        public class RoleService : IRoleService
        {
            private readonly IRoleService _roleRepository;
            public RoleService(IRoleService roleRepository)
            {
                _roleRepository = roleRepository;
            }

            public async Task<PagedList<Role>> GetRoleAsync(RoleFilter filter, Sorting sorting, Paginate pagination)
            {
                PagedList<Role> role = await _roleRepository.GetRoleAsync(filter, sorting, pagination);
                return role;
            }

            public async Task<Role> GetRoleByIdAsync(Guid id)
            {
                Role role = await _roleRepository.GetRoleByIdAsync(id);
                return role;
            }

            public async Task<bool> CreateRoleAsync(Role role)
            {
                bool isCreated = await _roleRepository.CreateRoleAsync(role);
                return isCreated;
            }

            public async Task<bool> DeleteRoleAsync(Guid id)
            {
                bool isDeleted = await _roleRepository.DeleteRoleAsync(id);
                return isDeleted;
            }

            public async Task<bool> UpdateRoleAsync(Guid id, Role role)
            {
                bool isUpdated = await _roleRepository.UpdateRoleAsync(id, role);
                return isUpdated;
            }
        }
    

}
