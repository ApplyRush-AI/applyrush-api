using Domain.Entities.Resumes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;

namespace Infrastructure.Common.Converters;

public sealed class ResumeParseDataJsonConverter : ValueConverter<ResumeParseData, string>
{
    private static readonly Expression<Func<ResumeParseData, string>> ConvertToExpr = x => Serialize(x);
    private static readonly Expression<Func<string, ResumeParseData>> ConvertFromExpr = x => Deserialize(x);

    public ResumeParseDataJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string Serialize(ResumeParseData data) =>
        JsonSerializer.Serialize(data);

    private static ResumeParseData Deserialize(string json) =>
        JsonSerializer.Deserialize<ResumeParseData>(json)!;
}
