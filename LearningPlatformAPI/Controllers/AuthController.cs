using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningPlatformAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //[Route("signup")]
        //public async Task<IActionResult> GetSignups()
        //{
        //    if (_context.Person == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok("Login page would sit here");
        //}

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> PostPerson([FromBody]Person person)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'DataContext.Person'  is null.");
            }
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            return Ok(new { UserID = person.UserId});
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password) {

            if (_context.CheckCredentials(email, password))
            {           
                return Ok($"Login successful! With credentials { email } and { password }");
            }

            //with bool
            //if (_context.CheckCredentials(email, password))
            //{           
            //    return Ok($"Login successful! With {email} and {password}");
            //}
            
            if (email != "email")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");

            }
            
            if (password != "password")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");
            }

            return BadRequest();
        }
    }
}
