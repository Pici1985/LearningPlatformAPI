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
            Console.WriteLine("test");
        }

        
        public Person? CheckCredentials(string email, string password)
        {
            return (from p in Person
                    where p.Email == email && p.Password == password
                    select p).FirstOrDefault();
            
            // if I would return a bool this would be the implementation but that's not a good practice
            //if(credentials != null)
            //{
            //    //return (IQueryable<Person>)credentials;
            //    return true;
            //}
            //return false;      
            

        }

    }
}
