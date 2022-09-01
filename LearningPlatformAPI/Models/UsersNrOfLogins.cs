namespace LearningPlatformAPI.Models
{
    public class UsersNrOfLogins
    {
        public string Title { get; set; }
        public IOrderedEnumerable<NrOfLoginTimes> Leaders { get; set; }
    }
}
