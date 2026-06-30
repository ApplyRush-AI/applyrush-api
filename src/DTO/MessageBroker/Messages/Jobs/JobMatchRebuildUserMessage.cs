using DTO.MessageBroker;

namespace DTO.MessageBroker.Messages.Jobs;

public sealed record JobMatchRebuildUserMessage(int UserId) : MessageBase;
