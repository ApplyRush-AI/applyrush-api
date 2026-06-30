using Application.Common.Interfaces;
using Domain.Entities.JobFunctions;
using Domain.Entities.Jobs.JobApplications;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Entities.Jobs.UserHiddenJobs;
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
using Domain.Entities.Extension.ExtensionSessions;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Entities.Subscriptions.UserCredits;
using Domain.Entities.Subscriptions.CreditTransactions;
using Domain.Entities.Tailoring.ResumeTailorings;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class => base.Entry(entity);
    public DbSet<ApplicationUser> User => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
    public DbSet<Notification> Notification => Set<Notification>();
    public DbSet<Language> Language => Set<Language>();

    // Profiles
    public DbSet<UserProfile> UserProfile => Set<UserProfile>();
    public DbSet<Education> Education => Set<Education>();
    public DbSet<WorkExperience> WorkExperience => Set<WorkExperience>();
    public DbSet<WorkExperienceBullet> WorkExperienceBullet => Set<WorkExperienceBullet>();
    public DbSet<UserSkill> UserSkill => Set<UserSkill>();
    public DbSet<EeoData> EeoData => Set<EeoData>();
    public DbSet<UserJobTypePreference> UserJobTypePreference => Set<UserJobTypePreference>();
    public DbSet<UserPreferredJobFunction> UserPreferredJobFunction => Set<UserPreferredJobFunction>();

    // Job Functions
    public DbSet<JobFunction> JobFunction => Set<JobFunction>();

    // Settings
    public DbSet<UserNotificationPreference> UserNotificationPreference => Set<UserNotificationPreference>();
    public DbSet<UserJobPreference> UserJobPreference => Set<UserJobPreference>();

    // Resumes
    public DbSet<Resume> Resume => Set<Resume>();

    // Jobs
    public DbSet<JobListing> JobListing => Set<JobListing>();
    public DbSet<UserJobMatch> UserJobMatch => Set<UserJobMatch>();
    public DbSet<UserSavedJob> UserSavedJob => Set<UserSavedJob>();
    public DbSet<UserHiddenJob> UserHiddenJob => Set<UserHiddenJob>();
    public DbSet<JobApplication> JobApplication => Set<JobApplication>();
    public DbSet<ResumeTailoring> ResumeTailoring => Set<ResumeTailoring>();
    public DbSet<ResumeAnalysis> ResumeAnalysis => Set<ResumeAnalysis>();

    // Extension
    public DbSet<ExtensionSession> ExtensionSession => Set<ExtensionSession>();

    // Subscriptions
    public DbSet<UserSubscription> UserSubscription => Set<UserSubscription>();
    public DbSet<UserCredit> UserCredit => Set<UserCredit>();
    public DbSet<CreditTransaction> CreditTransaction => Set<CreditTransaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}