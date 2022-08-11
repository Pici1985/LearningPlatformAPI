using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;

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
       
        // GET api/<MyCoursesController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MyCourses>> GetMyCourses(int id)
        {
            var mycourses = (from myc in _context.MyCourses
                             where myc.UserID == id
                             select myc);

            foreach (var course in mycourses)
            {
                //Console.WriteLine(course.UserID);
                if (course.CourseID == id)
                {
                    return Ok(mycourses);
                }
            }                     
            return BadRequest();
        }

        //// POST api/<MyCoursesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<MyCoursesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<MyCoursesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
