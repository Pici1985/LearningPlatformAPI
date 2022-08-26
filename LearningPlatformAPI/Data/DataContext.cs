using LearningPlatformAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Person> Person { get; set; }
        public DbSet<AllCourses> AllCourses { get; set; }
        public DbSet<MyCourses> MyCourses { get; set; }
        public DbSet<UserTriggeredEvent> UserTriggeredEvent { get; set; }
        public DbSet<AllSections> AllSections { get; set; }
        public DbSet<CourseSection> CourseSection { get; set; }        
        
        public Person? CheckCredentials(string email, string password)
        {
            return (from p in Person
                    where p.Email == email && p.Password == password
                    select p).FirstOrDefault(); 
        }

    }
}
