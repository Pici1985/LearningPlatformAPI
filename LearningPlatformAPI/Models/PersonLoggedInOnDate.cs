using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class PersonLoggedInOnDate
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime DateTime { get; set; }   
    }
}
