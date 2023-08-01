using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface ICategoryService
    {
        Task<PagedList<Category>> GetCategoriesAsync(CategoryFilter filter, Sorting sorting, Paginate pagination);
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<bool> UpdateCategoryAsync(Guid id, Category category);
    }
}
