using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

namespace Application.Features.Resumes.Data;

internal sealed record ParsedBulletInsertData(
    int WorkExperienceId,
    string Content) : IWorkExperienceBulletInsertData;
