using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class MyCourses
    {
        [Key]
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int UserID { get; set; }
    }
}
