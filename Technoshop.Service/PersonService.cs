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
    public class PersonService: IPersonService
    {
        private readonly IPersonRepository _personRepository;
        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }     

        public async Task<PagedList<Person>> GetPersonAsync(PersonFilter filter, Sorting sorting, Paginate pagination)
        {
            PagedList<Person> person = await _personRepository.GetPersonAsync(filter, sorting, pagination);
            return person;
        }

        public async Task<Person> GetPersonByIdAsync(Guid id)
        {
            Person person = await _personRepository.GetPersonByIdAsync(id);
            return person;
        }

        public async Task<Person> GetPersonByUserId(Guid id)
        {
            Person person = await _personRepository.GetPersonByUserId(id);
            return person;
        }

        public async Task<bool> CreatePersonAsync(Person person)
        {
            bool isCreated = await _personRepository.CreatePersonAsync(person);
            return isCreated;
        }

        public async Task<bool> DeletePersonAsync(Guid id)
        {
            bool isDeleted = await _personRepository.DeletePersonAsync(id);
            return isDeleted;
        }

        public async Task<bool> UpdatePersonAsync(Guid id, Person person)
        {
            bool isUpdated = await _personRepository.UpdatePersonAsync(id, person);
            return isUpdated;
        }



    }
}

