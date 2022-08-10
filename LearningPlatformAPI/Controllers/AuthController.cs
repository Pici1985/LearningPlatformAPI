using LearningPlatformAPI.ActionFilters;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningPlatformAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> PostPerson([FromBody] Person person)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'DataContext.Person'  is null.");
            }

            DateTime date = DateTime.Now;
            person.DateOfRegistration = date;

            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            return Ok(new { UserID = person.UserId });
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var person = _context.CheckCredentials(loginModel.Email, loginModel.Password); 

            if (person != null)
            {
                //generate token 
                Guid token = Guid.NewGuid();

                DateTime date = DateTime.Now;

                PersonLoggedInOnDate passin = new PersonLoggedInOnDate()
                {
                    UserID = person.UserId,
                    DateTime = date
                };

                // get the user from the db with the passed in credentials
                // save token against a user in db
                person.Token = token;                
                _context.Person.Update(person);
                _context.PersonLoggedInOnDate.Add(passin);

                await _context.SaveChangesAsync();       

                //this could save the timestamp as well

                // this another way to build a string
                //var sb = new StringBuilder();
                //    sb.Append("hello");
                //    sb.Append(person.FirstName);
                //    sb.Append("Login successful! Token created:");
                //    sb.Append(token);

                var loginSuccess = new LoginSuccess() 
                { 
                    FirstName = person.FirstName,
                    Token = token
                };
                
                //------------------------------------- this is to calculate the streak -----------------------------//

                //get dates from db 
                var dates = (from d in _context.PersonLoggedInOnDate
                            where d.UserID == person.UserId
                            orderby d.DateTime ascending
                            select d.DateTime.Date).Distinct().ToList();

                //get number of days in current month for iteration
                var ThisMonth = DateTime.Now.Month;
                var ThisYear = DateTime.Now.Year;
                int daysInMonth = DateTime.DaysInMonth(ThisYear, ThisMonth);
                Console.WriteLine(daysInMonth);
                Console.WriteLine();

                // instead of days in month need nr of days registered
                //var today = DateTime.Now;
                //var dateOfFirstRegistration = dates.Min();
                //int daysRegistered = (int)Math.Floor((today - dateOfFirstRegistration).TotalDays);
                //Console.WriteLine(daysRegistered);

                //create lists for storing stuff
                List<int> list = new List<int>(){ };
                List<int> binaryList = new List<int>(){ };


                //this is not quite right (this is actually a big fuckup)
                foreach (var row in dates)
                {
                    list.Add(row.Day);
                    Console.WriteLine(row.Day);
                }
                    Console.WriteLine();

                //this is not quite right (this is actually a big fuckup)


                // create a list of 1s and 0s
                for (var k = 1; k < daysInMonth; ++k) {
                    if (list.Contains(k))
                    {
                        binaryList.Add(1);
                    }
                    else 
                    {
                        binaryList.Add(0);
                    }
                }

                //binaryList.Reverse();

                foreach (var i in binaryList)
                {
                    Console.WriteLine(i);
                }

                // this formula IS not right
                //static int Streak(List<int> binaryList)
                //{
                //    int max = 0;

                //    for (int i = 0; i < binaryList.Count; i++)
                //    {
                //        int count = 0;
                //        for (int j = i; j < binaryList.Count; j++)
                //        {
                //            if (binaryList[i] == binaryList[j]) count++;
                //            if (count > max) max = count;
                //            if (binaryList[i] != binaryList[j]) break;
                //        }
                //    }
                //    return max;
                //}

                // this formula should be made right :)
                static int Streak(List<int> binaryList)
                {
                    int currentStreak = 0;
                    int maxStreak = 0;

                    foreach (var m in binaryList) 
                    {
                        if (m.Equals(1))
                        {
                            currentStreak++;
                        }
                        else
                        {
                            currentStreak = 0;
                        }
                        if (currentStreak > maxStreak)
                        {
                            maxStreak = currentStreak;
                        }
                    };

                    return maxStreak;
                    //return currentStreak;
                }

                int longestStreak = Streak(binaryList);

                //----------------------------------- this is to calcualte the streak ------------------------------------//


                return Ok($"Successful login: {loginSuccess.FirstName}! Your longest streak is {longestStreak} ");

            }
                        
            if (loginModel.Email != "email")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");

            }

            if (loginModel.Password != "password")
            {
                return BadRequest("Login failed!");
                //return BadRequest("Login failed!");
            }

            return BadRequest();
        }
    }
}
