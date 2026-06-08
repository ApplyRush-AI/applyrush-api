using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public sealed class PdfExportService : IPdfExportService
{
    public Task<Stream> ExportProfileAsPdfAsync(int userId, CancellationToken cancellationToken)
    {
        // Stub: PDF generation not yet implemented
        // Will be implemented in a future milestone using a PDF library (e.g. QuestPDF)
        throw new NotImplementedException("PDF export is not yet implemented.");
    }

    public Task<Stream> ExportTailoringAsPdfAsync(int tailoringId, CancellationToken cancellationToken)
    {
        // Stub: Tailored resume PDF generation not yet implemented
        throw new NotImplementedException("Tailored resume PDF export is not yet implemented.");
    }
}

