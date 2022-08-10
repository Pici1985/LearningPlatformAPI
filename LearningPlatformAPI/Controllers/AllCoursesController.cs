using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllCoursesController : ControllerBase
    {
        private readonly DataContext _context;

        public AllCoursesController(DataContext context)
        {
            _context = context;
        }
        // GET: api/<AllCoursesController>
        [HttpGet]
        public async Task<ActionResult<AllCourses>> GetAllCourses()
        {
            //var mycourses = (from myc in _context.MyCourses
            //                 where myc.UserID == id
            //                 select myc);

            var courses = await _context.AllCourses.ToListAsync();

            if (courses == null)
            {
                return BadRequest();
            }
            return Ok(courses);
        }     

        // POST api/<AllCoursesController>
        [HttpPost]
        [Route("enroll/{id}")]
        public async Task<ActionResult<AllCourses>> Post([FromRouteAttribute] int id, int CourseID)
        {
            //await _context.AllCourses.ToListAsync();

            int courseid = CourseID;
            int userid = id;

            // add a new line to mycourses table with userid courseid 
            // check if course id exists
            // check if user id exists
            // if this combination already exists give back an error

            return Ok($" This User: {userid} wants to enroll in this course:{courseid}");
        }

        //// PUT api/<AllCoursesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AllCoursesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
