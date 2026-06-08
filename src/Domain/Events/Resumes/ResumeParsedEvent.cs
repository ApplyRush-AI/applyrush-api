using Domain.Entities.Resumes.Resume;

namespace Domain.Events.Resumes;

public sealed class ResumeParsedEvent : BaseEvent
{
    public ResumeParsedEvent(Resume resume)
    {
        Resume = resume;
    }

    public Resume Resume { get; }
}
