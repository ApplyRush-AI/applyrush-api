namespace Domain.Entities.Resumes;

public interface IWithResumeParseData
{
    ResumeParseData? ParsedData { get; }
}
