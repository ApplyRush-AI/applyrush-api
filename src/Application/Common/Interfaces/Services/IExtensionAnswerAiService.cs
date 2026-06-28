using Application.Common.Interfaces.Services.Ai;

namespace Application.Common.Interfaces.Services;

public interface IExtensionAnswerAiService
{
    Task<ExtensionAnswerAiResult> GenerateAnswerAsync(ExtensionAnswerAiOptions options, CancellationToken cancellationToken);
}
