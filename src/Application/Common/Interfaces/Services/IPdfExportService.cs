namespace Application.Common.Interfaces.Services;

public interface IPdfExportService
{
    Task<Stream> ExportProfileAsPdfAsync(int userId, CancellationToken cancellationToken);
    Task<Stream> ExportTailoringAsPdfAsync(int tailoringId, CancellationToken cancellationToken);
}
