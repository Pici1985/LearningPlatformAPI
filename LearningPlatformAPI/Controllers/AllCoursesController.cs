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
            int courseid = CourseID;
            int userid = id;

            //query to check if an enrollment already exists
            var userEnrolledOnCourse = (from c in _context.MyCourses
                                        where c.CourseID == courseid && c.UserID == userid
                                        select c);

            //check if user exists
            if (_context.Person.Any(i => i.UserId == userid))
            {
                //check if course exists
                if (_context.AllCourses.Any(c => c.CourseId == courseid))
                {
                    foreach (var course in userEnrolledOnCourse)
                    {   
                        //check if enrollment already exists
                        if (userEnrolledOnCourse != null)
                        {
                            Console.WriteLine($"User: {course.UserID} already enrolled to Course: {course.ID}");
                            return Ok("already enrolled");
                        }
                    }
                }
                else 
                { 
                    return Ok($"course: {courseid} not found");                
                }
            }
            else 
            { 
                return Ok($"person: {userid} not found");            
            }

            //creat new enrollment with given user on given course
            MyCourses newCourse = new MyCourses
            {
                CourseID = courseid,
                UserID = userid,
                Progress = 0,
                Finished = false
            };
            _context.MyCourses.Add(newCourse);
            await _context.SaveChangesAsync();
            return Ok($"user: {userid} enrolled on course: {courseid}");
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
