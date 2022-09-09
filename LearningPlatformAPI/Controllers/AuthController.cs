using LearningPlatformAPI.ActionFilters;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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

        // Endpoints
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> PostPerson([FromBody] Person person)
        {
            if(string.IsNullOrWhiteSpace(person.FirstName) && 
               string.IsNullOrWhiteSpace(person.LastName) && 
               string.IsNullOrWhiteSpace(person.Email) &&  
               string.IsNullOrWhiteSpace(person.Password)
               ) 
            {
                var newPerson = _context.registerPerson(person);
                return Ok($"Person registered: {newPerson.FirstName}, {newPerson.LastName}");
            }           
            return BadRequest(new { Message = "Field cannot be empty!!"});   
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var person = _context.CheckCredentials(loginModel.Email, loginModel.Password);

            if (person != null)
            { 
                var loginSuccess = await _context.personLoginFunction(person);

                var currentStreak = await _context.currentStreakCounter(person);

                return Ok($"Successful login: {loginSuccess.FirstName}! Current streak: {currentStreak}");

            }    
            return BadRequest();                               
        }
    }
}
