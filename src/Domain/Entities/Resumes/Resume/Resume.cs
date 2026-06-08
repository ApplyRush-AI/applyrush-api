using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Medias;
using Domain.Entities.Tailoring.ResumeTailorings;
using Domain.Entities.User;
using Domain.Events.Resumes;
using DTO.Enums;
using DTO.Enums.Media;
using DTO.Enums.Resume;

namespace Domain.Entities.Resumes.Resume;

public sealed class Resume : BaseAuditableEntity, IWithStatus, IWithMedia, IWithResumeParseData
{
    private Resume() { }

    public int UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public ResumeFileType FileType { get; private set; }
    public ResumeParseStatus ParseStatus { get; private set; }
    public Status Status { get; private set; }
    public ResumeParseData? ParsedData { get; private set; }
    public Media Media { get; set; } = null!;

    public ApplicationUser User { get; } = null!;
    public ICollection<ResumeTailoring> Tailorings { get; } = new List<ResumeTailoring>();

    public static Resume Create(IResumeInsertData data)
    {
        var resume = new Resume
        {
            UserId = data.UserId,
            Name = data.Name,
            IsPrimary = data.IsPrimary,
            FileType = data.FileType,
            ParseStatus = ResumeParseStatus.Pending,
            Status = Status.Active,
            Media = new Media(MediaEntityType.Resume)
        };

        resume.AddDomainEvent(new ResumeUploadedEvent(resume));
        return resume;
    }

    public void SetParsedData(ResumeParseData data)
    {
        ParsedData = data;
    }

    public void Rename(string name)
    {
        Name = name;
    }

    public void SetPrimary()
    {
        IsPrimary = true;
        AddDomainEvent(new ResumeSetPrimaryEvent(this));
    }

    public void ClearPrimary()
    {
        IsPrimary = false;
    }

    public void MarkParseCompleted()
    {
        ParseStatus = ResumeParseStatus.Completed;
        AddDomainEvent(new ResumeParsedEvent(this));
    }

    public void MarkParseFailed()
    {
        ParseStatus = ResumeParseStatus.Failed;
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
        AddDomainEvent(new ResumeDeletedEvent(this));
    }
}
