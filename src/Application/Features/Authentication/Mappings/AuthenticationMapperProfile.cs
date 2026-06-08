using AutoMapper;
using Domain.Entities.User;
using DTO.Authentication;

namespace Application.Features.Authentication.Mappings;

public sealed class AuthenticationMapperProfile : Profile
{
    public AuthenticationMapperProfile()
    {
        CreateMap<ApplicationUser, LoginUserInfo>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.Initials, opt => opt.MapFrom(s => ComputeInitials(s.FirstName, s.LastName)));
    }

    private static string ComputeInitials(string? firstName, string? lastName)
    {
        var result = string.Empty;
        if (!string.IsNullOrWhiteSpace(firstName)) result += firstName[0];
        if (!string.IsNullOrWhiteSpace(lastName)) result += lastName[0];
        return result.ToUpper();
    }
}
