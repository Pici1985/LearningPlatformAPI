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
            // this may not be right at all
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

            if (courses != null)
            {
                return Ok(courses);
            }
            return BadRequest();
        }     

        // POST api/<AllCoursesController>
        [HttpPost]
        [Route("enroll/{id}")]
        public async Task<ActionResult<AllCourses>> Post([FromRouteAttribute] int id, int CourseID)
        {
            //await _context.AllCourses.ToListAsync();

            int courseid = CourseID;
            int userid = id;

            //query to check if a course exists
            var courseExists = (from c in _context.AllCourses
                               where c.CourseId == courseid 
                               select c);

            // this is the new branch

            //query to check if user exists
            //var userExists = (from u in _context.Person
            //                 where u.UserId == userid
            //                 select new { u });

            // check if course id exists 
            foreach (var course in courseExists)
            {
                if (course != null)
                {
                    // check if user id exists      
                    //foreach (var user in userExists)
                    //{
                    //    if (user != null) 
                    //    {
                    //        return BadRequest();
                    //    }
                    //}
                    //Console.WriteLine($"{course.CourseTitle} exists {user.FirstName}{user.LastName} exists");
                    Console.WriteLine($"{course.CourseTitle} exists");
                    return Ok(course.CourseTitle);   
                }
                return BadRequest();
            }
            return BadRequest();

            // check if user is already enrolled to course
            // add a new line to mycourses table with userid courseid 
            // if this combination already exists give back an error

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
