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

            if (boardid == (int)LeaderBoardTypesEnum.NrOfLoginTimes) 
            {
                
                var getLeadersGroup = (from l in _context.UserTriggeredEvent
                                       where l.UserID > 0 && l.EventID == 1
                                       group l by l.UserID into newGroup
                                       select new NrOfLoginTimes
                                       {
                                           UserID = newGroup.Key,
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

            if (boardid == (int)LeaderBoardTypesEnum.FastestCourseCompletionTime)
            {
                return Ok("FastestCourseCompletionTime");
            }
            
            if (boardid == (int)LeaderBoardTypesEnum.LongestConsecutiveStreak) 
            {
                return Ok("LongestConsecutiveStreak");
            }
            
            if (boardid == (int)LeaderBoardTypesEnum.NrOfFinishedCourses) 
            {
                return Ok("NrOfFinishedCourses");
            }
            return BadRequest("LeaderBoard does not exist");
        }
    }
}
