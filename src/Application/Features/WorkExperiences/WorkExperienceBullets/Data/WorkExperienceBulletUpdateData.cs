using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

namespace Application.Features.WorkExperiences.WorkExperienceBullets.Data;

public sealed record WorkExperienceBulletUpdateData(
    string Content
    ) : IWorkExperienceBulletUpdateData;
