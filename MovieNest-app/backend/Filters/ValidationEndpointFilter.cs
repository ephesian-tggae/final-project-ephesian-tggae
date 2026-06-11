using System.ComponentModel.DataAnnotations;
using MovieNest.Api.Dtos;
using MovieNest.Api.Exceptions;

namespace MovieNest.Api.Filters;

public class ValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var fieldErrors = new Dictionary<string, List<string>>();

        foreach (var argument in context.Arguments)
        {
            if (argument is null || !ShouldValidate(argument.GetType()))
            {
                continue;
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(argument);

            if (Validator.TryValidateObject(
                    argument,
                    validationContext,
                    validationResults,
                    validateAllProperties: true))
            {
                continue;
            }

            foreach (var result in validationResults)
            {
                var members = result.MemberNames.Any()
                    ? result.MemberNames
                    : [string.Empty];

                foreach (var member in members)
                {
                    if (!fieldErrors.TryGetValue(member, out var messages))
                    {
                        messages = [];
                        fieldErrors[member] = messages;
                    }

                    messages.Add(result.ErrorMessage ?? "Invalid value.");
                }
            }
        }

        if (fieldErrors.Count > 0)
        {
            var errors = fieldErrors.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToArray());

            var firstMessage = errors.Values.SelectMany(messages => messages).FirstOrDefault()
                ?? "Validation failed.";

            throw new ApiException(
                StatusCodes.Status400BadRequest,
                firstMessage,
                ApiErrorCodes.ValidationError,
                errors);
        }

        return await next(context);
    }

    private static bool ShouldValidate(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
        {
            return false;
        }

        if (type.Namespace?.StartsWith("MovieNest.Api.Dtos", StringComparison.Ordinal) == true)
        {
            return true;
        }

        return type.GetProperties().Any(property =>
            property.GetCustomAttributes(typeof(ValidationAttribute), inherit: true).Length > 0);
    }
}
