using Application.Features.Resumes.Commands;
using AutoMapper;
using Domain.Entities.Resumes;
using Domain.Entities.Resumes.Resume;
using DTO.MessageBroker.Messages.Resumes;
using DTO.Resumes;

namespace Application.Features.Resumes.Mappings;

public sealed class ResumeMapperProfile : Profile
{
    public ResumeMapperProfile()
    {
        CreateMap<Resume, ResumeListItemResponse>()
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.Created))
            .ForMember(d => d.LastModifiedAt, opt => opt.MapFrom(s => s.LastModified));

        CreateMap<Resume, ResumeDetailResponse>()
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.Created))
            .ForMember(d => d.LastModifiedAt, opt => opt.MapFrom(s => s.LastModified));

        CreateMap<ResumeUploadedMessage, ResumeParseCommand>()
            .ConstructUsing((src, _) => new ResumeParseCommand(src.ResumeId, src.UserId));

        CreateMap<ResumeParseData, ResumeParseDataResponse>();
        CreateMap<ResumeParseExperienceItem, ResumeParsedExperienceItem>();
        CreateMap<ResumeParseEducationItem, ResumeParsedEducationItem>();
    }
}
