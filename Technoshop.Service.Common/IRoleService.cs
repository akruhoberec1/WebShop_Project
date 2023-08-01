using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IRoleService
    {
        Task<PagedList<Role>> GetRoleAsync(RoleFilter filter, Sorting sorting, Paginate pagination);
        Task<Role> GetRoleByIdAsync(Guid id);
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(Guid id);
        Task<bool> UpdateRoleAsync(Guid id, Role role);
    }
}
