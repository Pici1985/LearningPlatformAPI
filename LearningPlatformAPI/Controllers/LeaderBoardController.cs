using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {

        // dependency injection
        private readonly DataContext _context;

        // constructor
        public LeaderBoardController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Route("{boardid}")]
        public async Task<IActionResult> GetLeaderBoards(int boardid)
        {

        // this if is for NrOfLoginTimes
            if (boardid == (int)LeaderBoardTypesEnum.NrOfLoginTimes) 
            {
                
                var getLeadersGroup = (from l in _context.UserTriggeredEvent
                                       where l.UserID > 0 && l.EventID == 1
                                       group l by l.UserID into newGroup
                                       select new NrOfLoginTimes
                                       {
                                           UserID = newGroup.Key,
                                           FirstName = (from f in _context.Person
                                                        where f.UserId == newGroup.Key
                                                        select f.FirstName).FirstOrDefault(),
                                           LastName = (from f in _context.Person
                                                       where f.UserId == newGroup.Key
                                                       select f.LastName).FirstOrDefault(),
                                           LoginTimes = (from logincount in newGroup
                                                         where logincount.UserID > 0 && logincount.EventID == 1
                                                         select logincount).Count()
                                       }).ToList();

                var leaderGroupOrdered = getLeadersGroup.OrderByDescending(l => l.LoginTimes);

                var response = new UsersNrOfLogins()
                {
                    Title = "NrOfLoginTimes",
                    Leaders = leaderGroupOrdered
                };

                return Ok(response);
            }

        // this if is for FastestCourseCompletionTime
            if (boardid == (int)LeaderBoardTypesEnum.FastestCourseCompletionTime)
            {

                var leaders = (from l in _context.UserTriggeredEvent
                               select new UserFinishedCourseIn
                               {
                                   UserID = l.UserID,
                                   CourseID = (from c in _context.CourseSection
                                               where c.SectionId == l.Detail
                                               select c.CourseId).FirstOrDefault(),
                                   // this is obviously not right :) 
                                   FinishedIn = (from f in _context.UserTriggeredEvent
                                                 join courseid in _context.CourseSection on f.Detail equals courseid.Id
                                                 where f.Detail == courseid.Id && f.EventID == (int)EventsEnum.FinishCourse
                                                 select f.TimeStamp).FirstOrDefault().Subtract((from f in _context.UserTriggeredEvent
                                                                                                join courseid in _context.CourseSection on f.Detail equals courseid.Id
                                                                                                where f.Detail == courseid.Id && f.EventID == (int)EventsEnum.StartCourse
                                                                                                select f.TimeStamp).FirstOrDefault()).ToString()
                               }
                               ).ToList();

                // dummy data
                //var leaders = new List <UserFinishedCourseIn>() 
                //{ };

                //var time1 = new TimeOnly(0, 14, 18).ToLongTimeString();
                //var time2 = new TimeOnly(0, 16, 18).ToLongTimeString();

                //var user1 = new UserFinishedCourseIn()
                //{
                //    UserID = 1,
                //    CourseID = 1,
                //    FinishedIn = time1
                //};
                
                //var user2 = new UserFinishedCourseIn()
                //{
                //    UserID = 2,
                //    CourseID = 1,
                //    FinishedIn = time2
                //};
                
                //leaders.Add(user1);
                //leaders.Add(user2);
                // until here 


                var result = new FastestFinishedCourses()
                {
                    Title = "FastestCourseCompletionTime",
                    Leaders = leaders
                };

                return Ok(result);
            }


        // this if is for LongestStreak
            if (boardid == (int)LeaderBoardTypesEnum.LongestConsecutiveStreak) 
            {
                return Ok("LongestConsecutiveStreak");
            }


        // this if is for NrOfFinishedCourses
            if (boardid == (int)LeaderBoardTypesEnum.NrOfFinishedCourses) 
            {
                var getLeadersByFinishedCoursesGroup = (from l in _context.UserTriggeredEvent
                                                       where l.UserID > 0 && l.EventID == (int)EventsEnum.FinishCourse
                                                       group l by l.UserID into newGroup
                                                       select new NrOfFinishedCourses
                                                       {
                                                           UserID = newGroup.Key,
                                                           FirstName = (from f in _context.Person
                                                                        where f.UserId == newGroup.Key
                                                                        select f.FirstName).FirstOrDefault(),
                                                           LastName = (from f in _context.Person
                                                                       where f.UserId == newGroup.Key
                                                                       select f.LastName).FirstOrDefault(),
                                                           CoursesFinished = (from courseFinshedcount in newGroup
                                                                            where courseFinshedcount.UserID > 0 && courseFinshedcount.EventID == (int)EventsEnum.FinishCourse
                                                                              select courseFinshedcount).Count()
                                                       }).ToList();

                var getLeadersByFinishedCoursesGroupOrdered = getLeadersByFinishedCoursesGroup.OrderByDescending(l => l.CoursesFinished);

                var response = new UsersNrOfFinishedCourses()
                {
                    Title = "NrOfFinishedCourses",
                    Leaders = getLeadersByFinishedCoursesGroupOrdered
                };

                return Ok(response);
            }
            return BadRequest("LeaderBoard does not exist");
        }
    }
}
