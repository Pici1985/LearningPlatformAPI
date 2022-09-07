using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }


        // Entities to control DB tables
        public DbSet<Person> Person { get; set; }
        public DbSet<AllCourses> AllCourses { get; set; }
        public DbSet<MyCourses> MyCourses { get; set; }
        public DbSet<UserTriggeredEvent> UserTriggeredEvent { get; set; }
        public DbSet<AllSections> AllSections { get; set; }
        public DbSet<CourseSection> CourseSection { get; set; }

        // Method to control Login Functionality
        public Person? CheckCredentials(string email, string password)
        {
            return (from p in Person
                    where p.Email == email && p.Password == password
                    select p).FirstOrDefault();
        }

        // Methods for AllCoursesController
        public List<AllCourses> GetAllCourses()
        {
            var allCourses = AllCourses.ToList();

            return allCourses;
        }

        public bool DoesPersonExist(int userid) 
        {
            if (Person.Any(i => i.UserId == userid))
            { 
                return true;
            }
            return false;
        }

        public List<AllCourses> GetAvailableCourses(int userid) 
        {
            var allcourses = AllCourses.ToList();

            var allCoursesIds = (from a in allcourses
                                 select a.CourseId).ToList();

            var enrolledCoursesIds = (from m in MyCourses
                                      where m.UserID == userid
                                      select m.CourseID).ToList();

            var availableCourses = allCoursesIds.Except(enrolledCoursesIds).ToList();

            var courses = (from m in allcourses
                           join av in availableCourses on m.CourseId equals av
                           select m).ToList();

            return courses;
        }

        public void EnrollOnCourse(CreateEnrollRequest request) 
        {
            //create new enrollment with given user on given course
            MyCourses newCourse = new MyCourses
            {
                CourseID = request.CourseId,
                UserID = request.UserId,
            };

            //create enroll event
            var newEnroll = new UserTriggeredEvent
            {
                UserID = request.UserId,
                EventID = (int)EventsEnum.EnrollCourse,
                TimeStamp = DateTime.Now,
                Detail = request.CourseId
            };

            UserTriggeredEvent.Add(newEnroll);

            MyCourses.Add(newCourse);
            SaveChangesAsync();
        }

        public string Validate(CreateEnrollRequest request)
        {
            //check if user exists
            if (Person.Any(i => i.UserId == request.UserId))
            {
                //check if course exists
                if (AllCourses.Any(c => c.CourseId == request.CourseId))
                {
                    var userEnrolledOnCourse = (from c in MyCourses
                                                where c.CourseID == request.CourseId && c.UserID == request.UserId
                                                select c).Count();

                    //check if enrollment already exists
                    if (userEnrolledOnCourse > 0)
                    {
                        Console.WriteLine($"User: {request.UserId} already enrolled to Course: {request.CourseId}");
                        return "already enrolled";
                    }
                }
                else
                {
                    return $"course: {request.CourseId} not found";
                }
            }
            else
            {
                return $"person: {request.UserId} not found";
            }
            return null;
        }


        // Methods for LeaderBoardController
        public UsersNrOfLogins GetUsersNrOfLogins() 
        {
            var getLeadersGroup = (from l in UserTriggeredEvent
                                   where l.UserID > 0 && l.EventID == 1
                                   group l by l.UserID into newGroup
                                   select new NrOfLoginTimes
                                   {
                                       UserID = newGroup.Key,
                                       FirstName = (from f in Person
                                                    where f.UserId == newGroup.Key
                                                    select f.FirstName).FirstOrDefault(),
                                       LastName = (from f in Person
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

            return response;
        }

        public FastestFinishedCourses GetFastestFinishedCourses()
        {
            var table = UserTriggeredEvent.ToList();

            var leaders = new List<UserFinishedCourseIn>() { };

            var finishedCourseIds = new List<int>() { };

            foreach (var i in table)
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

            return result;
        }

        public LeadersOfLongestConsecutiveStreak GetLeadersOfLongestConsecutiveStreak() 
        {
            //figure out how many users we have
            var allUserIds = (from a in UserTriggeredEvent
                              where a.EventID == 1
                              select a.UserID).Distinct().ToList();


            //calculate longest streak for each user

            var dataTable = UserTriggeredEvent.ToList();

            var firstActualDateLoggedIn = new DateTime() { };
            var lastActualDateLoggedIn = new DateTime() { };

            var leaders = new List<LongestConsecutiveStreak>() { };

            foreach (var user in allUserIds)
            {
                // getting all the dates a user actually logged in
                var allActualDatesLoggedIn = (from d in dataTable
                                              where d.UserID == user
                                              orderby d.TimeStamp ascending
                                              select d.TimeStamp.Date).Distinct().ToList();

                //getting first and last date
                firstActualDateLoggedIn = allActualDatesLoggedIn.Min();
                lastActualDateLoggedIn = allActualDatesLoggedIn.Max();

                //cal lfunction to create a list of all possible dates  
                var allDates = GetDateRange(firstActualDateLoggedIn, lastActualDateLoggedIn);

                var allPossibleDates = allDates.ToList();

                // query to create a list with the difference of possible dates and actual dates                    
                var joinedEnum = from a in allPossibleDates
                                 join ad in allActualDatesLoggedIn
                                 on a equals ad into eGroup
                                 from ad in eGroup.DefaultIfEmpty()
                                 select ad;

                var joinedList = joinedEnum.ToList();
                joinedList.Reverse();

                // check the streaks 
                var longestStreak = 1;
                var currentStreak = 1;

                for (var i = 0; i < (joinedList.Count - 1); i++)
                {
                    var lastDate = joinedList[i];
                    var nextDate = joinedList[i + 1];

                    var difference = (nextDate - lastDate).Days;

                    if (difference == -1)
                    {
                        currentStreak++;
                        if (currentStreak > longestStreak)
                        {
                            longestStreak = currentStreak;
                        }
                    }
                    else
                    {
                        currentStreak = 1;
                    }

                }

                // create an object for each user to be returned 
                var userLongestStreak = new LongestConsecutiveStreak()
                {
                    UserID = user,
                    longestStreak = longestStreak
                };

                leaders.Add(userLongestStreak);
            }

            // create list and order it by longestStreak
            var result = new LeadersOfLongestConsecutiveStreak()
            {
                Title = "LongestConsecutiveStreak",
                Leaders = leaders.OrderByDescending(x => x.longestStreak)
            };

            // helper function to create a list with all available dates between first and last date
            IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
            {
                if (lastActualDateLoggedIn < firstActualDateLoggedIn)
                    throw new ArgumentException("lastActualDateLoggedIn must be greater than or equal to firstActualDateLoggedIn");

                while (firstActualDateLoggedIn <= lastActualDateLoggedIn)
                {
                    yield return firstActualDateLoggedIn;
                    firstActualDateLoggedIn = firstActualDateLoggedIn.AddDays(1);
                }
            }

            return result;
        }

        public UsersNrOfFinishedCourses GetUsersNrOfFinishedCourses() 
        {
            var getLeadersByFinishedCoursesGroup = (from l in UserTriggeredEvent
                                                    where l.UserID > 0 && l.EventID == (int)EventsEnum.FinishCourse
                                                    group l by l.UserID into newGroup
                                                    select new NrOfFinishedCourses
                                                    {
                                                        UserID = newGroup.Key,
                                                        FirstName = (from f in Person
                                                                     where f.UserId == newGroup.Key
                                                                     select f.FirstName).FirstOrDefault(),
                                                        LastName = (from f in Person
                                                                    where f.UserId == newGroup.Key
                                                                    select f.LastName).FirstOrDefault(),
                                                        CoursesFinished = (from courseFinshedcount in newGroup
                                                                           where courseFinshedcount.UserID > 0 && courseFinshedcount.EventID == (int)EventsEnum.FinishCourse
                                                                           select courseFinshedcount).Count()
                                                    }).ToList();

            var getLeadersByFinishedCoursesGroupOrdered = getLeadersByFinishedCoursesGroup.OrderByDescending(l => l.CoursesFinished);

            var result = new UsersNrOfFinishedCourses()
            {
                Title = "NrOfFinishedCourses",
                Leaders = getLeadersByFinishedCoursesGroupOrdered
            };

            return result;
        }
    }
}
