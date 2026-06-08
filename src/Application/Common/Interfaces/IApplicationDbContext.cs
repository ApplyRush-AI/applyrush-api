using Domain.Entities.JobFunctions;
using Domain.Entities.Jobs.JobApplications;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Entities.Jobs.UserSavedJobs;
using Domain.Entities.Languages;
using Domain.Entities.Notifications;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.UserJobTypeReferences;
using Domain.Entities.Profiles.UserPreferredJobFunctions;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.UserSkills;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using Domain.Entities.RefreshTokens;
using Domain.Entities.Resumes.Resume;
using Domain.Entities.Settings;
using Domain.Entities.Tailoring.ResumeAnalyses;
using Domain.Entities.Tailoring.ResumeTailorings;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;
    DbSet<ApplicationUser> User { get; }
    DbSet<RefreshToken> RefreshToken { get; }
    DbSet<Notification> Notification { get; }
    DbSet<Language> Language { get; }

    // Profiles
    DbSet<UserProfile> UserProfile { get; }
    DbSet<Education> Education { get; }
    DbSet<WorkExperience> WorkExperience { get; }
    DbSet<WorkExperienceBullet> WorkExperienceBullet { get; }
    DbSet<UserSkill> UserSkill { get; }
    DbSet<EeoData> EeoData { get; }
    DbSet<UserJobTypePreference> UserJobTypePreference { get; }
    DbSet<UserPreferredJobFunction> UserPreferredJobFunction { get; }

    // Job Functions
    DbSet<JobFunction> JobFunction { get; }

    // Settings
    DbSet<UserNotificationPreference> UserNotificationPreference { get; }
    DbSet<UserJobPreference> UserJobPreference { get; }

    // Resumes
    DbSet<Resume> Resume { get; }

    // Jobs
    DbSet<JobListing> JobListing { get; }
    DbSet<UserJobMatch> UserJobMatch { get; }
    DbSet<UserSavedJob> UserSavedJob { get; }
    DbSet<JobApplication> JobApplication { get; }

    // Tailoring & Analysis
    DbSet<ResumeTailoring> ResumeTailoring { get; }
    DbSet<ResumeAnalysis> ResumeAnalysis { get; }
}
