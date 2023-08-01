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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedList<User>> GetUserAsync(UserFilter filter, Sorting sorting, Paginate pagination)
        {
            PagedList<User> user = await _userRepository.GetUserAsync(filter, sorting, pagination);
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            User user = await _userRepository.GetUserByIdAsync(id);
            return user;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            bool isCreated = await _userRepository.CreateUserAsync(user);
            return isCreated;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            bool isDeleted = await _userRepository.DeleteUserAsync(id);
            return isDeleted;
        }

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            bool isUpdated = await _userRepository.UpdateUserAsync(id,user);
            return isUpdated;
        }

        public async Task<User> RegisterUser(Registration data)
        {
            User user = await _userRepository.RegisterUser(data);
            return (user != null) ? user : null;
          
        }

        public async Task<User> RegisterAdmin(Registration data)
        {
            User user = await _userRepository.RegisterAdmin(data);
            return (user != null) ? user : null;

        }

        public async Task<User> LoginUserAsync(LoginData data)
        {
            User user = await _userRepository.LoginUserAsync(data);
            
            return (user != null) ? user : null;  
        }

    }
}

