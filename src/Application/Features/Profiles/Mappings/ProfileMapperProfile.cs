using AutoMapper;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using DTO.Profile;
using DTO.Profile.Educations;
using DTO.Profile.WorkExperiences;
using DTO.Profile.WorkExperiences.WorkExperienceBullets;
using DTO.Response;

namespace Application.Features.Profiles.Mappings;

public sealed class ProfileMapperProfile : Profile
{
    public ProfileMapperProfile()
    {
        CreateMap<UserProfile, ProfileResponse>()
            .ForMember(d => d.Skills, opt => opt.MapFrom(s => s.Skills.OrderBy(sk => sk.OrderIndex).Select(sk => sk.Name).ToList()))
            .ForMember(d => d.WorkExperiences, opt => opt.MapFrom(s => s.WorkExperiences))
            .ForMember(d => d.Educations, opt => opt.MapFrom(s => s.Educations));

        CreateMap<WorkExperience, WorkExperienceItemResponse>()
            .ForMember(d => d.JobType, opt => opt.MapFrom(s => s.JobType.HasValue
                ? new ListItemBaseResponse { Id = (int)s.JobType.Value, Name = s.JobType.Value.ToString() }
                : null))
            .ForMember(d => d.Bullets, opt => opt.MapFrom(s => s.Bullets));

        CreateMap<WorkExperienceBullet, WorkExperienceBulletResponse>();

        CreateMap<Education, EducationItemResponse>()
            .ForMember(d => d.DegreeType, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.DegreeType, Name = s.DegreeType.ToString() }));

        CreateMap<EeoData, EeoDataResponse>();
    }
}
