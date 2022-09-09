using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.Models;
using LearningPlatformAPI.ActionFilters;

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DataContext _context;

        public PersonController(DataContext context)
        {
            _context = context;
        }

        // Endpoints

        // actionfilter disabled 
        //[ServiceFilter(typeof(SampleActionFilter))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
        {
            var person = _context.GetAllPersons();

            if (person.Count == 0)
            {
                return NotFound(new { Message = "Person not found! "});
            }
            return person;
        }

        // actionfilter disabled
        //[ServiceFilter(typeof(SampleActionFilter))]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = _context.GetPerson(id);

            if (person == null)
            {
                return NotFound(new { Message = $"Person with Id nr {id} not found!! "});
            }          
            return person;
        }

        // how to do this best??? 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.UserId)
            {
                return BadRequest(new { Message = $"Entered ID {id} does not match UserID!!" });
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PersonExists(id))
                {
                    return NotFound(new { Message = "Person not found!!"});
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { Message = "Person updated!!" });
        }
                
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {

            var personToPost = _context.PostPerson(person);

            if (personToPost == null)
            {
                return Problem("Entity set 'DataContext.Person'  is null.");
            }
             return CreatedAtAction("GetPerson", new { id = person.UserId }, person);
        }
                
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = _context.DeletePerson(id);
                                    
            if (person == null)
            {
                return NotFound(new { Message = "Person not found! "});
            }      
            return Ok(new { Message = $"Person {id} removed from DB! "});
        }

        
    }
}
