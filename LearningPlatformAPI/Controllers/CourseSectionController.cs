using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSectionController : ControllerBase
    {
        private readonly DataContext _context;

        public CourseSectionController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> Start([FromBody]CreateStartRequest request)
        {
            // check if userid exists
            if (_context.Person.Any(i => i.UserId == request.UserId))
            {
                // check if CourseSectionId exists
                if (_context.CourseSection.Any(c => c.Id == request.CourseSectionId))
                {
                    // Query
                    var someEvent = (from e in _context.UserTriggeredEvent
                                    where e.UserID == request.UserId && e.EventID == 3 && e.Detail == $"CourseSectionID = {request.CourseSectionId}"
                                    select e).FirstOrDefault();

                    // check if event has happened already
                    if (someEvent == null) 
                    {
                        //create event
                        var newStart = new UserTriggeredEvent
                        {
                            UserID = request.UserId,
                            EventID = 3,
                            TimeStamp = DateTime.Now,
                            Detail = $"CourseSectionID = {request.CourseSectionId}"
                        };

                        _context.UserTriggeredEvent.Add(newStart);
                        await _context.SaveChangesAsync();

                        return Ok("Event created");
                    }
                    return BadRequest("Section already started!!");
                }
                return BadRequest("CourseSectionId doesn't exist!!");  
            }            
            // create event if it is not there
            return BadRequest("User doesn't exist!");
        }

        [HttpPost]
        [Route("finish")]
        public async Task<IActionResult> Finish([FromBody] CreateStartRequest request)
        {
            // check if user exists
            if (_context.Person.Any(i => i.UserId == request.UserId))
            {
                // check if coursesectionid exists
                if (_context.CourseSection.Any(c => c.Id == request.CourseSectionId))
                {

                    var doesEventExist = (from d in _context.UserTriggeredEvent
                                          where d.UserID == request.UserId &&
                                                d.EventID == 3 &&
                                                d.Detail == $"CourseSectionID = {request.CourseSectionId}"
                                          select d).FirstOrDefault();

                    // check if event started
                    if (doesEventExist != null)
                    {
                        var hasEventBeenFinished = (from d in _context.UserTriggeredEvent
                                              where d.UserID == request.UserId &&
                                                    d.EventID == 4 &&
                                                    d.Detail == $"CourseSectionID = {request.CourseSectionId}"
                                              select d).FirstOrDefault();

                        if (hasEventBeenFinished == null) 
                        { 
                            // create finished event 
                            var newFinish = new UserTriggeredEvent
                            {
                                UserID = request.UserId,
                                EventID = 4,
                                TimeStamp = DateTime.Now,
                                Detail = $"CourseSectionID = {request.CourseSectionId}"
                            };

                            _context.UserTriggeredEvent.Add(newFinish);
                            await _context.SaveChangesAsync();

                            return Ok("Section finished!");
                        }                        
                        return BadRequest("Section already finished!");
                    }
                    return BadRequest("Section not started yet!");
                }
                return BadRequest("CourseSectionId doesn't exist!");
            }
            return BadRequest("User doesn't exist!");
        }
    }
}
