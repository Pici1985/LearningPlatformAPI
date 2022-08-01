using LearningPlatformAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatformAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Person> Person { get; set; }

        public void WriteStuff()
        {
            Console.WriteLine("Anyad");
        }

        // IQueryable<Person> // bool
        public bool CheckCredentials(string email, string password)
        {
            var credentials = from p in Person
                              where p.Email == email && p.Password == password
                              select p.FirstName;

            if (credentials.Any(email => (bool)email.Contains(email)) &&
                credentials.Any(password => (bool)password.Contains(password))
               )
            {
                //return (IQueryable<Person>)credentials;
                return true;
            }
            else 
            {
                return false;
            }

        }

    }
}
