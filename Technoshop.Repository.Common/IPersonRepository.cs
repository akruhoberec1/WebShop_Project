using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;

namespace Technoshop.Repository.Common
{
    public interface IPersonRepository
    {
        Task<PagedList<Person>> GetPersonAsync(PersonFilter personFilter,Sorting sorting,Paginate paginate);

        Task<Person> GetPersonByIdAsync(Guid id);

        Task<Person> GetPersonByUserId(Guid id);

        Task<bool> CreatePersonAsync(Person person);

        Task<bool> UpdatePersonAsync(Guid id, Person person);

        Task<bool> DeletePersonAsync(Guid id);

    }
}
