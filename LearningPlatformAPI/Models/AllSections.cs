using System.ComponentModel.DataAnnotations;

namespace LearningPlatformAPI.Models
{
    public class AllSections
    {
        [Key]
        public int SectionID { get; set; }
        public int SectionTitle { get; set; }
    }
}
