using AutoMapper;
using Domain.Entities.JobFunctions;
using DTO.Enums;
using DTO.Profile.JobFunctions;

namespace Application.Features.JobFunctions.Mappings;

public sealed class JobFunctionMapperProfile : Profile
{
    public JobFunctionMapperProfile()
    {
        CreateMap<JobFunction, JobFunctionItemResponse>()
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children.Where(c => c.Status == Status.Active).OrderBy(c => c.Name)));
    }
}
