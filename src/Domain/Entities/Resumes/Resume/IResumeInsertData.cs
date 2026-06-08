using DTO.Enums.Resume;

namespace Domain.Entities.Resumes.Resume;

public interface IResumeInsertData
{
    int UserId { get; }
    string Name { get; }
    bool IsPrimary { get; }
    ResumeFileType FileType { get; }
}
