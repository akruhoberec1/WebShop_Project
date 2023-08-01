using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web.Http;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Service;
using Technoshop.Service.Common;
using Technoshop.WebApi.Models;

namespace Technoshop.WebApi.Controllers
{
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("api/categories/")]
        public async Task<HttpResponseMessage> GetCategoriesAsync(
               int pageNumber = 1,
               int pageSize = 10,
               string orderBy = "",
               string sortBy = "",
               string searchQuery = "",
               bool isActive = false)
        {
            CategoryFilter filter = new CategoryFilter
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

            PagedList<Category> pagedCategories = await _categoryService.GetCategoriesAsync(filter, sorting, pagination);
            if (pagedCategories.Results.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, pagedCategories);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "No category found.");
            }
        }


        [HttpGet]
        [Route("api/category/{id}")]
        public async Task<HttpResponseMessage> GetCategoryByIdAsync(Guid id)
        {
            Category category = await _categoryService.GetCategoryByIdAsync(id);

            if (category != null)


                return Request.CreateResponse(HttpStatusCode.OK, category);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Category with Id {id} not found.");
        }


        [HttpPut]
        [Route("api/category/update/{id}")]
        public async Task<HttpResponseMessage> UpdateCategoryAsync(Guid id, [FromBody] Category updatedCategory)
        {
            if (updatedCategory == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid category data.");

            updatedCategory.Id = id;
            bool result = await _categoryService.UpdateCategoryAsync(id, updatedCategory);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "Category data updated.");
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Category with Id {id} not found.");
        }

        [HttpPost]
        [Route("api/category")]
        public async Task<HttpResponseMessage> CreateCategoryAsync([FromBody] Category category)
        {
            if (category == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid category data.");

            bool result = await _categoryService.CreateCategoryAsync(category);

            if (result)
                return Request.CreateResponse(HttpStatusCode.Created, "Category created successfully.");
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create category.");
        }

        [HttpPut]
        [Route("api/category/delete/{id}")]
        public async Task<HttpResponseMessage> DeleteCategoryAsync(Guid id)
        {
            bool result = await _categoryService.DeleteCategoryAsync(id);

            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, $"Category with Id {id} successfully deleted.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Category with Id {id} not found.");
        }



        public CategoryRest MapCategoryToRest(Category category)
        {
            if (category != null)
            {
                CategoryRest categoryRest = new CategoryRest()
                {
                    Id = category.Id,
                    Title = category.Title
                };
                return categoryRest;
            }

            return null;
        }


        public PagedList<CategoryRest> MapCategoriesToRest(PagedList<Category> categories)
        {
            if (categories != null)
            {
                PagedList<CategoryRest> categoryRests = new PagedList<CategoryRest>()
                {
                    Results = categories.Results.Select(category => new CategoryRest
                    {
                        Id = category.Id,
                        Title = category.Title
                    }
                    ).ToList(),
                    CurrentPage = categories.CurrentPage,
                    PageSize = categories.PageSize,
                    TotalCount = categories.TotalCount
                };
                return categoryRests;

            }
            return null;
        }


    }


}
