using Domain.Entities.Resumes.Resume;
using DTO.Enums.Resume;

namespace Application.Features.Resumes.Data;

internal sealed record ResumeInsertData(
    int UserId, 
    string Name, 
    bool IsPrimary, 
    ResumeFileType FileType) : IResumeInsertData;
