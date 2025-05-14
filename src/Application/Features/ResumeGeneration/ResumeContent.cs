using System.Collections.Generic;

namespace Application.Features.ResumeGeneration;

public class ResumeContent
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public List<ExperienceItem> Experience { get; set; }
    public List<ProjectItem> Projects { get; set; }
    public List<string> Skills { get; set; }
    public EducationItem Education { get; set; }
    
    public string PhoneNumber { get; set; }
    public string Location { get; set; }
    
    public class ExperienceItem
    {
        public string Role { get; set; }
        public string Company { get; set; }
        public string Duration { get; set; }
        public List<string> Description { get; set; }
    }
    
    public class ProjectItem
    {
        public string Title { get; set; }
        public List<string> Description { get; set; }
    }
    
    public class EducationItem
    {
        public string University { get; set; }
        public string Degree { get; set; }
        public string GradYear { get; set; }
        public List<string> Highlights { get; set; }
    }
}
