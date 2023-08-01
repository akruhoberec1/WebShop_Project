using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Technoshop.Model;
using System.Web.Http;
using Technoshop.Service.Common;
using Technoshop.Service;
using Technoshop.WebApi.Models;
using Technoshop.WebApi.Models.PersonRest;


namespace Technoshop.WebApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/person")]
    public class PersonController : ApiController
    {
        // GET: Person
        private readonly IPersonService _personService;
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public async Task<HttpResponseMessage> GetPersonById(Guid id)
        {
            if (id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please insert valid Id.");
            }

            Person person = await GetPersonByIdAsync(id);

            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Did not find the person.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, MapPersonToRest(person));

        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("user/{id}")]
        public async Task<HttpResponseMessage> GetPersonByUserId(Guid id)
        {
            if (id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please insert valid Id.");
            }

            Person person = await _personService.GetPersonByUserId(id);

            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Did not find the person.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, MapPersonToRest(person));

        }

        public async Task<HttpResponseMessage> Put([FromBody] Guid id, Models.PersonRestModel personRest)
        {
            if (personRest == null || id == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Couldn't find the person you want to update, please try again.");
            }

            bool isUpdated = await _personService.UpdatePersonAsync(id, MapPersonFromRest(id, personRest));
            if (isUpdated == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Person updated successfully!");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong! Please try again.");
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Mvc.Route("{id}")]
        public async Task<HttpResponseMessage> DeletePerson(Guid id)
        {
            bool isDeleted = await _personService.DeletePersonAsync(id);

            if (isDeleted == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Person deleted!");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong, did not delete the person.");
        }
        public async Task<Person> GetPersonByIdAsync(Guid id)
        {
            Person person = await _personService.GetPersonByIdAsync(id);

            return person;
        }
        public PersonRestModel MapPersonToRest(Person person)
        {

            if (person != null)
            {
                PersonRestModel personRest = new PersonRestModel()
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Phone = person.Phone,
                    Email = person.Email,
                    CreatedAt = person.CreatedAt,
                   
                };  

                return personRest;
            }
            return null;
        }

        private Person MapPersonFromRest(Guid id, PersonRestModel personRest)
        {

            if (personRest != null)
            {
                Person person= new Person()
                {
                    Id =id,
                    FirstName = personRest.FirstName,
                    LastName = personRest.LastName,
                    Phone = personRest.Phone,
                    Email = personRest.Email,
                    CreatedAt = personRest.CreatedAt,

                    
                };

                return person;
            }
            return null;
        }

    }
}