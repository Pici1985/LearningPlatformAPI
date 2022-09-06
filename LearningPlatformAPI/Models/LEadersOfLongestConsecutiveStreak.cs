namespace LearningPlatformAPI.Models
{
    public class LeadersOfLongestConsecutiveStreak
    {
        public string Title { get; set; }
        public IOrderedEnumerable<LongestConsecutiveStreak> Leaders { get; set; }
    }
}
