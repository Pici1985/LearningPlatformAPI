﻿namespace LearningPlatformAPI.Models
{
    public class StartedSection
    {
        public int CourseSectionID { get; set; }    
        public int SectionID { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
    }
}
