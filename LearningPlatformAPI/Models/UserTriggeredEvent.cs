using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class UserTriggeredEvent
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Detail { get; set; }  
    }
}
