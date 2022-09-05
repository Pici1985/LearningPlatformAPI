namespace LearningPlatformAPI.Models
{
    public class UsersNrOfFinishedCourses
    {
        public string Title { get; set; }
        public IOrderedEnumerable<NrOfFinishedCourses> Leaders { get; set; }
    }
}
