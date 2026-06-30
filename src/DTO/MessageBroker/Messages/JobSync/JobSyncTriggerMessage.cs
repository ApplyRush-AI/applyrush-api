namespace DTO.MessageBroker.Messages.JobSync;

public sealed record JobSyncTriggerMessage(int PagesPerQuery) : MessageBase;
