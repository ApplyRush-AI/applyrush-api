using AutoMapper;
using Domain.Entities.Jobs.JobApplications;
using DTO.JobApplications;
using DTO.Response;

namespace Application.Features.JobApplications.Mappings;

public sealed class JobApplicationMapperProfile : Profile
{
    public JobApplicationMapperProfile()
    {
        CreateMap<JobApplication, JobApplicationResponse>()
            .ForMember(d => d.Stage, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created));
    }
}
