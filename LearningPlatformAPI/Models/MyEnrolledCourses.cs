namespace LearningPlatformAPI.Models
{
    public class MyEnrolledCourses
    {
        public string CourseTitle { get; set; }
        public List <StartedSection>? Sections { get; set; }
        public DateTime? CourseStarted { get; set; }
        public DateTime? CourseFinished { get; set; }
        public int CourseProgress { get; set; }
    }
}
