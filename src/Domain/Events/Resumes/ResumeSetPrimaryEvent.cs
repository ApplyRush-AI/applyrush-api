using Domain.Entities.Resumes.Resume;

namespace Domain.Events.Resumes;

public sealed class ResumeSetPrimaryEvent : BaseEvent
{
    public ResumeSetPrimaryEvent(Resume resume)
    {
        Resume = resume;
    }

    public Resume Resume { get; }
}
