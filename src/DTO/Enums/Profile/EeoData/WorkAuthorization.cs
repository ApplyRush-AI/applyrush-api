using DTO.Attributes;

namespace DTO.Enums.Profile.EeoData;

public enum WorkAuthorization
{
    [LocalizationKey("enum.workAuthorization.citizen")]
    Citizen = 1,
    [LocalizationKey("enum.workAuthorization.permanentResident")]
    PermanentResident = 2,
    [LocalizationKey("enum.workAuthorization.workVisa")]
    WorkVisa = 3,
    [LocalizationKey("enum.workAuthorization.studentVisa")]
    StudentVisa = 4,
    [LocalizationKey("enum.workAuthorization.other")]
    Other = 5
}
