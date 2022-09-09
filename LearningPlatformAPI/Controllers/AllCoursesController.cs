using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllCoursesController : ControllerBase
    {
        // dependency injection
        private readonly DataContext _context;
        
        // constructor
        public AllCoursesController(DataContext context)
        {
            _context = context;
        }

        // endpoints
        [HttpGet]
        public async Task<ActionResult<AllCourses>> GetAllCourses()
        {
            var courses = _context.GetAllCourses();

            if (courses != null)
            {
                return Ok(courses);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("available/{userid}")]
        public async Task<IActionResult> GetAvailableCourses(int userid)
        {
            // need to check if userid exists
            var personExists = _context.DoesPersonExist(userid);

            if (personExists)
            {
                var availableCourses = _context.GetAvailableCourses(userid);

                return Ok(availableCourses);
            }
            return BadRequest($"UserID: {userid} doesn't exist!!");
        }

        [HttpPost]
        [Route("enroll")]
        public async Task<ActionResult<EnrollValidateResponse>> Post([FromBody] CreateEnrollRequest request)
        {
            var validateResult = _context.Validate(request);

            if (validateResult != null)
            {
                if (validateResult.IsValidated == false)
                {
                    return BadRequest(validateResult);
                }
                return BadRequest(validateResult);
            }

            validateResult = new EnrollValidateResponse()
            {
                IsValidated = true,
                Message = $" User: {request.UserId} enrolled on Course: {request.CourseId}"
            };
            
            _context.EnrollOnCourse(request);
            return Ok(validateResult);
        }
    }
}
