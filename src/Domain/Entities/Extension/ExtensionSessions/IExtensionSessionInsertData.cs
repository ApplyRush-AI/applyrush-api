namespace Domain.Entities.Extension.ExtensionSessions;

public interface IExtensionSessionInsertData
{
    int UserId { get; }
    string JobUrl { get; }
    int CreditsUsed { get; }
}
