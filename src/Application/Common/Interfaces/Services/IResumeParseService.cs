using Domain.Entities.Resumes;

namespace Application.Common.Interfaces.Services;

public interface IResumeParseService
{
    Task<ResumeParseData> ParseAsync(Stream fileContent, string fileName, CancellationToken cancellationToken);
}
