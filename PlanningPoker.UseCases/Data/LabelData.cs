using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record LabelData(string Id, string? Name, string? Description, string? ColorHexCode);

public static class LabelDataExtensions
{
    public static LabelData ToLabelData(this Property property)
    {
        const string labelNameIdentifierKey = "Name";
        const string colorHexCodeIdentifierKey = "colorHex";
        return new LabelData(Id: property.Id,
            Name: property.Data[labelNameIdentifierKey],
            Description: null,
            ColorHexCode: property.Data[colorHexCodeIdentifierKey]);
    }
}
