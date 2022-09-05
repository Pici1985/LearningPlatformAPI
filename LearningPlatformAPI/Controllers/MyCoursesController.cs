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
       
        // GET api/<MyCoursesController>/5
        [HttpGet("{userid}")]
        public async Task<ActionResult<MyCourses>> GetMyCourses(int userid)
        {

            var courses = (from mycourses in _context.MyCourses
                           join allcourses in _context.AllCourses on mycourses.CourseID equals allcourses.CourseId
                           where mycourses.UserID == userid
                           select new MyEnrolledCourses
                           {
                               CourseTitle = allcourses.CourseTitle,
                               Sections = (from coursesection in _context.CourseSection
                                           join allsections in _context.AllSections on coursesection.SectionId equals allsections.SectionID
                                           where coursesection.CourseId == allcourses.CourseId
                                           select new StartedSection
                                           {
                                               SectionID = allsections.SectionID,
                                               Started = _context.UserTriggeredEvent.Where(x=> x.UserID == userid && x.EventID == 
                                                        (int)EventsEnum.StartSection && x.Detail == coursesection.Id).FirstOrDefault() != 
                                                        null ? _context.UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.StartSection && x.Detail == coursesection.Id).FirstOrDefault().TimeStamp : null,
                                               Finished = _context.UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID == 
                                                        (int)EventsEnum.FinishSection && x.Detail == coursesection.Id).FirstOrDefault() != 
                                                        null ? _context.UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.FinishSection && x.Detail == coursesection.Id).FirstOrDefault().TimeStamp : null,
                                           }).ToList()
                           }).ToList();

            foreach( MyEnrolledCourses mec in courses)
            {
                var courseStarted = mec.Sections.Where(x => x.Started != null).Min(x => x.Started);
                mec.CourseStarted = courseStarted != null ? courseStarted : null;
                double totalsections = mec.Sections.Count();          
                 
                var courseFinished = mec.Sections.Where(x => x.Finished != null).Max(x => x.Finished);
                double finishedsections = mec.Sections.Where(x => x.Finished != null).Count();
                           
                // figure out this bit 
                var progress = (int)((finishedsections / totalsections)*100);
                                
                mec.CourseFinished = progress == 100 ? courseFinished : null;
                mec.CourseProgress = progress;
            }

            return Ok(courses);
        }
    }
}
