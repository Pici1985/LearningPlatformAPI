using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyCoursesController : ControllerBase
    {
        private readonly DataContext _context;

        public MyCoursesController(DataContext context)
        {
            _context = context;
        }

        // Endpoints
        [HttpGet("{userid}")]
        public async Task<ActionResult<MyCourses>> GetMyCourses(int userid)
        {
            
            var courses = _context.GetMyCourses(userid);

            if (courses.Count == 0) 
            {
                return BadRequest(new { Message = "User doesn't exist or hasn't enrolled on any courses!" });
            }
            return Ok(courses);
        }
    }
}
