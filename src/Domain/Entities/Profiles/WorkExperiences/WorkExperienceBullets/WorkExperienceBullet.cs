using Domain.Entities.Base;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

namespace Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;

public sealed class WorkExperienceBullet : BaseAuditableEntity
{
    private WorkExperienceBullet() { }

    public int WorkExperienceId { get; private set; }
    public string Content { get; private set; } = null!;
    public int OrderIndex { get; private set; }

    public WorkExperience WorkExperience { get; } = null!;

    public static WorkExperienceBullet Create(IWorkExperienceBulletInsertData data, int orderIndex)
    {
        return new WorkExperienceBullet
        {
            WorkExperienceId = data.WorkExperienceId,
            Content = data.Content,
            OrderIndex = orderIndex
        };
    }

    public void Update(IWorkExperienceBulletUpdateData data)
    {
        Content = data.Content;
    }

    public void SetOrderIndex(int orderIndex)
    {
        OrderIndex = orderIndex;
    }
}
