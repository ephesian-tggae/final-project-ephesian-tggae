using System.ComponentModel.DataAnnotations;

namespace MovieNest.Api.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return new ValidationResult(
                ErrorMessage ?? "This field is required.",
                [validationContext.MemberName ?? string.Empty]);
        }

        return ValidationResult.Success;
    }
}
