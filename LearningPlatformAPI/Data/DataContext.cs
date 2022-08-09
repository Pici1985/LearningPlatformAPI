using LearningPlatformAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Person> Person { get; set; }
        public DbSet<PersonLoggedInOnDate> PersonLoggedInOnDate { get; set; }
        
        
        public Person? CheckCredentials(string email, string password)
        {
            return (from p in Person
                    where p.Email == email && p.Password == password
                    select p).FirstOrDefault(); 
        }

    }
}
