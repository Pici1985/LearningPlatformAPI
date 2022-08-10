using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class AllCourses
    {
        [Key]
        public int CourseId { get; set; }

        public string? CourseTitle { get; set; }
    }
}
