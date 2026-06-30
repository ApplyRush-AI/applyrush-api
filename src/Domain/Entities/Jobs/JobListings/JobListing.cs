using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Jobs.JobApplications;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Entities.Jobs.UserSavedJobs;
using Domain.Events.Jobs;
using DTO.Enums;
using DTO.Enums.Job;
using System.Collections.Generic;

namespace Domain.Entities.Jobs.JobListings;

public sealed class JobListing : BaseAuditableEntity, IWithStatus
{
    private JobListing() { }

    public string ExternalId { get; private set; } = null!;
    public JobSource Source { get; private set; }
    public string Title { get; private set; } = null!;
    public string Company { get; private set; } = null!;
    public string? CompanyLogoUrl { get; private set; }
    public string Description { get; private set; } = null!;
    public string? About { get; private set; }
    public string? Responsibilities { get; private set; }
    public string? Requirements { get; private set; }
    public string? Benefits { get; private set; }
    public string? RequiredSkills { get; private set; }
    public string? Industry { get; private set; }
    public string Location { get; private set; } = null!;
    public WorkModel WorkModel { get; private set; }
    public JobType JobType { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public decimal? SalaryMin { get; private set; }
    public decimal? SalaryMax { get; private set; }
    public string? Currency { get; private set; }
    public int? YearsRequired { get; private set; }
    public int? ApplicantCount { get; private set; }
    public DateTime PostedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string ApplyUrl { get; private set; } = null!;
    public bool H1BSupported { get; private set; }
    public string? AiSummary { get; private set; }
    public Status Status { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    public ICollection<JobListingJobFunction> JobFunctions { get; } = new List<JobListingJobFunction>();
    public ICollection<UserJobMatch> UserMatches { get; } = new List<UserJobMatch>();
    public ICollection<UserSavedJob> SavedByUsers { get; } = new List<UserSavedJob>();
    public ICollection<JobApplication> Applications { get; } = new List<JobApplication>();

    public static JobListing Create(IJobListingInsertData data)
    {
        var jobListing = new JobListing
        {
            ExternalId = data.ExternalId,
            Source = data.Source,
            Title = data.Title,
            Company = data.Company,
            CompanyLogoUrl = data.CompanyLogoUrl,
            Description = data.Description,
            About = data.About,
            Responsibilities = data.Responsibilities,
            Requirements = data.Requirements,
            Benefits = data.Benefits,
            RequiredSkills = data.RequiredSkills,
            Industry = data.Industry,
            Location = data.Location,
            WorkModel = data.WorkModel,
            JobType = data.JobType,
            ExperienceLevel = data.ExperienceLevel,
            SalaryMin = data.SalaryMin,
            SalaryMax = data.SalaryMax,
            Currency = data.Currency,
            YearsRequired = data.YearsRequired,
            ApplicantCount = data.ApplicantCount,
            PostedAt = data.PostedAt,
            ExpiresAt = data.ExpiresAt,
            ApplyUrl = data.ApplyUrl,
            H1BSupported = data.H1BSupported,
            AiSummary = data.AiSummary,
            LastSyncedAt = data.LastSyncedAt,
            Status = Status.Active
        };

        jobListing.AddDomainEvent(new JobListingCreatedEvent(jobListing));
        return jobListing;
    }

    public void Update(IJobListingUpdateData data)
    {
        Title = data.Title;
        Company = data.Company;
        CompanyLogoUrl = data.CompanyLogoUrl;
        Description = data.Description;
        About = data.About;
        Responsibilities = data.Responsibilities;
        Requirements = data.Requirements;
        Benefits = data.Benefits;
        RequiredSkills = data.RequiredSkills;
        Industry = data.Industry;
        Location = data.Location;
        WorkModel = data.WorkModel;
        JobType = data.JobType;
        ExperienceLevel = data.ExperienceLevel;
        SalaryMin = data.SalaryMin;
        SalaryMax = data.SalaryMax;
        Currency = data.Currency;
        YearsRequired = data.YearsRequired;
        ApplicantCount = data.ApplicantCount;
        PostedAt = data.PostedAt;
        ExpiresAt = data.ExpiresAt;
        ApplyUrl = data.ApplyUrl;
        H1BSupported = data.H1BSupported;
        AiSummary = data.AiSummary;
        LastSyncedAt = data.LastSyncedAt;
    }

    public void SetJobFunctions(IEnumerable<int> jobFunctionIds)
    {
        JobFunctions.Clear();
        foreach (var id in jobFunctionIds)
            JobFunctions.Add(JobListingJobFunction.Create(id));
    }

    public void AppendUserMatches(IReadOnlyList<UserJobMatch> matches)
    {
        foreach (var match in matches)
            UserMatches.Add(match);

        AddDomainEvent(new JobUserMatchesComputedEvent(this));
    }

    public void Activate()
    {
        Status = Status.Active;
    }

    public void Deactivate() 
    {
        Status = Status.Inactive;
    }
    public void Delete() 
    {
        Status = Status.Deleted;
    }
}
