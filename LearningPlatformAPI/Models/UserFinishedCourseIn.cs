namespace LearningPlatformAPI.Models
{
    public class UserFinishedCourseIn
    {
        public int UserID { get; set; } 
        public int CourseID { get; set; }
        public TimeSpan FinishedIn { get; set; }
    }
}
