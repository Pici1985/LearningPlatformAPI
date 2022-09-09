using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
        
        //Endpoints
        [HttpGet]
        [Route("{boardid}")]
        public async Task<IActionResult> GetLeaderBoards(int boardid)
        {

        // this if is for NrOfLoginTimes
            if (boardid == (int)LeaderBoardTypesEnum.NrOfLoginTimes) 
            {
                var result = _context.GetUsersNrOfLogins();

                return Ok(result);
            }

        // this if is for FastestCourseCompletionTime
            if (boardid == (int)LeaderBoardTypesEnum.FastestCourseCompletionTime)
            {
                var result = _context.GetFastestFinishedCourses();

                return Ok(result);
            }


        // this if is for LongestStreak
            if (boardid == (int)LeaderBoardTypesEnum.LongestConsecutiveStreak) 
            {
                var result = _context.GetLeadersOfLongestConsecutiveStreak();

                return Ok(result);
            }


        // this if is for NrOfFinishedCourses
            if (boardid == (int)LeaderBoardTypesEnum.NrOfFinishedCourses) 
            {
                var result = _context.GetUsersNrOfFinishedCourses();

                return Ok(result);
            }
            return BadRequest("LeaderBoard does not exist");
        }
    }
}
