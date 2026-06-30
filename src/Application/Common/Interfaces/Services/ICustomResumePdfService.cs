using DTO.Resumes;

namespace Application.Common.Interfaces.Services;

public interface ICustomResumePdfService
{
    Task<Stream> RenderAsync(TailoredResumeContent content, TailoredResumeStyle? style, int userId, CancellationToken cancellationToken);
}
