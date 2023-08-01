using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Repository.Common
{
    public interface IUserRepository
    {
        Task<PagedList<User>> GetUserAsync(UserFilter filter, Sorting sorting, Paginate pagination);

        Task<User> GetUserByIdAsync(Guid id);

        Task<bool> CreateUserAsync(User user);

        Task<bool> UpdateUserAsync(Guid id, User user);

        Task<bool> DeleteUserAsync(Guid id);
        Task<User> ValidateUserAsync(User request);
        Task<Role> GetUserRoleAsync(Guid id);
        Task<User> RegisterUser(Registration data);
        Task<User> RegisterAdmin(Registration data);
        Task<User> LoginUserAsync(LoginData data);
    }
}
