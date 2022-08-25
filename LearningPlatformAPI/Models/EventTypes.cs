using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class EventTypes
    {
        [Key]
        public int Id { get; set; }
        public string? EventName { get; set; }
    }
}
