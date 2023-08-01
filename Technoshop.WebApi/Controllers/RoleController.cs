using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service.Common;
using Technoshop.WebApi.Models;
using Technoshop.WebApi.Models.Role;

namespace Technoshop.WebApi.Controllers
{
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/role")]
        public async Task<HttpResponseMessage> GetRoleAsync(
               int pageNumber = 1,
               int pageSize = 10,
               string orderBy = "",
               string sortBy = "",
               string searchQuery = "",
               bool isActive = false)
        {
            RoleFilter filter = new RoleFilter
            {
                SearchQuery = searchQuery,
                IsActive = isActive
            };

            Sorting sorting = new Sorting
            {
                SortBy = string.IsNullOrEmpty(sortBy) ? "Title" : sortBy,
                OrderBy = string.IsNullOrEmpty(orderBy) ? "ASC" : orderBy
            };

            Paginate pagination = new Paginate
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            PagedList<Role> paged = await _roleService.GetRoleAsync(filter, sorting, pagination);
            if (paged.Results.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, paged);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "No role found.");
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/role")]
        public async Task<HttpResponseMessage> GetRoleByIdAsync(Guid id)
        {
            Role role = await _roleService.GetRoleByIdAsync(id);

            if (role != null)
                return Request.CreateResponse(HttpStatusCode.OK, role);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Role with Id {id} not found.");
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/role")]
        public async Task<HttpResponseMessage> UpdateRoleAsync(Guid id, [FromBody] Role updated)
        {
            if (updated == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid role data.");

            updated.Id = id;
            bool result = await _roleService.UpdateRoleAsync(id, updated);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "Role data updated.");
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Role with Id {id} not found.");
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/role")]
        public async Task<HttpResponseMessage> CreateRoleAsync([FromBody] Role role)
        {
            if (role == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid role data.");

            bool result = await _roleService.CreateRoleAsync(role);

            if (result)
                return Request.CreateResponse(HttpStatusCode.Created, "User created successfully.");
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create user.");
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/role")]
        public async Task<HttpResponseMessage> DeleteUserAsync(Guid id)
        {
            bool result = await _roleService.DeleteRoleAsync(id);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, $"Role with Id {id} successfully deleted.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Role with Id {id} not found.");
        }


        public RoleRest MapRoleToRest(Role role)
        {
            if (role != null)
            {
                RoleRest rolerest = new RoleRest()
                {
                    Title = role.Title
                };
                return rolerest;
            }

            return null;
        }


        public PagedList<RoleRest> MapRolesToRest(PagedList<Role> roles)
        {
            if (roles != null)
            {
                PagedList<RoleRest> roleRests = new PagedList<RoleRest>(


                    )
                {
                 Results=roles.Results.Select(role => new RoleRest
                    {
                        Title = role.Title
                    }
                    ).ToList(),
                    CurrentPage=roles.CurrentPage,
                    TotalCount=roles.TotalCount,
                    PageSize = roles.PageSize
                }
                ;
                return roleRests;

            }
            return null;
        }


    }
}
