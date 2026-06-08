using Application.Common.Interfaces.Services.Ai;
using Application.Features.ResumeTailorings.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using DTO.Notification.Payloads;
using DTO.Response;
using DTO.Resumes;
using System.Text.Json;

namespace Application.Features.ResumeTailorings.Mappings;

public sealed class ResumeTailoringMapperProfile : Profile
{
    public ResumeTailoringMapperProfile()
    {
        CreateMap<ResumeTailoring, ResumeTailoringResponse>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.Status, Name = s.Status.ToString() }))
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created));

        CreateMap<ResumeTailoring, ResumeTailoringAiOptions>();
        CreateMap<ResumeTailoring, ResumeTailoringAiRewriteOptions>()
            .ForMember(d => d.TailoringId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Action, opt => opt.Ignore())
            .ForMember(d => d.FreeFormInstruction, opt => opt.Ignore());

        CreateMap<ResumeTailoringAiRewriteResult, ResumeTailoringAiRewriteResponse>();

        CreateMap<ResumeTailoringAiResult, ResumeTailoringCompleteData>()
            .ForMember(d => d.Status, opt => opt.Ignore());

        CreateMap<ResumeTailoringNotificationPayload, string>()
            .ConvertUsing((src, _) => JsonSerializer.Serialize(src));
    }
}
