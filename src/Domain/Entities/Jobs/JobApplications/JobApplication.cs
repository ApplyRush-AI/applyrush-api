using Domain.Entities.Base;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.User;
using Domain.Events.Jobs;
using DTO.Enums.JobApplication;

namespace Domain.Entities.Jobs.JobApplications;

public sealed class JobApplication : BaseAuditableEntity
{
    private JobApplication() { }

    public int UserId { get; private set; }
    public int JobId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public string? Note { get; private set; }

    public ApplicationUser User { get; } = null!;
    public JobListing Job { get; } = null!;

    public static JobApplication Create(IJobApplicationInsertData data)
    {
        var application = new JobApplication
        {
            UserId = data.UserId,
            JobId = data.JobId,
            Status = ApplicationStatus.Applied
        };

        application.AddDomainEvent(new JobApplicationCreatedEvent(application));
        return application;
    }

    public void UpdateStage(ApplicationStatus status)
    {
        Status = status;
        AddDomainEvent(new JobApplicationStatusUpdatedEvent(this));
    }

    public void UpdateNote(string note)
    {
        Note = note;
        AddDomainEvent(new JobApplicationNoteUpdatedEvent(this));
    }

    public void Delete()
    {
        Status = ApplicationStatus.Deleted;
    }
}
