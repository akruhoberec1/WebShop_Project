using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Repository.Common
{
    public interface IRoleRepository
    {
        Task<PagedList<Role>> GetRoleAsync(RoleFilter filter, Sorting sorting, Paginate pagination);

        Task<Role> GetRoleByIdAsync(Guid id);

        Task<bool> CreateRoleAsync(Role role);

        Task<bool> UpdateRoleAsync(Guid id, Role role);

        Task<bool> DeleteRoleAsync(Guid id);
    }
}
