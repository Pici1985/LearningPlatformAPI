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
                var table = _context.UserTriggeredEvent.ToList();

                var leaders = new List<UserFinishedCourseIn>() { };

                var finishedCourseIds = new List<int>() { };

                foreach(var i in table) 
                {
                    if (i.EventID == (int)EventsEnum.FinishCourse)
                    {
                        finishedCourseIds.Add(i.Detail);
                    }
                }
                           
                foreach (var f in finishedCourseIds) 
                {
                    var userfinishedcoursein = new UserFinishedCourseIn() { };

                    var userid = (from x in table
                                  where x.Detail == f
                                  select x.UserID).FirstOrDefault();
                    var courseid = f;
                    var finishedin = ((from x in table
                                       where x.Detail == f && x.EventID == (int)EventsEnum.FinishCourse
                                       select x.TimeStamp).FirstOrDefault()).Subtract((from y in table
                                                                                       where y.Detail == f && y.EventID == (int)EventsEnum.StartCourse
                                                                                       select y.TimeStamp).FirstOrDefault());
                                       
                    userfinishedcoursein.UserID = userid;
                    userfinishedcoursein.CourseID = courseid;
                    userfinishedcoursein.FinishedIn = finishedin;   

                    leaders.Add(userfinishedcoursein);
                }                   
           
                var result = new FastestFinishedCourses()
                {
                    Title = "FastestCourseCompletionTime",
                    Leaders = leaders.OrderByDescending(x => x.FinishedIn).ToList()
                };

                return Ok(result);
            }


        // this if is for LongestStreak
            if (boardid == (int)LeaderBoardTypesEnum.LongestConsecutiveStreak) 
            {
                
                
                var leaders = new List<LongestConsecutiveStreak>() { };          
                
                var result = new LeadersOfLongestConsecutiveStreak()
                {
                    Title = "LongestConsecutiveStreak",
                    Leaders = leaders.OrderByDescending(x => x.Streak)
                };

                return Ok(result);
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
