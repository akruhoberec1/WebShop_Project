using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model.Common;
using Technoshop.Common;
using Technoshop.Repository.Common;
using Technoshop.Service.Common;
using System.Runtime.Remoting.Messaging;
using Technoshop.Model;
using System.Runtime.InteropServices;

namespace Technoshop.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedList<Category>> GetCategoriesAsync(CategoryFilter filter, Sorting sorting, Paginate pagination)
        {
            PagedList<Category> categories = await _categoryRepository.GetCategoriesAsync(filter, sorting, pagination);
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            Category category = await _categoryRepository.GetCategoryByIdAsync(id);
            return category;
        }
        public async Task<bool> CreateCategoryAsync(Category category)
        {
            bool isCreated = await _categoryRepository.CreateCategoryAsync(category);
            return isCreated;
        }
        
        public async Task<bool> DeleteCategoryAsync(Guid id)

        {
            bool isDeleted = await _categoryRepository.DeleteCategoryAsync(id);
            return isDeleted;

        }

        public async Task<bool> UpdateCategoryAsync(Guid id, Category category)
        {
            bool isUpdated = await _categoryRepository.UpdateCategoryAsync(id, category);
            return isUpdated;
        }

    }
}
