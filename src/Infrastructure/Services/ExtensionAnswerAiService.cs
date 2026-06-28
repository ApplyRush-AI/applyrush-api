using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;

namespace Infrastructure.Services;

public sealed class ExtensionAnswerAiService : IExtensionAnswerAiService
{
    // Stub: Full LLM integration will be implemented alongside MS5 AI billing work.
    // For now, returns a placeholder answer to allow the endpoint to be consumed.
    public Task<ExtensionAnswerAiResult> GenerateAnswerAsync(ExtensionAnswerAiOptions options, CancellationToken cancellationToken)
    {
        var answer = $"Based on my background as a {options.UserTitle ?? "professional"} with expertise in " +
                     $"{string.Join(", ", options.Skills.Take(3))}, {options.Question.ToLowerInvariant().TrimEnd('?')} " +
                     "is an area where I bring strong value through hands-on experience and a commitment to continuous improvement.";

        return Task.FromResult(new ExtensionAnswerAiResult { Answer = answer });
    }
}
