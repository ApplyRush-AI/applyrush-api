using Application.Common.Localization;
using DTO.Response;

namespace Application.Common.Helpers
{
    public static class EnumHelper
    {
        public static List<ListItemBaseResponse> ToListItemBaseResponses<TEnum>(ILocalizationService localization) where TEnum : Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            return enumValues.Select(e => new ListItemBaseResponse
            {
                Id = (int)Convert.ChangeType(e, typeof(int)),
                Name = localization.GetValue(e)
            }).ToList();
        }
    }
}
