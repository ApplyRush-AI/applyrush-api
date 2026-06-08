using DTO.MessageBroker;

namespace DTO.MessageBroker.Messages.Analysis;

public sealed record ResumeAnalysisProcessMessage(int AnalysisId, int UserId) : MessageBase;
