using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events.Extension;
using DTO.Enums.Extension;

namespace Domain.Entities.Extension.ExtensionSessions;

public sealed class ExtensionSession : BaseAuditableEntity
{
    private ExtensionSession() { }

    public int UserId { get; private set; }
    public string JobUrl { get; private set; } = null!;
    public int CreditsUsed { get; private set; }
    public ExtensionSessionStatus Status { get; private set; }

    public ApplicationUser User { get; } = null!;

    public static ExtensionSession Create(IExtensionSessionInsertData data)
    {
        var session = new ExtensionSession
        {
            UserId = data.UserId,
            JobUrl = data.JobUrl,
            CreditsUsed = data.CreditsUsed,
            Status = ExtensionSessionStatus.Active
        };

        session.AddDomainEvent(new ExtensionSessionCreatedEvent(session));
        return session;
    }

    public void Complete()
    {
        Status = ExtensionSessionStatus.Completed;
    }

    public void Fail()
    {
        Status = ExtensionSessionStatus.Failed;
    }
}
