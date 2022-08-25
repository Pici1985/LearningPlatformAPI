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

        // TEST endpoint to check if db connection works

        //[HttpGet]
        //[Route("events")]
        //public async Task<ActionResult<UserTriggeredEvent>> GetEvents()
        //{
        //    var events = await _context.UserTriggeredEvent.ToListAsync();

        //    if (events != null)
        //    {
        //        return Ok(events);
        //    }
        //    return BadRequest();
        //}


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

                // this bit is redundant------------------------------------------

                //PersonLoggedInOnDate passin = new PersonLoggedInOnDate()
                //{
                //    UserID = person.UserId,
                //    DateTime = date
                //};
                
                // this bit is redundant------------------------------------------

                UserTriggeredEvent passIn = new UserTriggeredEvent()
                {
                    UserID = person.UserId,
                    EventID = 1,
                    TimeStamp = date,
                    Detail = null
                 };

                // get the user from the db with the passed in credentials
                // save token against a user in db
                person.Token = token;                
                _context.Person.Update(person);
                //_context.PersonLoggedInOnDate.Add(passin);
                _context.UserTriggeredEvent.Add(passIn);

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
                
                //------------------------------------- this is to calculate the CurrentStreak -----------------------------//

                //get distinct dates from db 
                var dates = (from d in _context.UserTriggeredEvent
                             where d.UserID == person.UserId
                            orderby d.TimeStamp ascending
                            select d.TimeStamp.Date).Distinct().ToList();
                
                // create a list of the actual dates logged in                                              
                List<DateTime> actualDates = new List<DateTime>(){ };

                foreach (var row in dates)
                {
                    actualDates.Add(row);
                    //Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(row);
                }              

                // get first and last login date
                var start = actualDates.Min();
                var end = actualDates.Max();

                // function to create a list with all available dates between first and last date
                IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
                {
                    if (endDate < startDate)
                        throw new ArgumentException("endDate must be greater than or equal to startDate");

                    while (startDate <= endDate)
                    {
                        yield return startDate;
                        startDate = startDate.AddDays(1);
                    }
                }

                // function called 
                var allDates = GetDateRange(start, end);

                var listOfDates = allDates.ToList();               
               
                // LINQ to figure out differences between actual dates and all possible dates
                var joinedEnum = from a in allDates
                                 join ad in actualDates
                                 on a equals ad into eGroup
                                 from ad in eGroup.DefaultIfEmpty()
                                 select ad;

                var joinedList = joinedEnum.ToList();             

                joinedList.Reverse();
                                         
                var currentStreak = 1;

                // check if the difference between date and following date is -1 
                for (var i = 0; i < (joinedList.Count - 1); i++)
                {                   
                    var lastDate = joinedList[i];
                    var nextDate = joinedList[i + 1];

                    var difference = (nextDate - lastDate).Days;
                                      
                    if (difference == -1)
                    {
                        currentStreak++;
                    }
                    else
                    {
                        break;
                    }

                }

                return Ok($"Successful login: {loginSuccess.FirstName}! Current streak: {currentStreak}");
            }
                // Calculate Current streak ------------------- useful until here ----------------------------------------------------



                //----------------------------------- this is to calcualte the LongestStreak ------------------------------------//
                //if (currentStreak > maxStreak)
                //{
                //    maxStreak = currentStreak;
                //}           

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
                //static int Streak(List<int> binaryList)
                //{
                //    int currentStreak = 0;
                //    int maxStreak = 0;

                //    foreach (var m in binaryList) 
                //    {
                //        if (m.Equals(1))
                //        {
                //            currentStreak++;
                //        }
                //        else
                //        {
                //            currentStreak = 0;
                //        }
                //    };

                //    return maxStreak;
                //}

                //----------------------------------- this is to calcualte the LongestStreak ------------------------------------//
                        
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
