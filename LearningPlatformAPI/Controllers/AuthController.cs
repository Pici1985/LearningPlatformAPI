using LearningPlatformAPI.ActionFilters;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> PostPerson([FromBody] Person person)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'DataContext.Person'  is null.");
            }
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            return Ok(new { UserID = person.UserId });
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var person = _context.CheckCredentials(loginModel.Email, loginModel.Password);

            if (person != null)
            {
                //generate token 
                Guid token = Guid.NewGuid();
                                                
                // get the user from the db with the passed in credentials
                // save token against a user in db
                person.Token = token;                
                _context.Person.Update(person);
                await _context.SaveChangesAsync();

                // this another way to build a string
                //var sb = new StringBuilder();
                //    sb.Append("hello");
                //    sb.Append(person.FirstName);
                //    sb.Append("Login successful! Token created:");
                //    sb.Append(token);

                var loginSuccess = new LoginSuccess() 
                { 
                    FirstName = person.FirstName,
                    Token = token
                };

                //return Ok($"Hello {person.FirstName} Login successful! Token created: {token}");
                return Ok(loginSuccess);

            }

            //with bool
            //if (_context.CheckCredentials(email, password))
            //{           
            //    return Ok($"Login successful! With {email} and {password}");
            //}

            if (loginModel.Email != "email")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");

            }

            if (loginModel.Password != "password")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");
            }

            return BadRequest();
        }
    }
}
