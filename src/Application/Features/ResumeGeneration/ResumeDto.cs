using System;
using System.Collections.Generic;
using Domain.Aggregates.Internships;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;

namespace Application.Features.ResumeGeneration;

// This class serves as a serializable, flattened representation of all data needed for resume generation
public class ResumeDto
{
    // Student Data
    public string StudentName { get; set; }
    public string University { get; set; }
    public string Faculty { get; set; }
    public int GraduationYear { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
    public string Location { get; set; }
    public string Role { get; set; }
    
    // Internship Data
    public string InternshipTitle { get; set; }
    public string InternshipDescription { get; set; }
    public string InternshipRequirements { get; set; }
    public string InternshipResponsibilities { get; set; }
    
    // Company Data
    public string CompanyName { get; set; }
    public string CompanyDescription { get; set; }
    public string CompanyIndustry { get; set; }
    
    // Skills, Experiences, Projects
    public List<SkillDto> Skills { get; set; }
    public List<ExperienceDto> Experiences { get; set; }
    public List<ProjectDto> Projects { get; set; }

    public static ResumeDto Create(
        Domain.Aggregates.Profiles.StudentProfile student, 
        Internship internship,
        Domain.Aggregates.Profiles.CompanyProfile company,
        IEnumerable<Skill> skills,
        IEnumerable<StudentExperience> experiences,
        IEnumerable<StudentProject> projects)
    {
        var dto = new ResumeDto
        {
            // Student data
            StudentName = student.FullName,
            University = student.University.ToString(),
            Faculty = student.Faculty,
            GraduationYear = student.GraduationYear.Value,
            PhoneNumber = student.PhoneNumber.Value,
            Bio = student.Bio ?? string.Empty,
            Location = student.Location ?? string.Empty,
            Role = student.Role ?? string.Empty,
            
            // Internship data
            InternshipTitle = internship.Title,
            InternshipDescription = internship.About,
            InternshipRequirements = internship.Requirements,
            InternshipResponsibilities = internship.KeyResponsibilities,
            
            // Company data
            CompanyName = company.CompanyName,
            CompanyDescription = company.Description,
            CompanyIndustry = company.Industry,
            
            // Collections
            Skills = new List<SkillDto>(),
            Experiences = new List<ExperienceDto>(),
            Projects = new List<ProjectDto>()
        };

        // Map each collection
        foreach (var skill in skills)
        {
            dto.Skills.Add(new SkillDto 
            { 
                Name = skill.Name
            });
        }

        foreach (var experience in experiences)
        {
            dto.Experiences.Add(new ExperienceDto
            {
                Title = experience.JobTitle,
                Company = experience.CompanyName,
                StartDate = experience.DateRange.StartDate,
                EndDate = experience.DateRange.EndDate
            });
        }

        foreach (var project in projects)
        {
            dto.Projects.Add(new ProjectDto
            {
                Title = project.ProjectName,
                Description = project.Description,
                Link = project.ProjectUrl ?? string.Empty,
            });
        }

        return dto;
    }

    public class SkillDto
    {
        public string Name { get; set; }
    }

    public class ExperienceDto
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}
