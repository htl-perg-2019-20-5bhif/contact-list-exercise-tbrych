using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactListQuiz.Controllers
{
    public class Person
    {
        [Required]
        public int ID { get; set; }

        [MinLength(5)]
        [MaxLength(256)]
        [Required]
        public string Email { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }
    }

    [ApiController]
    [Route("contacts")]
    public class ContactListQuizController : ControllerBase
    {
        private static readonly Dictionary<int, Person> items =
            new Dictionary<int, Person>{
                { 0, new Person { ID=0, FirstName="Foo", LastName="Bar", Email="foo@bar.it" } },
                { 1, new Person { ID=1, FirstName="Test", LastName="Person", Email="test.person@cool.tech" } }
            };

        [HttpGet]
        public IActionResult GetAllItems()
        {
            return Ok(items.Values);
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] Person newItem)
        {
            try
            {    
                if (newItem == null || newItem.ID < 0 || String.IsNullOrEmpty(newItem.Email))
                {
                    return BadRequest("Invalid arguments!");
                }
           
                if (items.TryGetValue(newItem.ID, out _))
                {
                    return BadRequest("Person with ID " + newItem.ID + " already exists!");
                }

                items[newItem.ID] = newItem;
                return Created("", newItem);
            }
            catch (Exception e)
            {
                return BadRequest("An Error happened! Exception: " + e.GetType().ToString());
            }
        }

        [HttpDelete]
        [Route("{index}")]
        public IActionResult DeleteItem(int index)
        {
            if (index < 0)
            {
                BadRequest("Invalid ID");
            }

            if (items.TryGetValue(index, out Person ret))
            {
                items.Remove(index);
                return NoContent();
            }
            return NotFound("Person with id " + index + " not found!");
        }

        [HttpGet]
        [Route("findByName")]
        public IActionResult GetItem([FromQuery] string nameFilter)
        {
            if (string.IsNullOrEmpty(nameFilter))
            {
                return BadRequest("Invalid Name");
            }

            IEnumerable<Person> result = from curPerson in items.Values
                         where
                                (!string.IsNullOrEmpty(curPerson.FirstName) && curPerson.FirstName.ToLower().Equals(nameFilter.ToLower())) ||
                                (!string.IsNullOrEmpty(curPerson.LastName) && curPerson.LastName.ToLower().Equals(nameFilter.ToLower()))
                         select curPerson;

            if (result.Count() > 0)
            {
                return Ok(result);
            }

            return NotFound("Person with Name " + nameFilter + " not found!");
        }
    }
}
