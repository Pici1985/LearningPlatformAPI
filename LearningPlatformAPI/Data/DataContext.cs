using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System;

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

        public EnrollValidateResponse Validate(CreateEnrollRequest request)
        {
            var result = new EnrollValidateResponse() { };
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
                        //Console.WriteLine($"User: {request.UserId} already enrolled to Course: {request.CourseId}");
                        result.IsValidated = false;
                        result.Message = "already enrolled";
                        return result;
                    }
                }
                else
                {
                    result.IsValidated = false;
                    result.Message = $"course: {request.CourseId} not found";
                    return result;
                    //return $"course: {request.CourseId} not found";
                }
            }
            else
            {
                result.IsValidated = false;
                result.Message = $"user: {request.UserId} not found";
                return result;
                //return $"person: {request.UserId} not found";
            }
            return null;
        }


        //Methods for AuthController
        public Person registerPerson(Person person)
        {
            var dateOfRegistration = DateTime.Now;          
             
            var newPerson = new Person() 
            {
                FirstName = person.FirstName, 
                LastName = person.LastName,
                Email = person.Email,
                Password = person.Password,
                Gender = person.Gender,
                Age = person.Age,
                Occupation = person.Occupation,
                DateOfRegistration = dateOfRegistration,
                Token = person.Token,
            };
            
            Person.Add(newPerson);
            SaveChanges();

            return person;
        }
                
        public Person? CheckCredentials(string email, string password)
        {
            return (from p in Person
                    where p.Email == email && p.Password == password
                    select p).FirstOrDefault();
        }

        public async Task<LoginSuccess> personLoginFunction(Person person) 
        {
            //generate token 
            Guid token = Guid.NewGuid();

            DateTime date = DateTime.Now;

            UserTriggeredEvent passIn = new UserTriggeredEvent()
            {
                UserID = person.UserId,
                EventID = 1,
                TimeStamp = date,
                Detail = 0
            };

            // get the user from the db with the passed in credentials

            // save token against a user in db
            person.Token = token;
            Person.Update(person);
            UserTriggeredEvent.Add(passIn);
            await SaveChangesAsync();

            var loginSuccess = new LoginSuccess()
            {
                FirstName = person.FirstName,
                Token = token
            };

            return loginSuccess;
        }

        public async Task<int> currentStreakCounter(Person person) 
        {
            //get distinct dates from db 
            var dates = (from d in UserTriggeredEvent
                         where d.UserID == person.UserId && d.EventID == 1
                         orderby d.TimeStamp ascending
                         select d.TimeStamp.Date).Distinct().ToList();

            // create a list of the actual dates logged in                                              
            List<DateTime> actualDates = new List<DateTime>() { };

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
            return currentStreak;
        }


        // Methods for CourseSectionController
        public StartSectionByUser startSectionByUser(CreateSectionRequest request) 
        {
            var result = new StartSectionByUser() { };
            // check if userid exists
            if (Person.Any(i => i.UserId == request.UserId))
            {
                // check if CourseSectionId exists
                if (CourseSection.Any(c => c.Id == request.CourseSectionId))
                {
                    var currentCourseId = (from c in CourseSection
                                           where c.Id == request.CourseSectionId
                                           select c.CourseId
                                          ).FirstOrDefault();

                    var userEnrolledOnCourse = (from u in MyCourses
                                                where u.CourseID == currentCourseId
                                                select u).FirstOrDefault();

                    // check if user enrolled on course
                    if (userEnrolledOnCourse != null)
                    {
                        var someEvent = (from e in UserTriggeredEvent
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

                            var currentCourseSectionIds = (from c in CourseSection
                                                           where c.CourseId == currentCourseId
                                                           select c.Id).ToList();

                            // check if section start event is a course start event as well
                            var isCourseStartEvent = (from i in UserTriggeredEvent
                                                      where i.UserID == request.UserId &&
                                                            i.EventID == (int)EventsEnum.StartSection &&
                                                            currentCourseSectionIds.Contains(i.Detail)
                                                      select i).FirstOrDefault();

                            result.Result = "Event created";

                            if (isCourseStartEvent == null)
                            {
                                //Console.ForegroundColor = ConsoleColor.Red;
                                //Console.WriteLine("This is a course start event! ");

                                var newCourseStartEvent = new UserTriggeredEvent
                                {
                                    UserID = request.UserId,
                                    EventID = (int)EventsEnum.StartCourse,
                                    TimeStamp = DateTime.Now,
                                    Detail = currentCourseId
                                };

                                UserTriggeredEvent.Add(newCourseStartEvent);

                                result.Result = "Event created and Course Started!!";
                            }

                            UserTriggeredEvent.Add(newStart);
                            SaveChangesAsync();

                            return result;
                        }
                        result.Result = "Section already started!!";
                        return result;
                    }
                    result.Result = "User isn't enrolled on course!!";
                    return result;
                }
                result.Result = "CourseSectionId doesn't exist!!";
                return result;
            }
            result.Result = "User doesn't exist!";
            return result;
        }

        public FinishSectionByUser finishSectionByUser(CreateSectionRequest request) 
        {
            var result = new FinishSectionByUser() { };

            // check if user exists
            if (Person.Any(i => i.UserId == request.UserId))
            {
                // check if coursesectionid exists
                if (CourseSection.Any(c => c.Id == request.CourseSectionId))
                {

                    var doesEventExist = (from d in UserTriggeredEvent
                                          where d.UserID == request.UserId &&
                                                d.EventID == (int)EventsEnum.StartSection &&
                                                d.Detail == request.CourseSectionId
                                          select d).FirstOrDefault();

                    // check if event started
                    if (doesEventExist != null)
                    {
                        var hasEventBeenFinished = (from d in UserTriggeredEvent
                                                    where d.UserID == request.UserId &&
                                                          d.EventID == (int)EventsEnum.FinishSection &&
                                                          d.Detail == request.CourseSectionId
                                                    select d).FirstOrDefault();

                        if (hasEventBeenFinished == null)
                        {
                            var currentCourseId = (from c in CourseSection
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


                            var currentCourseSectionIds = (from c in CourseSection
                                                           where c.CourseId == currentCourseId
                                                           select c.Id).ToList();

                            var totalNrOfSectionsInCurrentCourse = (from t in CourseSection
                                                                    where t.CourseId == currentCourseId
                                                                    select t).Count();

                            var finishedNrOfSectionsInCurrentCourse = (from f in UserTriggeredEvent
                                                                       where f.UserID == request.UserId &&
                                                                             f.EventID == (int)EventsEnum.FinishSection &&
                                                                             currentCourseSectionIds.Contains(f.Detail)
                                                                       select f
                                                                       ).Count();

                            bool hasCurrentCourseBeenFinished = totalNrOfSectionsInCurrentCourse == (finishedNrOfSectionsInCurrentCourse + 1);

                            result.Result = "Section Finished";

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

                                UserTriggeredEvent.Add(newCourseFinish);

                                result.Result = "Section and Course Finished";
                            }

                            UserTriggeredEvent.Add(newFinish);
                            SaveChangesAsync();

                            return result;
                        }
                        result.Result = "Section already finished!";
                        return result;
                    }
                    result.Result = "Section not started yet!";
                    return result;
                }
                result.Result = "CourseSectionId doesn't exist!";
                return result;
            }
            result.Result = "User doesn't exist!";
            return result;
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


        // Methods for MyCoursesController
        public List<MyEnrolledCourses> GetMyCourses(int userid) 
        {
            var user = (from u in Person
                       where u.UserId == userid
                       select u).FirstOrDefault();

            var courses = new List<MyEnrolledCourses>();

            if (user == null) 
            {
                return courses;   
            }
            courses = (from mycourses in MyCourses
                           join allcourses in AllCourses on mycourses.CourseID equals allcourses.CourseId
                           where mycourses.UserID == userid
                           select new MyEnrolledCourses
                           {
                               CourseTitle = allcourses.CourseTitle,
                               Sections = (from coursesection in CourseSection
                                           join allsections in AllSections on coursesection.SectionId equals allsections.SectionID
                                           where coursesection.CourseId == allcourses.CourseId
                                           select new StartedSection
                                           {
                                               SectionID = allsections.SectionID,
                                               Started = UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.StartSection && x.Detail == coursesection.Id).FirstOrDefault() !=
                                                        null ? UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.StartSection && x.Detail == coursesection.Id).FirstOrDefault().TimeStamp : null,
                                               Finished = UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.FinishSection && x.Detail == coursesection.Id).FirstOrDefault() !=
                                                        null ? UserTriggeredEvent.Where(x => x.UserID == userid && x.EventID ==
                                                        (int)EventsEnum.FinishSection && x.Detail == coursesection.Id).FirstOrDefault().TimeStamp : null,
                                           }).ToList()
                           }).ToList();

            foreach (MyEnrolledCourses mec in courses)
            {
                var courseStarted = mec.Sections.Where(x => x.Started != null).Min(x => x.Started);
                mec.CourseStarted = courseStarted != null ? courseStarted : null;
                double totalsections = mec.Sections.Count();

                var courseFinished = mec.Sections.Where(x => x.Finished != null).Max(x => x.Finished);
                double finishedsections = mec.Sections.Where(x => x.Finished != null).Count();

                // figure out this bit 
                var progress = (int)((finishedsections / totalsections) * 100);

                mec.CourseFinished = progress == 100 ? courseFinished : null;
                mec.CourseProgress = progress;
            }

            return courses;
        }


        //Methods for PersonController
        public List<Person> GetAllPersons() 
        {
            var person = Person.ToList();

            return person;
        }

        public Person? GetPerson(int id) 
        {
            var person = Person.Find(id);

            return person;
        }

        public Person PostPerson(Person person) 
        {
            Person.Add(person);
            SaveChanges();

            return person;
        }

        public Person? DeletePerson(int id) 
        {
            var person = Person.Find(id);

            if(person != null) 
            { 
                Person.Remove(person);
                SaveChanges();
            }
            return person;
        }

        public bool PersonExists(int id)
        {
            return (Person?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
