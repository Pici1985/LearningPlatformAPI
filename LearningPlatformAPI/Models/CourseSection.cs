using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class CourseSection
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SectionId { get; set; }

    }
}
