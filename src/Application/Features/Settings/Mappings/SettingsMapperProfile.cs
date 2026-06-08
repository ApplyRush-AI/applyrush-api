using AutoMapper;
using Domain.Entities.Settings;
using DTO.Profile.Settings.JobPreferences;
using DTO.Profile.Settings.NotificationPreferences;
using DTO.Response;

namespace Application.Features.Settings.Mappings;

public sealed class SettingsMapperProfile : Profile
{
    public SettingsMapperProfile()
    {
        CreateMap<UserNotificationPreference, NotificationPreferenceResponse>();

        CreateMap<UserJobPreference, JobPreferenceResponse>()
            .ForMember(d => d.WorkModel, opt => opt.MapFrom(s => s.WorkModel.HasValue
                ? new ListItemBaseResponse { Id = (int)s.WorkModel.Value, Name = s.WorkModel.Value.ToString() }
                : null));
    }
}
