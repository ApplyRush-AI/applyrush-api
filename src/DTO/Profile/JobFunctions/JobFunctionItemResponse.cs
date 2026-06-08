namespace DTO.Profile.JobFunctions;

public record JobFunctionItemResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int? ParentId { get; init; }
    public IReadOnlyList<JobFunctionItemResponse> Children { get; init; } = [];
}
