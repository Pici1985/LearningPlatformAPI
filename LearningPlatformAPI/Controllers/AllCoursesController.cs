﻿using LearningPlatformAPI.Models;
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

        // GET: api/<AllCoursesController>
        [HttpGet]
        public async Task<ActionResult<AllCourses>> GetAllCourses()
        {
            var courses = await _context.AllCourses.ToListAsync();

            if (courses != null)
            {
                return Ok(courses);
            }
            return BadRequest();
        }

        //my solution 

        //POST api/<AllCoursesController>
        //[HttpPost]
        //[Route("enroll/{id}")]
        //public async Task<ActionResult<AllCourses>> Post([FromRouteAttribute] int id, int CourseID)
        //{
        //    int courseid = CourseID;
        //    int userid = id;

        //    //query to check if an enrollment already exists
        //    var userEnrolledOnCourse = (from c in _context.MyCourses
        //                                where c.CourseID == courseid && c.UserID == userid
        //                                select c);


        //    //check if user exists
        //    if (_context.Person.Any(i => i.UserId == userid))
        //    {
        //        //check if course exists
        //        if (_context.AllCourses.Any(c => c.CourseId == courseid))
        //        {
        //            foreach (var course in userEnrolledOnCourse)
        //            {
        //                //check if enrollment already exists
        //                if (userEnrolledOnCourse != null)
        //                {
        //                    Console.WriteLine($"User: {course.UserID} already enrolled to Course: {course.ID}");
        //                    return Ok("already enrolled");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return Ok($"course: {courseid} not found");
        //        }
        //    }
        //    else
        //    {
        //        return Ok($"person: {userid} not found");
        //    }

        //    //create new enrollment with given user on given course
        //    MyCourses newCourse = new MyCourses
        //    {
        //        CourseID = courseid,
        //        UserID = userid,
        //        Progress = 0,
        //        Finished = false
        //    };
        //    _context.MyCourses.Add(newCourse);
        //    await _context.SaveChangesAsync();
        //    return Ok($"user: {userid} enrolled on course: {courseid}");
        //}

        //until here

        //refactored solution

       [HttpPost]
        [Route("enroll")]
        public async Task<IActionResult> Post([FromBody] CreateEnrollRequest request)
        {
            var validateResult = Validate(request);

            if (validateResult != null)
            {
                return validateResult;
            }
            //create new enrollment with given user on given course
            MyCourses newCourse = new MyCourses
            {
                CourseID = request.CourseId,
                UserID = request.UserId,
                Progress = 0,
                Finished = false
            };
            _context.MyCourses.Add(newCourse);
            await _context.SaveChangesAsync();
            return Ok($"user: {request.UserId} enrolled on course: {request.CourseId}");
        }

        private ObjectResult Validate(CreateEnrollRequest request)
        {
            //check if user exists
            if (_context.Person.Any(i => i.UserId == request.UserId))
            {
                //check if course exists
                if (_context.AllCourses.Any(c => c.CourseId == request.CourseId))
                {
                    var userEnrolledOnCourse = (from c in _context.MyCourses
                                                where c.CourseID == request.CourseId && c.UserID == request.UserId
                                                select c).Count();

                    //check if enrollment already exists
                    if (userEnrolledOnCourse > 0)
                    {
                        Console.WriteLine($"User: {request.UserId} already enrolled to Course: {request.CourseId}");
                        return Conflict("already enrolled");
                    }
                }
                else
                {
                    return BadRequest($"course: {request.CourseId} not found");
                }
            }
            else
            {
                return BadRequest($"person: {request.UserId} not found");
            }
            return null;
        }


        // this is from boilerplate

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
