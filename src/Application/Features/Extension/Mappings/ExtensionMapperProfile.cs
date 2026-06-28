using Application.Common.Interfaces.Services.Ai;
using Application.Features.Extension.Commands;
using AutoMapper;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using DTO.Extension;

namespace Application.Features.Extension.Mappings;

public sealed class ExtensionMapperProfile : Profile
{
    public ExtensionMapperProfile()
    {
        CreateMap<UserProfile, ExtensionProfileResponse>()
            .ForMember(d => d.Skills, opt => opt.MapFrom(s => s.Skills.OrderBy(sk => sk.OrderIndex).Select(sk => sk.Name).ToList()))
            .ForMember(d => d.WorkExperiences, opt => opt.MapFrom(s => s.WorkExperiences))
            .ForMember(d => d.Educations, opt => opt.MapFrom(s => s.Educations));

        CreateMap<WorkExperience, ExtensionWorkExperienceItemResponse>()
            .ForMember(d => d.Bullets, opt => opt.MapFrom(s => s.Bullets.Select(b => b.Content).ToList()));

        CreateMap<WorkExperienceBullet, string>().ConvertUsing(b => b.Content);

        CreateMap<Education, ExtensionEducationItemResponse>()
            .ForMember(d => d.DegreeType, opt => opt.MapFrom(s => s.DegreeType.ToString()));

        CreateMap<ExtensionAnswerAiResult, ExtensionAnswerResponse>();

        CreateMap<ExtensionAnswerGenerateCommand, ExtensionAnswerAiOptions>();
    }
}
