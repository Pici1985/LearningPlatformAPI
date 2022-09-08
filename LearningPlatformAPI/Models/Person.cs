using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class Person
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Email { get; set; }
        public string Password { get; set; }    
        public string? Gender { get; set; }  
        public int Age { get; set; }    
        public string? Occupation { get; set; } 
        public DateTime DateOfRegistration { get; set; }
        public Guid? Token { get; set; }
    }
}
