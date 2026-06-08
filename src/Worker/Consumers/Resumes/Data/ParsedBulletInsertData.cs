using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

namespace Worker.Consumers.Resumes.Data;

internal sealed record ParsedBulletInsertData(
    int WorkExperienceId,
    string Content) : IWorkExperienceBulletInsertData;
