using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Website.Forms;

[AttributeUsage(AttributeTargets.Property)]
public class NotInListAttribute(string listOfStringsPropertyName) : ValidationAttribute
{
    private string ListOfStringsPropertyName { get; } = listOfStringsPropertyName;

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var instance = validationContext.ObjectInstance;
        var listProperty = instance.GetType().GetProperty(ListOfStringsPropertyName);
        if (listProperty is null)
        {
            return new ValidationResult($"Property '{ListOfStringsPropertyName}' not found.");
        }

        if (listProperty.GetValue(instance) is not List<string> forbiddenValues)
        {
            return new ValidationResult($"Property '{ListOfStringsPropertyName}' is not a list of strings.");
        }

        if (value?.ToString() is not null && forbiddenValues.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success!;
    }
}
