using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Enums;
using System.Linq;

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
        public async Task<IActionResult> Start([FromBody]CreateSectionRequest request)
        {
            // check if userid exists
            if (_context.Person.Any(i => i.UserId == request.UserId))
            {
                // check if CourseSectionId exists
                if (_context.CourseSection.Any(c => c.Id == request.CourseSectionId))
                {
                    var currentCourseId = (from c in _context.CourseSection
                                           where c.Id == request.CourseSectionId
                                           select c.CourseId
                                          ).FirstOrDefault();

                    var userEnrolledOnCourse = (from u in _context.MyCourses
                                                where u.CourseID == currentCourseId
                                                select u).FirstOrDefault();

                    // check if user enrolled on course
                    if (userEnrolledOnCourse != null)
                    {
                        var someEvent = (from e in _context.UserTriggeredEvent
                                        where e.UserID == request.UserId && e.EventID == (int)EventsEnum.StartSection && e.Detail == request.CourseSectionId
                                        select e).FirstOrDefault();

                        // check if event has happened already
                        if (someEvent == null) 
                        {
                            //create event
                            var newStart = new UserTriggeredEvent
                            {
                                UserID = request.UserId,
                                EventID = (int)EventsEnum.StartSection,
                                TimeStamp = DateTime.Now,
                                Detail = request.CourseSectionId
                            };

                            var currentCourseSectionIds = (from c in _context.CourseSection
                                                           where c.CourseId == currentCourseId
                                                           select c.Id).ToList();

                            // check if section start event is a course start event as well
                            var isCourseStartEvent = (from i in _context.UserTriggeredEvent
                                                      where i.UserID == request.UserId && 
                                                            i.EventID == (int)EventsEnum.StartSection &&
                                                            currentCourseSectionIds.Contains(i.Detail)
                                                      select i).FirstOrDefault();                         

                            var result = "Event created";

                            if (isCourseStartEvent == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("This is a course start event! ");

                                var newCourseStartEvent = new UserTriggeredEvent
                                {
                                    UserID = request.UserId,
                                    EventID = (int)EventsEnum.StartCourse,
                                    TimeStamp = DateTime.Now,
                                    Detail = currentCourseId
                                };

                                _context.UserTriggeredEvent.Add(newCourseStartEvent);

                                result = "Event created and Course Started!!";
                            }

                            _context.UserTriggeredEvent.Add(newStart);
                            await _context.SaveChangesAsync();

                            return Ok(result);
                        }
                        return BadRequest("Section already started!!");
                    }
                    return BadRequest("User isn't enrolled on course!!");
                }
                return BadRequest("CourseSectionId doesn't exist!!");  
            }            
            return BadRequest("User doesn't exist!");

        }

        [HttpPost]
        [Route("finish")]
        public async Task<IActionResult> Finish([FromBody] CreateSectionRequest request)
        {
            // check if user exists
            if (_context.Person.Any(i => i.UserId == request.UserId))
            {
                // check if coursesectionid exists
                if (_context.CourseSection.Any(c => c.Id == request.CourseSectionId))
                {

                    var doesEventExist = (from d in _context.UserTriggeredEvent
                                          where d.UserID == request.UserId &&
                                                d.EventID == (int)EventsEnum.StartSection &&
                                                d.Detail == request.CourseSectionId
                                          select d).FirstOrDefault();
                                      
                    // check if event started
                    if (doesEventExist != null)
                    {
                        var hasEventBeenFinished = (from d in _context.UserTriggeredEvent
                                                    where d.UserID == request.UserId &&
                                                          d.EventID == (int)EventsEnum.FinishSection &&
                                                          d.Detail == request.CourseSectionId
                                                    select d).FirstOrDefault();

                        if (hasEventBeenFinished == null) 
                        { 
                            var currentCourseId = (from c in _context.CourseSection
                                                   where c.Id == request.CourseSectionId
                                                   select c.CourseId).FirstOrDefault();

                            // create finished event 
                            var newFinish = new UserTriggeredEvent
                            {
                                UserID = request.UserId,
                                EventID = (int)EventsEnum.FinishSection,
                                TimeStamp = DateTime.Now,
                                Detail = request.CourseSectionId
                            };

                            // check if a course has been finished with this finishevent


                            var currentCourseSectionIds = (from c in _context.CourseSection
                                                           where c.CourseId == currentCourseId
                                                           select c.Id).ToList();

                            var totalNrOfSectionsInCurrentCourse = (from t in _context.CourseSection
                                                                    where t.CourseId == currentCourseId
                                                                    select t).Count();
                                                     
                            var finishedNrOfSectionsInCurrentCourse = (from f in _context.UserTriggeredEvent
                                                                       where f.UserID == request.UserId &&
                                                                             f.EventID == (int)EventsEnum.FinishSection &&
                                                                             currentCourseSectionIds.Contains(f.Detail)    
                                                                       select f
                                                                       ).Count();
                                                        
                            bool hasCurrentCourseBeenFinished = totalNrOfSectionsInCurrentCourse == (finishedNrOfSectionsInCurrentCourse + 1);

                            var resultText = "Section Finished";

                            // until here

                            if (hasCurrentCourseBeenFinished)
                            {
                                var newCourseFinish = new UserTriggeredEvent
                                {
                                    UserID = request.UserId,
                                    EventID = (int)EventsEnum.FinishCourse,
                                    TimeStamp = DateTime.Now,
                                    Detail = currentCourseId
                                };

                                _context.UserTriggeredEvent.Add(newCourseFinish);
                                
                                resultText = "Section and Course Finished";
                            }
                            
                            _context.UserTriggeredEvent.Add(newFinish);
                            await _context.SaveChangesAsync();

                            return Ok(resultText);
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
