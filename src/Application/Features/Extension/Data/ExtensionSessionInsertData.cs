using Domain.Entities.Extension.ExtensionSessions;

namespace Application.Features.Extension.Data;

internal sealed record ExtensionSessionInsertData(
    int UserId,
    string JobUrl,
    int CreditsUsed) : IExtensionSessionInsertData;
