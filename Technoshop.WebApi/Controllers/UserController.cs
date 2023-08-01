using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service.Common;
using System.Web.Http;
using Technoshop.Service;
using Technoshop.WebApi.Models;
using Technoshop.WebApi.Models.User;
using System.Security.Cryptography;

namespace Technoshop.WebApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
       
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/user")]
        public async Task<HttpResponseMessage> GetCategoriesAsync(
               int pageNumber = 1,
               int pageSize = 10,
               string orderBy = "",
               string sortBy = "",
               string searchQuery = "",
               bool isActive = false)
        {
           UserFilter filterUser = new UserFilter
            {
                SearchQuery = searchQuery,
                IsActive = isActive
            };

            Sorting sorting = new Sorting
            {
                SortBy = string.IsNullOrEmpty(sortBy) ? "UserName" : sortBy,
                OrderBy = string.IsNullOrEmpty(orderBy) ? "ASC" : orderBy
            };

            Paginate pagination = new Paginate
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            PagedList<User> paged = await _userService.GetUserAsync(filterUser, sorting, pagination);
            if (paged.Results.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, paged);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "No user found.");
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/user")]
        public async Task<HttpResponseMessage> GetUserByIdAsync(Guid id)
        {
            User user = await _userService.GetUserByIdAsync(id);

            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"User with Id {id} not found.");
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/user")]
        public async Task<HttpResponseMessage> UpdateUserAsync(Guid id, [FromBody] User updated)
        {
            if (updated == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid user data.");

            updated.Id = id;
            bool result = await _userService.UpdateUserAsync(id, updated);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "User data updated.");
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with Id {id} not found.");
        }
       
        
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/user")]
        public async Task<HttpResponseMessage> CreateCategoryAsync([FromBody] User user)
        {
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid user data.");

            bool result = await _userService.CreateUserAsync(user);

            if (result)
                return Request.CreateResponse(HttpStatusCode.Created, "User created successfully.");
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create user.");
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/user")]
        public async Task<HttpResponseMessage> DeleteUserAsync(Guid id)
        {
            bool result = await _userService.DeleteUserAsync(id);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, $"User with Id {id} successfully deleted.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"User with Id {id} not found.");
        }
        public UserRest MapUserToRest(User user)
        {
            if (user != null)
            {
                UserRest userRest = new UserRest()
                {
                    UserName = user.UserName,
                    Password = user.Password
                };
                return userRest;
            }

            return null;
        }
        private PagedList<UserRest> MapProductListToRest(PagedList<User> users)
        {
            if (users != null)
            {
                PagedList<UserRest> usersRest = new PagedList<UserRest>(

                    )
                {
                    Results=users.Results.Select(user => new UserRest

                        {

                            UserName = user.UserName,
                            Password = user.Password


                        }
                        ).ToList(),
                        CurrentPage=users.CurrentPage,
                        TotalCount=users.TotalCount,
                        PageSize=users.PageSize
                };
                return usersRest;
            }
            return null;

        }


    }
    
}