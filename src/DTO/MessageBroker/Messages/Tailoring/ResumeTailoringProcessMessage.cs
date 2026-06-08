using DTO.MessageBroker;

namespace DTO.MessageBroker.Messages.Tailoring;

public sealed record ResumeTailoringProcessMessage(int TailoringId, int UserId) : MessageBase;
