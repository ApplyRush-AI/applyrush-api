using DTO.MessageBroker;

namespace DTO.MessageBroker.Messages.Jobs;

public sealed record JobMatchAllUsersMessage(int JobId) : MessageBase;
