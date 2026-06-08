using Domain.Entities.Resumes.Resume;

namespace Domain.Events.Resumes;

public sealed class ResumeDeletedEvent : BaseEvent
{
    public ResumeDeletedEvent(Resume resume)
    {
        Resume = resume;
    }

    public Resume Resume { get; }
}
