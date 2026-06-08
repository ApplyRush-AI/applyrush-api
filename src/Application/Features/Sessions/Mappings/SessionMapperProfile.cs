using AutoMapper;
using Domain.Entities.RefreshTokens;
using DTO.Profile.Sessions;

namespace Application.Features.Sessions.Mappings;

public sealed class SessionMapperProfile : Profile
{
    public SessionMapperProfile()
    {
        CreateMap<RefreshToken, SessionResponse>()
            .ForMember(d => d.Token, opt => opt.MapFrom(s => s.Value));
    }
}
