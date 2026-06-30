using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum EducationDisplayOrder
{
    [LocalizationKey("enum.educationDisplayOrder.degreeFirst")]
    DegreeFirst = 1,
    [LocalizationKey("enum.educationDisplayOrder.institutionFirst")]
    InstitutionFirst = 2
}
