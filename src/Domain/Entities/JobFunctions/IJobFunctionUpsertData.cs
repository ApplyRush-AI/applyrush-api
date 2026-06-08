namespace Domain.Entities.JobFunctions;

public interface IJobFunctionUpsertData
{
    string Name { get; }
    int? ParentId { get; }
}
