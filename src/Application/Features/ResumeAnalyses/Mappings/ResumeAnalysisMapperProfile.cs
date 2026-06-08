using Application.Common.Interfaces.Services.Ai;
using Application.Features.ResumeAnalyses.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Notification.Payloads;
using DTO.Response;
using DTO.Resumes;
using System.Text.Json;

namespace Application.Features.ResumeAnalyses.Mappings;

public sealed class ResumeAnalysisMapperProfile : Profile
{
    public ResumeAnalysisMapperProfile()
    {
        CreateMap<ResumeAnalysis, ResumeAnalysisResponse>()
            .ForMember(d => d.OverallGrade, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.OverallGrade, Name = s.OverallGrade.ToString() }))
            .ForMember(d => d.Status, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.Status, Name = s.Status.ToString() }))
            .ForMember(d => d.Issues, opt => opt.MapFrom((s, _, _, ctx) =>
            {
                if (string.IsNullOrEmpty(s.Issues))
                    return new List<ResumeAnalysisIssueResponse>();

                return JsonSerializer.Deserialize<List<ResumeAnalysisIssueResponse>>(s.Issues)
                    ?? new List<ResumeAnalysisIssueResponse>();
            }))
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created));

        CreateMap<ResumeAnalysisFixAiResult, ResumeAnalysisFixResponse>();

        CreateMap<ResumeAnalysis, ResumeAnalysisAiOptions>();
        CreateMap<ResumeAnalysis, ResumeAnalysisFixAiOptions>()
            .ForMember(d => d.AnalysisId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.IssueId, opt => opt.Ignore());
        CreateMap<ResumeAnalysis, ResumeAnalysisFixAllAiOptions>()
            .ForMember(d => d.AnalysisId, opt => opt.MapFrom(s => s.Id));

        CreateMap<ResumeAnalysisAiResult, ResumeAnalysisCompleteData>()
            .ForMember(d => d.Issues, opt => opt.MapFrom(s => s.IssuesJson))
            .ForMember(d => d.Status, opt => opt.Ignore());

        CreateMap<ResumeAnalysisNotificationPayload, string>()
            .ConvertUsing((src, _) => JsonSerializer.Serialize(src));
    }
}
