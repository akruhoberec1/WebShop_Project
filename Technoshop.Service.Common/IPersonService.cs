using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
     public interface IPersonService
    {
        Task<PagedList<Person>> GetPersonAsync(PersonFilter filter, Sorting sorting, Paginate pagination);
        Task<Person> GetPersonByIdAsync(Guid id);
        Task<Person> GetPersonByUserId(Guid id);
        Task<bool> CreatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(Guid id);
        Task<bool> UpdatePersonAsync(Guid id, Person person);
    }
}
