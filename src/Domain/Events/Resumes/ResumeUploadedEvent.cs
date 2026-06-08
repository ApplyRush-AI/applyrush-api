using Domain.Entities.Resumes.Resume;

namespace Domain.Events.Resumes;

public sealed class ResumeUploadedEvent : BaseEvent
{
    public ResumeUploadedEvent(Resume resume)
    {
        Resume = resume;
    }

    public Resume Resume { get; }
}
