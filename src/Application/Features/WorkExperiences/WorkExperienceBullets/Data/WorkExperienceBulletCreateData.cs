using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

namespace Application.Features.WorkExperiences.WorkExperienceBullets.Data;

public sealed record WorkExperienceBulletCreateData(
    int WorkExperienceId,
    string Content
    ) : IWorkExperienceBulletInsertData;
