using Domain.Entities.Extension.ExtensionSessions;

namespace Domain.Events.Extension;

public sealed class ExtensionSessionCreatedEvent : BaseEvent
{
    public ExtensionSessionCreatedEvent(ExtensionSession session)
    {
        Session = session;
    }

    public ExtensionSession Session { get; }
}
